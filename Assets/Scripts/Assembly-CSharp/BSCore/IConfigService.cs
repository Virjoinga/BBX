using System;
using System.Collections.Generic;

namespace BSCore
{
	public interface IConfigService
	{
		void Fetch(List<string> keys, Action<Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure);
	}
}
