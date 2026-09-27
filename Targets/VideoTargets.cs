using System.Collections.Generic;
using MediaShrink.Models;

namespace MediaShrink.Targets;

// Video formats for ffmpeg: container plus codec. Eff is the size of the codec compared with H.264.
public sealed class VideoTargets : ITargetSource
{
    private const string X265 = "-c:v libx265 -crf 28 -preset medium -pix_fmt yuv420p";
    private const string X264 = "-c:v libx264 -crf 24 -preset medium -pix_fmt yuv420p";
    private const string Vp9 = "-c:v libvpx-vp9 -crf 33 -b:v 0 -deadline good -cpu-used 4 -row-mt 1 -pix_fmt yuv420p";
    private const string Av1 = "-c:v libaom-av1 -crf 34 -b:v 0 -cpu-used 6 -row-mt 1 -pix_fmt yuv420p";
    private const string Aac = "-c:a aac -b:a 128k";
    private const string Opus = "-c:a libopus -b:a 96k";
    private const string Mp3 = "-c:a libmp3lame -b:a 128k";

    public MediaKind Kind
    {
        get { return MediaKind.Video; }
    }

    public IEnumerable<Target> All()
    {
        return new List<Target>
        {
            Gpu("mp4-h265", ".mp4", "H.265", 0.60, "hevc", X265, "-tag:v hvc1 " + Aac + " -movflags +faststart", "libx265", "aac"),
            Gpu("mkv-h265", ".mkv", "H.265", 0.60, "hevc", X265, Opus, "libx265", "libopus"),
            Make("mkv-av1", ".mkv", "AV1", 0.45, Av1 + " " + Opus, "libaom-av1", "libopus"),
            Make("webm-av1", ".webm", "AV1", 0.45, Av1 + " " + Opus, "libaom-av1", "libopus"),
            Make("mp4-av1", ".mp4", "AV1", 0.45, Av1 + " " + Aac + " -movflags +faststart", "libaom-av1", "aac"),
            Make("mkv-vp9", ".mkv", "VP9", 0.62, Vp9 + " " + Opus, "libvpx-vp9", "libopus"),
            Make("webm-vp9", ".webm", "VP9", 0.62, Vp9 + " " + Opus, "libvpx-vp9", "libopus"),
            Gpu("mp4-h264", ".mp4", "H.264", 1.00, "h264", X264, Aac + " -movflags +faststart", "libx264", "aac"),
            Gpu("mkv-h264", ".mkv", "H.264", 1.00, "h264", X264, Opus, "libx264", "libopus"),
            Gpu("mov-h265", ".mov", "H.265", 0.60, "hevc", X265, "-tag:v hvc1 " + Aac, "libx265", "aac"),
            Gpu("mov-h264", ".mov", "H.264", 1.00, "h264", X264, Aac, "libx264", "aac"),
            Gpu("ts-h265", ".ts", "H.265", 0.60, "hevc", X265, Aac, "libx265", "aac"),
            Gpu("ts-h264", ".ts", "H.264", 1.00, "h264", X264, Aac, "libx264", "aac"),
            Gpu("flv-h264", ".flv", "H.264", 1.00, "h264", X264, Aac, "libx264", "aac"),
            Gpu("3gp-h264", ".3gp", "H.264", 1.05, "h264",
                "-c:v libx264 -crf 26 -preset medium -pix_fmt yuv420p", "-c:a aac -b:a 96k", "libx264", "aac"),
            Gpu("avi-h264", ".avi", "H.264", 1.00, "h264", X264, Mp3, "libx264", "libmp3lame"),
            Make("webm-vp8", ".webm", "VP8", 1.15, "-c:v libvpx -crf 12 -b:v 4M -deadline good -cpu-used 4 -pix_fmt yuv420p -c:a libvorbis -b:a 96k", "libvpx", "libvorbis"),
            Make("mp4-mpeg4", ".mp4", "MPEG-4", 1.60, "-c:v mpeg4 -q:v 5 -pix_fmt yuv420p " + Aac + " -movflags +faststart", "mpeg4", "aac"),
            Make("avi-xvid", ".avi", "Xvid", 1.50, "-c:v libxvid -q:v 5 -pix_fmt yuv420p -vtag XVID " + Mp3, "libxvid", "libmp3lame"),
            Make("ogv-theora", ".ogv", "Theora", 1.50, "-c:v libtheora -q:v 6 -pix_fmt yuv420p -c:a libvorbis -b:a 128k", "libtheora", "libvorbis"),
            Make("wmv-wmv2", ".wmv", "WMV", 1.70, "-c:v wmv2 -q:v 4 -pix_fmt yuv420p -c:a wmav2 -b:a 128k", "wmv2", "wmav2"),
            Make("mpg-mpeg2", ".mpg", "MPEG-2", 2.20, "-c:v mpeg2video -q:v 4 -pix_fmt yuv420p -c:a mp2 -b:a 192k", "mpeg2video", "mp2"),
            Make("avi-mjpeg", ".avi", "MJPEG", 6.00, "-c:v mjpeg -q:v 5 -pix_fmt yuvj420p -c:a pcm_s16le", "mjpeg", "pcm_s16le"),
            Make("mov-prores", ".mov", "ProRes", 12.0, "-c:v prores_ks -profile:v 3 -pix_fmt yuv422p10le -c:a pcm_s16le", "prores_ks", "pcm_s16le")
        };
    }

    private static Target Make(string id, string ext, string codec, double eff, string args, params string[] need)
    {
        return new FfTarget
        {
            Id = id, Kind = MediaKind.Video, Ext = ext, Codec = codec, Eff = eff, Lossy = true,
            Name = ext.TrimStart('.').ToUpperInvariant() + " · " + codec, Args = args, Need = need
        };
    }

    private static Target Gpu(string id, string ext, string codec, double eff, string family, string soft, string tail,
        params string[] need)
    {
        FfTarget t = (FfTarget)Make(id, ext, codec, eff, "{v} " + tail, need);
        t.Family = family;
        t.Soft = soft;
        return t;
    }
}
