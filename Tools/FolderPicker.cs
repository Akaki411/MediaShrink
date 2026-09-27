using System.IO;
using WinForms = System.Windows.Forms;

namespace MediaShrink.Tools;

// Folder dialog from Windows Forms, which works on Windows 7 and newer.
public sealed class FolderPicker : IFolderPicker
{
    public string Pick(string start)
    {
        using (WinForms.FolderBrowserDialog dlg = new WinForms.FolderBrowserDialog())
        {
            dlg.ShowNewFolderButton = true;
            if (!string.IsNullOrEmpty(start) && Directory.Exists(start)) dlg.SelectedPath = start;
            return dlg.ShowDialog() == WinForms.DialogResult.OK ? dlg.SelectedPath : null;
        }
    }
}
