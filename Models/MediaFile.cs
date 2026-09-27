namespace MediaShrink.Models;

// One file found in the source folder, with what we know about its media.
public sealed class MediaFile
{
    public string Path { get; set; }
    public string Ext { get; set; }
    public long Size { get; set; }
    public MediaKind Kind { get; set; }
    public string Codec { get; set; } = "";
    public double Secs { get; set; }
    public int Kbps { get; set; }
}
