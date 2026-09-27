using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Work;

// State of one conversion run and the safe-for-threads work on a single file.
public sealed class Job
{
    private readonly Dictionary<MediaKind, IConverter> _conv;
    private readonly PathMaker _paths;
    private readonly IDictionary<MediaKind, Target> _pick;
    private readonly string _src;
    private readonly string _dst;
    private readonly double _total;
    private readonly IProgress<Step> _log;
    private readonly CancellationToken _ct;
    private readonly HashSet<string> _used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly object _gate = new object();
    private long _done;
    private long _last = -1000;

    public Job(Dictionary<MediaKind, IConverter> conv, PathMaker paths, IDictionary<MediaKind, Target> pick,
        string src, string dst, double total, IProgress<Step> log, CancellationToken ct)
    {
        _conv = conv;
        _paths = paths;
        _pick = pick;
        _src = src;
        _dst = dst;
        _total = total;
        _log = log;
        _ct = ct;
    }

    public Outcome Res { get; } = new Outcome();

    public void Do(MediaFile f)
    {
        if (_ct.IsCancellationRequested) return;
        Target t;
        IConverter conv;
        if (!_pick.TryGetValue(f.Kind, out t) || !_conv.TryGetValue(f.Kind, out conv)) return;

        string outPath;
        lock (_gate) outPath = _paths.Make(_src, _dst, f, t.Ext, _used);
        long size = Math.Max(f.Size, 1);
        string name = Path.GetFileName(f.Path);
        Inline<double> sub = new Inline<double>(x =>
        {
            if (x < 1) Post(Interlocked.Read(ref _done) + x * size, name, false);
        });

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            if (File.Exists(outPath)) File.Delete(outPath);
            conv.Run(f, t, outPath, sub, _ct);
            long len = new FileInfo(outPath).Length;
            lock (_gate)
            {
                Res.Ok++;
                Res.In += f.Size;
                Res.Out += len;
            }
        }
        catch (OperationCanceledException)
        {
            TryDelete(outPath);
            return;
        }
        catch (Exception)
        {
            TryDelete(outPath);
            lock (_gate) Res.Fail++;
        }

        Interlocked.Add(ref _done, size);
        Post(Interlocked.Read(ref _done), name, false);
    }

    public void Finish()
    {
        Post(_total, "", true);
    }

    private void Post(double bytes, string name, bool force)
    {
        lock (_gate)
        {
            long now = _clock.ElapsedMilliseconds;
            if (!force && now - _last < 200) return;
            _last = now;
        }
        double d = Math.Min(1, bytes / _total);
        TimeSpan? left = d < 0.005 ? (TimeSpan?)null : TimeSpan.FromSeconds(_clock.Elapsed.TotalSeconds * (1 - d) / d);
        _log.Report(new Step { Done = d, Left = left, Now = name });
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
        }
        catch (Exception)
        {
        }
    }
}
