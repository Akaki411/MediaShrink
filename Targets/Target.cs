using MediaShrink.Models;
using MediaShrink.Tools;

namespace MediaShrink.Targets;

// Base description of an output format: extension, codec label and hints for size guessing.
public class Target
{
    public string Id { get; set; }
    public MediaKind Kind { get; set; }
    public string Name { get; set; }
    public string Ext { get; set; }
    public string Codec { get; set; } = "";
    public double Eff { get; set; } = 1;
    public bool Lossy { get; set; } = true;
    public int Kbps { get; set; }

    public virtual bool Ready(IEncoders enc)
    {
        return true;
    }
}
