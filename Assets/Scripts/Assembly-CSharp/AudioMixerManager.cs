using System;
using BSCore;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class AudioMixerManager : IInitializable, IDisposable
{
	private const string MASTER_VOLUME_ID = "MasterVolume";

	private const string MUSIC_VOLUME_ID = "MusicVolume";

	private const string SOUND_EFFECTS_VOLUME_ID = "SoundEffectsVolume";

	private const string UI_VOLUME_ID = "UIVolume";

	private const string VOICES_VOLUME_ID = "VoicesVolume";

	[Inject(Id = DataStoreKeys.MasterVolume)]
	private DataStoreFloat _masterVolumeDataStore;

	[Inject(Id = DataStoreKeys.MusicVolume)]
	private DataStoreFloat _musicVolumeDataStore;

	[Inject(Id = DataStoreKeys.SFXVolume)]
	private DataStoreFloat _soundFXVolumeDataStore;

	[Inject(Id = DataStoreKeys.UIVolume)]
	private DataStoreFloat _UIVolumeDataStore;

	[Inject(Id = DataStoreKeys.VoiceVolume)]
	private DataStoreFloat _voiceVolumeDataStore;

	private AudioMixer _audioMixer;

	public void Initialize()
	{
		_audioMixer = Resources.Load<AudioMixer>("AudioMixer");
		_masterVolumeDataStore.ListenAndInvoke(OnMasterVolumeChanged);
		_musicVolumeDataStore.ListenAndInvoke(OnMusicVolumeChanged);
		_soundFXVolumeDataStore.ListenAndInvoke(OnSFXVolumeChanged);
		_UIVolumeDataStore.ListenAndInvoke(OnUIVolumeChanged);
		_voiceVolumeDataStore.ListenAndInvoke(OnVoicesVolumeChanged);
	}

	public void Dispose()
	{
		_masterVolumeDataStore.Unlisten(OnMasterVolumeChanged);
		_musicVolumeDataStore.Unlisten(OnMusicVolumeChanged);
		_soundFXVolumeDataStore.Unlisten(OnSFXVolumeChanged);
		_UIVolumeDataStore.Unlisten(OnUIVolumeChanged);
		_voiceVolumeDataStore.Unlisten(OnVoicesVolumeChanged);
	}

	private void OnMasterVolumeChanged(float volume)
	{
		_audioMixer.SetFloat("MasterVolume", ConvertValue(volume));
	}

	private void OnMusicVolumeChanged(float volume)
	{
		_audioMixer.SetFloat("MusicVolume", ConvertValue(volume));
	}

	private void OnSFXVolumeChanged(float volume)
	{
		_audioMixer.SetFloat("SoundEffectsVolume", ConvertValue(volume));
	}

	private void OnUIVolumeChanged(float volume)
	{
		_audioMixer.SetFloat("UIVolume", ConvertValue(volume));
	}

	private void OnVoicesVolumeChanged(float volume)
	{
		_audioMixer.SetFloat("VoicesVolume", ConvertValue(volume));
	}

	private float ConvertValue(float volume)
	{
		return Mathf.Log10(volume) * 20f;
	}
}
