namespace MediaShrink.Tools;

// Hardware video encoding: gives ready video arguments for a codec family, or null if there is no working GPU.
public interface IAccel
{
    string Video(string family);
    void Ok(string family);
    void Fail(string family);
    string Info();
}
