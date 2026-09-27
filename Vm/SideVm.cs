using System.Collections.Generic;
using MediaShrink.Core;

namespace MediaShrink.Vm;

// One side of the window: folder path, table rows, pie slices and text totals.
public sealed class SideVm : Bindable
{
    private string _path = "";
    private List<Row> _rows = new List<Row>();
    private List<Slice> _slices = new List<Slice>();
    private string _total = "";
    private string _caption = "";
    private string _info = "";

    public string Path
    {
        get { return _path; }
        set { Set(ref _path, value); }
    }

    public List<Row> Rows
    {
        get { return _rows; }
        private set { Set(ref _rows, value); }
    }

    public List<Slice> Slices
    {
        get { return _slices; }
        private set { Set(ref _slices, value); }
    }

    public string Total
    {
        get { return _total; }
        private set { Set(ref _total, value); }
    }

    public string Caption
    {
        get { return _caption; }
        private set { Set(ref _caption, value); }
    }

    public string Info
    {
        get { return _info; }
        set { Set(ref _info, value); }
    }

    public void Show(Table tab, string caption)
    {
        Rows = tab.Rows;
        Slices = tab.Slices;
        Total = tab.Count == 0 ? "" : Fmt.Size(tab.Bytes);
        Caption = tab.Count == 0 ? "" : caption;
    }
}
