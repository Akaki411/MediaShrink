using MediaShrink.Models;
using MediaShrink.Targets;

namespace MediaShrink.Estimate;

// Guesses the size of one file after it is converted to a target format.
public interface IEstimator
{
    MediaKind Kind { get; }
    long Guess(MediaFile file, Target target);
}
