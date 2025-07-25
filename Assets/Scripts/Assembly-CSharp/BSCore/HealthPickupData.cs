using System;

namespace BSCore
{
	[Serializable]
	public class HealthPickupData : BasePickupData
	{
		public uint healing;

		public uint duration;

		public override string ToString()
		{
			return $"{base.ToString()} - healing: {healing} - duration: {duration}";
		}
	}
}
