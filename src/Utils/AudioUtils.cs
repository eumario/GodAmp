using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using SpectralFX.Data;
using FileAccess = Godot.FileAccess;
using TagLib;
using File = System.IO.File;

namespace SpectralFX.Utils;

public static class AudioUtils
{
    public const int SpectrumAnalyzerAudioEffectIndex = 0;
    public const int AmplifyAudioEffectIndex = 1;
    public const int Eq10AudioEffectIndex = 2;
    public const int PannerAudioEffectIndex = 3;

    // Currently only supports MP3
    public static List<Track> LoadAllTracksFromDir(string directoryPath)
    {
        var result = new List<Track>();
        var dir = DirAccess.Open(directoryPath);

        if (dir == null)
        {
            GD.PrintErr("Directory not found: " + directoryPath);
            return result;
        }

        dir.ListDirBegin();
        var fileName = dir.GetNext().Replace(".import", "");

        while (!string.IsNullOrEmpty(fileName))
        {
            GD.Print("Checking: ", fileName);
            if (!dir.CurrentIsDir() && fileName.EndsWith(".mp3"))
            {
                var fullPath = Path.Combine(directoryPath, fileName);
                var track = LoadTrack(fullPath);
                if (track != null)
                {
                    result.Add(track);
                }
            }

            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
        return result;
    }

    public static Track LoadTrack(string fullPath)
    {
        try 
        {
            var file = FileAccess.Open(fullPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr("Failed to open file: " + fullPath);
                return null;
            }

            var mp3Stream = new AudioStreamMP3();
            var data = file.GetBuffer((long)file.GetLength());
            mp3Stream.Data = data;
            file.Close();

            // For TagLib, we need to create a temporary file in the user's temp directory
            var tempDir = Path.GetTempPath();
            var tempFile = Path.Combine(tempDir, Path.GetFileName(fullPath));
            
            // Write the data to the temp file
            File.WriteAllBytes(tempFile, data);

            // Read metadata using TagLib#
            var tagFile = TagLib.File.Create(tempFile);
            var tag = tagFile.Tag;
            var props = tagFile.Properties;

            var fileName = Path.GetFileNameWithoutExtension(fullPath);
            var useFileName = string.IsNullOrWhiteSpace(tag.Title);
            var t = new Track
            {
                Name = useFileName ? fileName : tag.Title,
                Artist = tag.FirstPerformer ?? "Unknown",
                Album = tag.Album,
                TrackNumber = (int)tag.Track,
                Duration = (float)props.Duration.TotalSeconds,
                BitrateKbps = props.AudioBitrate,
                SampleRateHz = props.AudioSampleRate,
                Stream = mp3Stream,
                UseFileName = useFileName,
            };

            // Clean up the temporary file
            try 
            {
                File.Delete(tempFile);
            }
            catch 
            {
                // Ignore cleanup errors
            }
            
            GD.Print("Loaded track: " + t.Name);
            return t;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error loading track {fullPath}: {e}");
            return null;
        }
    }

    public static List<Track> LoadTracksFromPathList(string[] pathList)
    {
        return pathList.Where(p => p != null).Select(LoadTrack).Where(t => t != null).ToList();
    }

    public static string GetFullTrackTitle(Track track)
    {
        if (track == null) return "Unknown Track";
        return track.UseFileName ? track.Name : $"{track.TrackNumber}. {track.Artist} - {track.Name}";
    }
}