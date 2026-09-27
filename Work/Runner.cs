using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Work;

// Runs photos in parallel (one thread per core), then audio and video one by one because ffmpeg uses all cores itself.
public sealed class Runner : IRunner
{
    private readonly Dictionary<MediaKind, IConverter> _conv;
    private readonly PathMaker _paths;

    public Runner(IEnumerable<IConverter> converters, PathMaker paths)
    {
        _conv = converters.ToDictionary(c => c.Kind);
        _paths = paths;
    }

    public Outcome Run(List<MediaFile> files, IDictionary<MediaKind, Target> pick, string src, string dst,
        IProgress<Step> log, CancellationToken ct)
    {
        double total = Math.Max(1, files.Sum(f => Math.Max(f.Size, 1)));
        Job job = new Job(_conv, _paths, pick, src, dst, total, log, ct);

        List<MediaFile> photos = files.Where(f => f.Kind == MediaKind.Photo).ToList();
        ParallelOptions opt = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = ct };
        try
        {
            Parallel.ForEach(photos, opt, job.Do);
        }
        catch (OperationCanceledException)
        {
        }

        foreach (MediaFile f in files.Where(f => f.Kind != MediaKind.Photo))
        {
            if (ct.IsCancellationRequested) break;
            job.Do(f);
        }

        job.Res.Stopped = ct.IsCancellationRequested;
        job.Finish();
        return job.Res;
    }
}
