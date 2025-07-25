using System.Collections.Generic;
using UnityEngine;

namespace BSCore
{
	public class BundleProfile : BaseProfile
	{
		public List<BaseProfile> BundledItems { get; private set; }

		public Color BackgroundColor { get; private set; }

		public uint DLCAppId { get; private set; }

		public BundleProfile(GameItem gameItem, Dictionary<string, BaseProfile> profiles)
			: base(gameItem)
		{
			BundledItems = new List<BaseProfile>();
			foreach (string bundledItem in gameItem.BundledItems)
			{
				if (profiles.TryGetValue(bundledItem, out var value))
				{
					BundledItems.Add(value);
				}
			}
		}

		protected override void DeserializeData(string json)
		{
			_profileData = BaseProfileData.FromJson<BundleProfileData>(json);
		}

		protected override void ParseCustomData()
		{
			base.ParseCustomData();
			BundleProfileData bundleProfileData = _profileData as BundleProfileData;
			BackgroundColor = (ColorUtility.TryParseHtmlString(bundleProfileData.BackgroundHexColor, out var color) ? color : Color.blue);
			DLCAppId = bundleProfileData.DLCAppId;
		}
	}
}
