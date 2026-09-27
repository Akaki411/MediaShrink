using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace MediaShrink.Views;

// Code for the table control: it only exposes the rows and the empty-state hint.
public partial class MediaTable : UserControl
{
    public static readonly DependencyProperty RowsProperty =
        DependencyProperty.Register("Rows", typeof(IEnumerable), typeof(MediaTable));

    public static readonly DependencyProperty HintProperty =
        DependencyProperty.Register("Hint", typeof(string), typeof(MediaTable));

    public MediaTable()
    {
        InitializeComponent();
    }

    public IEnumerable Rows
    {
        get { return (IEnumerable)GetValue(RowsProperty); }
        set { SetValue(RowsProperty, value); }
    }

    public string Hint
    {
        get { return (string)GetValue(HintProperty); }
        set { SetValue(HintProperty, value); }
    }
}
