using System;

namespace BSCore
{
	[Serializable]
	public abstract class BasePickupData
	{
		public string id;

		public uint initialCooldown;

		public uint cooldownLength;

		public override string ToString()
		{
			return $"id: {id} - initialCooldown: {initialCooldown} - cooldownLength: {cooldownLength}";
		}
	}
}
