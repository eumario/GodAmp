using System.Collections.Generic;
using System.Linq;
using Godot;
using SpectralFX.Components;
using SpectralFX.Data;
using SpectralFX.Player;

namespace SpectralFX.Controls;

public partial class Controls : VBoxContainer
{
	private MasterPanel.MasterPanel _masterPanel;
	private Equalizer.Equalizer _equalizer;
	private Playlist.Playlist _playlist;
	
	public override void _Ready()
	{
		_masterPanel = GetNode<MasterPanel.MasterPanel>("%MasterPanel");
		_equalizer = GetNode<Equalizer.Equalizer>("%Equalizer");
		_playlist = GetNode<Playlist.Playlist>("%Playlist");

		_masterPanel.ToggleEqualizerRequested += OnToggleEqualizerRequested;
		_masterPanel.TogglePlaylistRequested += OnTogglePlaylistRequested;
	}
	
	public void Setup(TrackPlayer trackPlayerRef, List<Track> playlistRef)
	{
		_masterPanel.Setup(trackPlayerRef);
		_playlist.Setup(trackPlayerRef, playlistRef);
	}
	
	public void SetMasterLabelText(string text)
	{
		_masterPanel.SetMasterLabelText(text);
	}

	public void Refresh()
	{
		_masterPanel.Refresh();
		_playlist.Refresh();
	}

	public void OnToggleEqualizerRequested()
	{
		_equalizer.Visible = !_equalizer.Visible;
	}

	public void OnTogglePlaylistRequested()
	{
		_playlist.Visible = !_playlist.Visible;
	}

	public void OnEqualizerCloseButtonClicked()
	{
		_masterPanel.ToggleEqualizerButton.ButtonPressed = false;
	}

	public void OnPlaylistCloseButtonClicked()
	{
		_masterPanel.TogglePlaylistButton.ButtonPressed = false;
	}
}