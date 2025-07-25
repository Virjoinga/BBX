using System;
using System.Collections.Generic;
using PlayFab.Internal;
using PlayFab.Json;

namespace PlayFab
{
	public class PluginManager
	{
		public const string PLUGIN_TRANSPORT_ONEDS = "PLUGIN_TRANSPORT_ONEDS";

		private Dictionary<PluginContractKey, IPlayFabPlugin> plugins = new Dictionary<PluginContractKey, IPlayFabPlugin>(new PluginContractKeyComparator());

		private static readonly PluginManager Instance = new PluginManager();

		private PluginManager()
		{
		}

		public static T GetPlugin<T>(PluginContract contract, string instanceName = "") where T : IPlayFabPlugin
		{
			return (T)Instance.GetPluginInternal(contract, instanceName);
		}

		public static void SetPlugin(IPlayFabPlugin plugin, PluginContract contract, string instanceName = "")
		{
			Instance.SetPluginInternal(plugin, contract, instanceName);
		}

		private IPlayFabPlugin GetPluginInternal(PluginContract contract, string instanceName)
		{
			PluginContractKey key = new PluginContractKey
			{
				_pluginContract = contract,
				_pluginName = instanceName
			};
			if (!plugins.TryGetValue(key, out var value))
			{
				switch (contract)
				{
				case PluginContract.PlayFab_Serializer:
					value = CreatePlugin<SimpleJsonInstance>();
					break;
				case PluginContract.PlayFab_Transport:
					value = ((!(instanceName == "PLUGIN_TRANSPORT_ONEDS")) ? ((IPlayFabPlugin)CreatePlayFabTransportPlugin()) : ((IPlayFabPlugin)CreateOneDSTransportPlugin()));
					break;
				default:
					throw new ArgumentException("This contract is not supported", "contract");
				}
				plugins[key] = value;
			}
			return value;
		}

		private void SetPluginInternal(IPlayFabPlugin plugin, PluginContract contract, string instanceName)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException("plugin", "Plugin instance cannot be null");
			}
			PluginContractKey key = new PluginContractKey
			{
				_pluginContract = contract,
				_pluginName = instanceName
			};
			plugins[key] = plugin;
		}

		private IPlayFabPlugin CreatePlugin<T>() where T : IPlayFabPlugin, new()
		{
			return (IPlayFabPlugin)Activator.CreateInstance(typeof(T));
		}

		private ITransportPlugin CreatePlayFabTransportPlugin()
		{
			ITransportPlugin transportPlugin = null;
			if (PlayFabSettings.RequestType == WebRequestType.HttpWebRequest)
			{
				transportPlugin = new PlayFabWebRequest();
			}
			if (transportPlugin == null)
			{
				transportPlugin = new PlayFabUnityHttp();
			}
			return transportPlugin;
		}

		private IOneDSTransportPlugin CreateOneDSTransportPlugin()
		{
			IOneDSTransportPlugin oneDSTransportPlugin = null;
			if (PlayFabSettings.RequestType == WebRequestType.HttpWebRequest)
			{
				oneDSTransportPlugin = new OneDsWebRequestPlugin();
			}
			if (oneDSTransportPlugin == null)
			{
				oneDSTransportPlugin = new OneDsUnityHttpPlugin();
			}
			return oneDSTransportPlugin;
		}
	}
}
