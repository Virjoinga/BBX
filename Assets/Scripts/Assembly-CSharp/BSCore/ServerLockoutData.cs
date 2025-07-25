using System;

namespace BSCore
{
	[Serializable]
	public class ServerLockoutData
	{
		public bool Locked;

		public string Message;
	}
}
