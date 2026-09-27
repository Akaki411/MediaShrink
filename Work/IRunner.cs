using System;
using System.Collections.Generic;
using System.Threading;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Work;

// Converts a list of files one by one and reports overall progress.
public interface IRunner
{
    Outcome Run(List<MediaFile> files, IDictionary<MediaKind, Target> pick, string src, string dst,
        IProgress<Step> log, CancellationToken ct);
}
