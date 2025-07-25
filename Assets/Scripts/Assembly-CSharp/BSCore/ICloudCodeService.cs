using System;
using BSCore.Constants.CloudCode;

namespace BSCore
{
	public interface ICloudCodeService
	{
		void Run(FunctionName functionName, object parameters, Action<object> onComplete, Action<FailureReasons> onFailed);

		void Run<T>(FunctionName functionName, object parameters, Action<T> onComplete, Action<FailureReasons> onFailed);
	}
}
