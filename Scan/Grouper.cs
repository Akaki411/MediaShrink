using System.Collections.Generic;
using System.Linq;
using MediaShrink.Models;

namespace MediaShrink.Scan;

// Puts files into groups by kind, extension and codec; non-media files form one "Other" group.
public sealed class Grouper
{
    private const int Hues = 12;

    public List<Group> Make(IEnumerable<MediaFile> files)
    {
        Dictionary<string, Group> map = new Dictionary<string, Group>();
        foreach (MediaFile f in files)
        {
            bool other = f.Kind == MediaKind.Other;
            string key = other ? "other" : f.Kind + "|" + f.Ext + "|" + f.Codec;
            Group g;
            if (!map.TryGetValue(key, out g))
            {
                g = new Group
                {
                    Key = key,
                    Kind = f.Kind,
                    Ext = other ? "Другое" : f.Ext,
                    Codec = other || f.Kind == MediaKind.Photo ? "" : Codecs.Nice(f.Codec),
                    Hue = -1
                };
                map[key] = g;
            }
            g.Count++;
            g.Bytes += f.Size;
            g.Files.Add(f);
        }

        List<Group> list = map.Values.OrderByDescending(g => g.Bytes).ToList();
        int n = 0;
        foreach (Group g in list)
        {
            if (g.Kind != MediaKind.Other) g.Hue = n++ % Hues;
        }
        return list;
    }
}
