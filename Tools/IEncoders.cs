namespace MediaShrink.Tools;

// Tells which ffmpeg encoders exist in the current build.
public interface IEncoders
{
    bool Has(string name);
}
