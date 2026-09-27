using System.Collections.Generic;
using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Estimate;

// Builds the "after conversion" picture from the "before" groups and the chosen formats.
public interface IPlanner
{
    List<Group> After(List<Group> groups, IDictionary<MediaKind, Target> pick);
    Dictionary<Target, double> Deltas(List<MediaFile> files, IEnumerable<Target> targets);
}
