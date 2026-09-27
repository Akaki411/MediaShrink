using System.Collections.Generic;

namespace MediaShrink.Models;

// Friendly codec names and a check for lossless codecs.
public static class Codecs
{
    private static readonly Dictionary<string, string> Names = new Dictionary<string, string>
    {
        { "h264", "H.264" }, { "hevc", "H.265" }, { "mpeg4", "MPEG-4" }, { "mpeg2video", "MPEG-2" },
        { "mpeg1video", "MPEG-1" }, { "vp8", "VP8" }, { "vp9", "VP9" }, { "av1", "AV1" },
        { "wmv1", "WMV" }, { "wmv2", "WMV" }, { "wmv3", "WMV" }, { "vc1", "VC-1" },
        { "theora", "Theora" }, { "mjpeg", "MJPEG" }, { "prores", "ProRes" }, { "dnxhd", "DNxHD" },
        { "h263", "H.263" }, { "flv1", "FLV1" }, { "msmpeg4v3", "MSMPEG4" }, { "msmpeg4v2", "MSMPEG4" },
        { "rawvideo", "Raw" }, { "ffv1", "FFV1" }, { "aac", "AAC" }, { "mp3", "MP3" }, { "mp2", "MP2" },
        { "opus", "Opus" }, { "vorbis", "Vorbis" }, { "flac", "FLAC" }, { "alac", "ALAC" },
        { "ac3", "AC-3" }, { "eac3", "E-AC-3" }, { "dts", "DTS" }, { "wmav1", "WMA" }, { "wmav2", "WMA" },
        { "wmapro", "WMA Pro" }, { "amr_nb", "AMR-NB" }, { "amr_wb", "AMR-WB" }, { "ape", "APE" },
        { "wavpack", "WavPack" }, { "tta", "TTA" }, { "speex", "Speex" }
    };

    private static readonly HashSet<string> Lossless = new HashSet<string>
    {
        "flac", "alac", "ape", "wavpack", "tta", "truehd", "mlp"
    };

    public static string Nice(string codec)
    {
        string name;
        if (Names.TryGetValue(codec, out name)) return name;
        if (codec.StartsWith("pcm")) return "PCM";
        return codec.ToUpperInvariant();
    }

    public static bool IsLossless(string codec)
    {
        return Lossless.Contains(codec) || codec.StartsWith("pcm");
    }
}
