using System.Collections.Generic;
using MediaShrink.Models;

namespace MediaShrink.Targets;

// Audio formats for ffmpeg. {k} in the arguments is replaced by the bitrate in kbit/s.
public sealed class AudioTargets : ITargetSource
{
    public MediaKind Kind
    {
        get { return MediaKind.Audio; }
    }

    public IEnumerable<Target> All()
    {
        return new List<Target>
        {
            Lossy("opus", ".opus", "Opus", 96, "-c:a libopus -b:a {k}k -vbr on", "libopus"),
            Lossy("ogg", ".ogg", "Vorbis", 112, "-c:a libvorbis -b:a {k}k", "libvorbis"),
            Lossy("m4a", ".m4a", "AAC", 128, "-c:a aac -b:a {k}k", "aac"),
            Lossy("mp3", ".mp3", "MP3", 128, "-c:a libmp3lame -b:a {k}k", "libmp3lame"),
            Lossy("aac", ".aac", "AAC", 128, "-c:a aac -b:a {k}k", "aac"),
            Lossy("mka", ".mka", "Opus", 96, "-c:a libopus -b:a {k}k -vbr on", "libopus"),
            Lossy("wma", ".wma", "WMA", 128, "-c:a wmav2 -b:a {k}k", "wmav2"),
            Lossy("ac3", ".ac3", "AC-3", 192, "-c:a ac3 -b:a {k}k", "ac3"),
            Lossy("mp2", ".mp2", "MP2", 192, "-c:a mp2 -b:a {k}k", "mp2"),
            Lossy("spx", ".spx", "Speex", 24, "-c:a libspeex -ar 16000 -ac 1 -b:a {k}k", "libspeex"),
            Lossy("amr", ".amr", "AMR-NB", 12, "-c:a libopencore_amrnb -ar 8000 -ac 1 -b:a 12.2k", "libopencore_amrnb"),
            Lossless("flac", ".flac", "FLAC", 0.55, "-c:a flac -compression_level 8", "flac"),
            Lossless("alac", ".m4a", "ALAC", 0.55, "-c:a alac", "alac"),
            Lossless("wav", ".wav", "PCM", 1.0, "-c:a pcm_s16le", "pcm_s16le"),
            Lossless("aiff", ".aiff", "PCM", 1.0, "-c:a pcm_s16be", "pcm_s16be")
        };
    }

    private static Target Lossy(string id, string ext, string codec, int kbps, string args, string need)
    {
        return new FfTarget
        {
            Id = id, Kind = MediaKind.Audio, Ext = ext, Codec = codec, Name = Title(ext, codec),
            Kbps = kbps, Lossy = true, Args = args, Need = new[] { need }
        };
    }

    private static Target Lossless(string id, string ext, string codec, double eff, string args, string need)
    {
        return new FfTarget
        {
            Id = id, Kind = MediaKind.Audio, Ext = ext, Codec = codec, Name = Title(ext, codec),
            Eff = eff, Lossy = false, Args = args, Need = new[] { need }
        };
    }

    private static string Title(string ext, string codec)
    {
        string name = ext.TrimStart('.').ToUpperInvariant();
        return name == codec.ToUpperInvariant() ? name : name + " · " + codec;
    }
}
