using System.Collections.Generic;
using System.Linq;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Estimate;

// Uses the estimators to show the new groups and the size change of each format.
public sealed class Planner : IPlanner
{
    private readonly Dictionary<MediaKind, IEstimator> _est;

    public Planner(IEnumerable<IEstimator> estimators)
    {
        _est = estimators.ToDictionary(e => e.Kind);
    }

    public List<Group> After(List<Group> groups, IDictionary<MediaKind, Target> pick)
    {
        List<Group> list = new List<Group>();
        foreach (Group g in groups)
        {
            Target t;
            if (g.Kind == MediaKind.Other || !pick.TryGetValue(g.Kind, out t)) continue;
            IEstimator e = _est[g.Kind];
            Group a = new Group
            {
                Key = g.Key, Kind = g.Kind, Ext = t.Ext, Codec = t.Codec, Hue = g.Hue, Count = g.Count
            };
            foreach (MediaFile f in g.Files) a.Bytes += e.Guess(f, t);
            list.Add(a);
        }
        return list;
    }

    public Dictionary<Target, double> Deltas(List<MediaFile> files, IEnumerable<Target> targets)
    {
        Dictionary<Target, double> map = new Dictionary<Target, double>();
        foreach (Target t in targets)
        {
            long before = 0;
            long after = 0;
            IEstimator e = _est[t.Kind];
            foreach (MediaFile f in files)
            {
                if (f.Kind != t.Kind) continue;
                before += f.Size;
                after += e.Guess(f, t);
            }
            map[t] = before > 0 ? (double)after / before - 1 : 0;
        }
        return map;
    }
}
