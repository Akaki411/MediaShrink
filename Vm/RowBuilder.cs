using System.Collections.Generic;
using System.Linq;
using MediaShrink.Core;
using MediaShrink.Models;

namespace MediaShrink.Vm;

// Turns groups into table rows and pie slices with their share of the total size.
public sealed class RowBuilder
{
    public Table Build(IEnumerable<Group> groups)
    {
        List<Group> list = groups.OrderByDescending(g => g.Bytes).ToList();
        Table tab = new Table { Bytes = list.Sum(g => g.Bytes), Count = list.Sum(g => g.Count) };
        foreach (Group g in list)
        {
            double pct = tab.Bytes > 0 ? g.Bytes * 100.0 / tab.Bytes : 0;
            tab.Rows.Add(new Row
            {
                Color = Palette.Fill(g.Hue),
                Tint = Palette.Tint(g.Hue),
                Ext = g.Ext,
                Kind = g.Kind == MediaKind.Other ? "" : Kinds.Name(g.Kind),
                Codec = g.Codec,
                Pct = pct,
                PctText = Fmt.Pct(pct),
                SizeText = Fmt.Size(g.Bytes),
                Count = g.Count
            });
            tab.Slices.Add(new Slice { Color = Palette.Fill(g.Hue), Pct = pct });
        }
        return tab;
    }
}
