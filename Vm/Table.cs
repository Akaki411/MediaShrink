using System.Collections.Generic;

namespace MediaShrink.Vm;

// Everything one side of the window shows: table rows, pie slices and the total size.
public sealed class Table
{
    public List<Row> Rows { get; } = new List<Row>();
    public List<Slice> Slices { get; } = new List<Slice>();
    public long Bytes { get; set; }
    public int Count { get; set; }
}
