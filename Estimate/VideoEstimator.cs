using System;
using System.Collections.Generic;
using MediaShrink.Models;
using MediaShrink.Targets;
using MediaShrink.Tools;

namespace MediaShrink.Estimate;

// Guesses video size from how efficient the old and new codecs are compared with H.264. GPU encoders make bigger files.
public sealed class VideoEstimator : IEstimator
{
    private const double Retune = 0.72;
    private const double Unknown = 1.3;
    private const double GpuExtra = 1.25;

    private static readonly Dictionary<string, double> Eff = new Dictionary<string, double>
    {
        { "h264", 1.0 }, { "hevc", 0.6 }, { "vp9", 0.62 }, { "av1", 0.45 }, { "vp8", 1.15 },
        { "mpeg4", 1.6 }, { "mpeg2video", 2.2 }, { "mpeg1video", 3.0 }, { "wmv1", 1.9 }, { "wmv2", 1.7 },
        { "wmv3", 1.5 }, { "vc1", 1.1 }, { "theora", 1.5 }, { "mjpeg", 6.0 }, { "prores", 12.0 },
        { "dnxhd", 12.0 }, { "h263", 2.2 }, { "flv1", 2.0 }, { "msmpeg4v3", 1.9 }, { "msmpeg4v2", 2.0 },
        { "rawvideo", 60.0 }, { "ffv1", 8.0 }
    };

    private readonly IAccel _accel;

    public VideoEstimator(IAccel accel)
    {
        _accel = accel;
    }

    public MediaKind Kind
    {
        get { return MediaKind.Video; }
    }

    public long Guess(MediaFile f, Target t)
    {
        double src;
        if (!Eff.TryGetValue(f.Codec, out src)) src = Unknown;
        double ratio = Retune * t.Eff / src;
        FfTarget ft = t as FfTarget;
        if (ft != null && ft.Family != null && _accel.Video(ft.Family) != null) ratio *= GpuExtra;
        ratio = Math.Max(0.05, Math.Min(3, ratio));
        return (long)(f.Size * ratio);
    }
}
