using System.Collections.Generic;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Estimate;

// Guesses audio size from length and bitrate; lossless targets are counted against raw PCM size.
public sealed class AudioEstimator : IEstimator
{
    private const double CdBytesPerSec = 176400;

    private static readonly Dictionary<string, double> LosslessEff = new Dictionary<string, double>
    {
        { "flac", 0.55 }, { "alac", 0.55 }, { "ape", 0.5 }, { "wavpack", 0.55 }, { "tta", 0.55 }
    };

    public MediaKind Kind
    {
        get { return MediaKind.Audio; }
    }

    public long Guess(MediaFile f, Target t)
    {
        if (f.Secs <= 0) return (long)(f.Size * (t.Lossy ? 0.4 : 1.0));
        if (t.Lossy) return (long)(f.Secs * Rate.For(t, f) * 125);

        double pcm = f.Secs * CdBytesPerSec;
        if (Codecs.IsLossless(f.Codec))
        {
            double eff;
            pcm = f.Size / (LosslessEff.TryGetValue(f.Codec, out eff) ? eff : 1.0);
        }
        return (long)(pcm * t.Eff);
    }
}
