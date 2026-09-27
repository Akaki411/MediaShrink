using MediaShrink.Models;

namespace MediaShrink.Scan;

// Reads codec, length and bitrate of an audio or video file. Returns false if it is not real media.
public interface IProbe
{
    bool Fill(MediaFile file);
}
