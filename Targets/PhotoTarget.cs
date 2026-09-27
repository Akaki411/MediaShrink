using System.Collections.Generic;
using ImageMagick;
using MediaShrink.Tools;

namespace MediaShrink.Targets;

// A photo format written by Magick.NET, with its quality and coder options.
public sealed class PhotoTarget : Target
{
    public MagickFormat Fmt { get; set; }
    public uint Q { get; set; } = 80;
    public bool Alpha { get; set; } = true;
    public bool Anim { get; set; }
    public bool Prog { get; set; }
    public CompressionMethod? Comp { get; set; }
    public Dictionary<string, string> Defs { get; } = new Dictionary<string, string>();

    public override bool Ready(IEncoders enc)
    {
        try
        {
            IMagickFormatInfo info = MagickFormatInfo.Create(Fmt);
            return info != null && info.SupportsWriting;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
