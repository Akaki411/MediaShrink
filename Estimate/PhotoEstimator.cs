using System;
using System.Collections.Generic;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Estimate;

// Guesses photo size from how big each format usually is compared with JPEG.
public sealed class PhotoEstimator : IEstimator
{
    private const double Retune = 0.85;

    private static readonly Dictionary<string, double> Eff = new Dictionary<string, double>
    {
        { ".jpg", 1.0 }, { ".jpeg", 1.0 }, { ".jpe", 1.0 }, { ".jfif", 1.0 }, { ".png", 3.2 },
        { ".gif", 3.0 }, { ".bmp", 14.0 }, { ".dib", 14.0 }, { ".tif", 5.0 }, { ".tiff", 5.0 },
        { ".webp", 0.72 }, { ".avif", 0.5 }, { ".heic", 0.55 }, { ".heif", 0.55 }, { ".jxl", 0.55 },
        { ".jp2", 1.05 }, { ".j2k", 1.05 }, { ".tga", 11.0 }, { ".ico", 4.0 }, { ".psd", 7.0 }, { ".pcx", 10.0 }
    };

    public MediaKind Kind
    {
        get { return MediaKind.Photo; }
    }

    public long Guess(MediaFile f, Target t)
    {
        double src;
        if (!Eff.TryGetValue(f.Ext, out src)) src = 1.0;
        double ratio = t.Eff / src;
        if (t.Lossy) ratio *= Retune;
        ratio = Math.Max(0.02, Math.Min(40, ratio));
        return (long)(f.Size * ratio);
    }
}
