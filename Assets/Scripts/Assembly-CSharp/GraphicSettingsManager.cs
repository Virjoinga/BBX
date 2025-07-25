using System;
using System.Collections.Generic;
using BSCore;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

public class GraphicSettingsManager : IInitializable, IDisposable
{
	[Inject]
	protected GameConfigData _gameConfigData;

	[Inject(Id = DataStoreKeys.Vsync)]
	private DataStoreInt _vSyncDataStore;

	[Inject(Id = DataStoreKeys.Antialiasing)]
	private DataStoreInt _antialiasingDataStore;

	[Inject(Id = DataStoreKeys.Textures)]
	private DataStoreInt _textureDataStore;

	[Inject(Id = DataStoreKeys.Anisotropic)]
	private DataStoreInt _anisotropicDataStore;

	[Inject(Id = DataStoreKeys.Shadows)]
	private DataStoreInt _shadowsDataStore;

	[Inject(Id = DataStoreKeys.FPSCap)]
	private DataStoreFloat _fpsCapDataStore;

	private List<string> _vSyncOptions = new List<string> { "On", "Off" };

	private List<string> _antialiasingOptions = new List<string> { "Disabled", "2x", "4x", "8x" };

	private List<string> _textureOptions = new List<string> { "Low", "Medium", "High" };

	private List<string> _anisotropicOptions = new List<string> { "On", "Off" };

	private List<string> _shadowOptions = new List<string> { "Disabled", "Low", "Medium", "High" };

	private Dictionary<DataStoreKeys, List<string>> _graphicSettingOptionsByKey;

	public void Initialize()
	{
		_graphicSettingOptionsByKey = new Dictionary<DataStoreKeys, List<string>>();
		_graphicSettingOptionsByKey.Add(DataStoreKeys.Vsync, _vSyncOptions);
		_graphicSettingOptionsByKey.Add(DataStoreKeys.Antialiasing, _antialiasingOptions);
		_graphicSettingOptionsByKey.Add(DataStoreKeys.Textures, _textureOptions);
		_graphicSettingOptionsByKey.Add(DataStoreKeys.Anisotropic, _anisotropicOptions);
		_graphicSettingOptionsByKey.Add(DataStoreKeys.Shadows, _shadowOptions);
		_vSyncDataStore.ListenAndInvoke(ApplyVsyncSetting);
		_antialiasingDataStore.ListenAndInvoke(ApplyAntialiasingSetting);
		_textureDataStore.ListenAndInvoke(ApplyTextureSetting);
		_anisotropicDataStore.ListenAndInvoke(ApplyAnisotropicSetting);
		_shadowsDataStore.ListenAndInvoke(ApplyShadowSetting);
		_fpsCapDataStore.ListenAndInvoke(ApplyFPSCapSetting);
	}

	public void Dispose()
	{
		_vSyncDataStore.Unlisten(ApplyVsyncSetting);
		_antialiasingDataStore.Unlisten(ApplyAntialiasingSetting);
		_textureDataStore.Unlisten(ApplyTextureSetting);
		_anisotropicDataStore.Unlisten(ApplyAnisotropicSetting);
		_shadowsDataStore.Unlisten(ApplyShadowSetting);
		_fpsCapDataStore.Unlisten(ApplyFPSCapSetting);
	}

	private void ApplyVsyncSetting(int index)
	{
		QualitySettings.vSyncCount = ((index == 0) ? 1 : 0);
	}

	private void ApplyAntialiasingSetting(int index)
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
		switch (index)
		{
		case 0:
			universalRenderPipelineAsset.msaaSampleCount = 1;
			break;
		case 1:
			universalRenderPipelineAsset.msaaSampleCount = 2;
			break;
		case 2:
			universalRenderPipelineAsset.msaaSampleCount = 4;
			break;
		case 3:
			universalRenderPipelineAsset.msaaSampleCount = 8;
			break;
		default:
			universalRenderPipelineAsset.msaaSampleCount = 2;
			break;
		}
	}

	private void ApplyTextureSetting(int index)
	{
		switch (index)
		{
		case 0:
			QualitySettings.masterTextureLimit = 2;
			break;
		case 1:
			QualitySettings.masterTextureLimit = 1;
			break;
		default:
			QualitySettings.masterTextureLimit = 0;
			break;
		}
	}

	private void ApplyAnisotropicSetting(int index)
	{
		QualitySettings.anisotropicFiltering = ((index == 0) ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable);
	}

	private void ApplyShadowSetting(int index)
	{
		switch (index)
		{
		case 0:
			GraphicsSettings.renderPipelineAsset = _gameConfigData.ShadowsDisabledAsset;
			break;
		case 1:
			GraphicsSettings.renderPipelineAsset = _gameConfigData.ShadowsLowAsset;
			break;
		case 2:
			GraphicsSettings.renderPipelineAsset = _gameConfigData.ShadowsMedAsset;
			break;
		default:
			GraphicsSettings.renderPipelineAsset = _gameConfigData.ShadowsHighAsset;
			break;
		}
		ApplyAntialiasingSetting(_antialiasingDataStore.Value);
	}

	private void ApplyFPSCapSetting(float value)
	{
		Application.targetFrameRate = Mathf.RoundToInt(value);
	}

	public List<string> GetOptions(DataStoreKeys key)
	{
		List<string> result = new List<string>();
		if (_graphicSettingOptionsByKey.ContainsKey(key))
		{
			result = _graphicSettingOptionsByKey[key];
		}
		else
		{
			Debug.LogError($"[GraphicSettingsManager] No Options Setup for Key {key}");
		}
		return result;
	}
}
