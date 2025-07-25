using System;
using System.Collections.Generic;

namespace BSCore
{
	public class ServerPlayFabConfigService : PlayFabService, IConfigService
	{
		public void Fetch(List<string> keys, Action<Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure)
		{
		}
	}
}
