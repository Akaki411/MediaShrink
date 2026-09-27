using System;
using System.Threading;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Work;

// Converts one file of a given media kind. Reports progress from 0 to 1.
public interface IConverter
{
    MediaKind Kind { get; }
    void Run(MediaFile file, Target target, string dst, IProgress<double> log, CancellationToken ct);
}
