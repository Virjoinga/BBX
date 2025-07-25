using System.Collections.Generic;

namespace BSCore
{
	public class IAPProfile : BaseProfile
	{
		public Dictionary<CurrencyType, uint> CurrencyBundle { get; protected set; }

		public IAPProfile(GameItem gameItem)
			: base(gameItem)
		{
			CurrencyBundle = gameItem.CurrencyBundle;
		}

		protected override void DeserializeData(string json)
		{
			_profileData = BaseProfileData.FromJson<IAPProfileData>(json);
		}

		protected override void ParseCustomData()
		{
			base.ParseCustomData();
			_ = _profileData;
		}
	}
}
