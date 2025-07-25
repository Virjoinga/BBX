using System;
using System.Collections.Generic;

namespace BSCore
{
	public interface IGameItemService
	{
		void Fetch(string catalogName, Action<Dictionary<string, BaseProfile>> onSuccess, Action<FailureReasons> onFailure);
	}
}
