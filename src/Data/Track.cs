using Godot;

namespace SpectralFX.Data;

public partial class Track : Resource
{
    public string Name;
    public string Artist;
    public string Album;
    public float Duration;
    public int TrackNumber;
    public int BitrateKbps;
    public int SampleRateHz;
    public AudioStreamMP3 Stream;
    public bool UseFileName; // We don't have relevant artist information
}