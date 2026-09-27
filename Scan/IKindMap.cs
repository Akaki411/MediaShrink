using MediaShrink.Models;

namespace MediaShrink.Scan;

// Decides the media kind of a file by its extension.
public interface IKindMap
{
    MediaKind Of(string ext);
}
