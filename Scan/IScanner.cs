using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaShrink.Models;

namespace MediaShrink.Scan;

// Finds and describes all files in a folder.
public interface IScanner
{
    Task<List<MediaFile>> Run(string dir, IProgress<string> log, CancellationToken ct);
}
