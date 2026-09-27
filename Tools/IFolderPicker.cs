namespace MediaShrink.Tools;

// Asks the user to choose a folder. Returns null when the user cancels.
public interface IFolderPicker
{
    string Pick(string start);
}
