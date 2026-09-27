using System.Collections.Generic;

namespace MediaShrink.Models;

// Files that share kind, extension and codec, with their total size.
public sealed class Group
{
    public string Key { get; set; }
    public MediaKind Kind { get; set; }
    public string Ext { get; set; } = "";
    public string Codec { get; set; } = "";
    public int Hue { get; set; }
    public int Count { get; set; }
    public long Bytes { get; set; }
    public List<MediaFile> Files { get; } = new List<MediaFile>();
}
