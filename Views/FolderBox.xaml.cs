using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaShrink.Views;

// Code for the folder box: it only exposes the caption, path and command.
public partial class FolderBox : UserControl
{
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(FolderBox));

    public static readonly DependencyProperty PathProperty =
        DependencyProperty.Register("Path", typeof(string), typeof(FolderBox));

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(FolderBox));

    public FolderBox()
    {
        InitializeComponent();
    }

    public string Caption
    {
        get { return (string)GetValue(CaptionProperty); }
        set { SetValue(CaptionProperty, value); }
    }

    public string Path
    {
        get { return (string)GetValue(PathProperty); }
        set { SetValue(PathProperty, value); }
    }

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }
}
