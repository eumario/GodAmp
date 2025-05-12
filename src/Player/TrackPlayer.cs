using Godot;
using SpectralFX.Data;

namespace SpectralFX.Player;

public partial class TrackPlayer : AudioStreamPlayer
{
    public Track CurrentTrack;

    public void SetCurrentTrack(Track track, bool autoplay = true)
    {
        CurrentTrack = track;
        Stream = CurrentTrack.Stream;
        Seek(0.0f);
        Playing = autoplay;
    }
}