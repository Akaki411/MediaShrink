using NReco.VideoConverter;

namespace MediaShrink.Tools;

// Gives ready-to-use ffmpeg wrappers and the path of the ffmpeg program.
public interface IFfTool
{
    FFMpegConverter Make();
    string Exe();
}
