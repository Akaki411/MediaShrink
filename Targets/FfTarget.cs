using System.Linq;
using MediaShrink.Tools;

namespace MediaShrink.Targets;

// An audio or video format made by ffmpeg. {v} in Args is replaced by GPU or CPU video arguments (Family: h264 or hevc).
public sealed class FfTarget : Target
{
    public string Args { get; set; }
    public string Soft { get; set; } = "";
    public string Family { get; set; }
    public string[] Need { get; set; } = new string[0];

    public override bool Ready(IEncoders enc)
    {
        return Need.All(enc.Has);
    }
}
