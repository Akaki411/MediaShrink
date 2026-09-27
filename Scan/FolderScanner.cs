using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaShrink.Models;

namespace MediaShrink.Scan;

// Walks a folder tree, sorts files by kind, then probes audio and video in parallel.
public sealed class FolderScanner : IScanner
{
    private readonly IKindMap _map;
    private readonly IProbe _probe;

    public FolderScanner(IKindMap map, IProbe probe)
    {
        _map = map;
        _probe = probe;
    }

    public Task<List<MediaFile>> Run(string dir, IProgress<string> log, CancellationToken ct)
    {
        return Task.Run(() => Scan(dir, log, ct), ct);
    }

    private List<MediaFile> Scan(string dir, IProgress<string> log, CancellationToken ct)
    {
        List<MediaFile> all = new List<MediaFile>();
        List<MediaFile> av = new List<MediaFile>();
        foreach (FileInfo fi in Walk(dir, ct))
        {
            MediaKind kind = _map.Of(fi.Extension);
            MediaFile f = new MediaFile
            {
                Path = fi.FullName,
                Ext = fi.Extension.ToLowerInvariant(),
                Size = fi.Length,
                Kind = kind
            };
            all.Add(f);
            if (kind == MediaKind.Audio || kind == MediaKind.Video) av.Add(f);
            if (all.Count % 500 == 0) log.Report("Поиск файлов: " + all.Count.ToString("N0"));
        }

        int done = 0;
        ParallelOptions opt = new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct };
        Parallel.ForEach(av, opt, f =>
        {
            if (!_probe.Fill(f)) f.Kind = MediaKind.Other;
            int n = Interlocked.Increment(ref done);
            if (n % 5 == 0 || n == av.Count) log.Report("Анализ аудио и видео: " + n + " из " + av.Count);
        });
        return all;
    }

    private static IEnumerable<FileInfo> Walk(string root, CancellationToken ct)
    {
        Stack<DirectoryInfo> todo = new Stack<DirectoryInfo>();
        todo.Push(new DirectoryInfo(root));
        while (todo.Count > 0)
        {
            ct.ThrowIfCancellationRequested();
            DirectoryInfo dir = todo.Pop();
            FileInfo[] files;
            DirectoryInfo[] subs;
            try
            {
                files = dir.GetFiles();
                subs = dir.GetDirectories();
            }
            catch (Exception)
            {
                continue;
            }
            foreach (FileInfo f in files) yield return f;
            foreach (DirectoryInfo s in subs)
            {
                if ((s.Attributes & FileAttributes.ReparsePoint) == 0) todo.Push(s);
            }
        }
    }
}
