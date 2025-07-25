using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BSCore
{
	public abstract class BasePlayFabGameItemService : PlayFabService, IGameItemService
	{
		private Regex _releaseVersionRegex = new Regex("ReleaseVersion\":\\s?\"([0-9]+\\.[0-9]+\\.[0-9]+)\"");

		protected BaseProfileFactory _profileFactory;

		private GameConfigData _configData;

		public BasePlayFabGameItemService(BaseProfileFactory profileFactory, GameConfigData configData)
		{
			_profileFactory = profileFactory;
			_configData = configData;
		}

		public abstract void Fetch(string catalogName, Action<Dictionary<string, BaseProfile>> onSuccess, Action<FailureReasons> onFailure);

		protected bool IncludeInCurrentVersion(GameItem gameItem)
		{
			string customData = gameItem.CustomData;
			Match match = _releaseVersionRegex.Match(customData);
			string version = "0.0.0";
			if (match != null && match.Success && match.Groups != null)
			{
				version = match.Groups[1].Value;
			}
			return _configData.GameVersionGreaterThanEqualsVersion(version);
		}

		protected bool IsValidItemType(string itemClass)
		{
			try
			{
				Enum<ItemType>.Parse(itemClass);
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Unrecognized ItemType: {0}", itemClass, ex);
				return false;
			}
			return true;
		}
	}
}
