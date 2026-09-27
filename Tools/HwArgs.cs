using System.Collections.Generic;

namespace MediaShrink.Tools;

// Video arguments and vendor names for the hardware encoders, tuned to look like the CPU encoders.
public static class HwArgs
{
    private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
    {
        { "h264_nvenc", "-c:v h264_nvenc -preset p5 -rc vbr -cq 26 -b:v 0 -pix_fmt yuv420p" },
        { "hevc_nvenc", "-c:v hevc_nvenc -preset p5 -rc vbr -cq 30 -b:v 0 -pix_fmt yuv420p" },
        { "h264_qsv", "-c:v h264_qsv -preset medium -global_quality 26 -pix_fmt nv12" },
        { "hevc_qsv", "-c:v hevc_qsv -preset medium -global_quality 28 -pix_fmt nv12" },
        { "h264_amf", "-c:v h264_amf -quality quality -rc cqp -qp_i 24 -qp_p 26 -pix_fmt yuv420p" },
        { "hevc_amf", "-c:v hevc_amf -quality quality -rc cqp -qp_i 26 -qp_p 28 -pix_fmt yuv420p" }
    };

    public static IEnumerable<string> Order(string family)
    {
        yield return family + "_nvenc";
        yield return family + "_qsv";
        yield return family + "_amf";
    }

    public static string For(string encoder)
    {
        return Map[encoder];
    }

    public static string Vendor(string encoder)
    {
        if (encoder.EndsWith("nvenc")) return "NVIDIA NVENC";
        if (encoder.EndsWith("qsv")) return "Intel Quick Sync";
        return "AMD AMF";
    }
}
