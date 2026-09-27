using System;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Estimate;

// Picks the audio bitrate: the target rate, but never more than a lossy source already has.
public static class Rate
{
    public static int For(Target target, MediaFile file)
    {
        int k = target.Kbps;
        if (file.Kbps > 0 && !Codecs.IsLossless(file.Codec)) k = Math.Min(k, Math.Max(file.Kbps, 32));
        return k;
    }
}
