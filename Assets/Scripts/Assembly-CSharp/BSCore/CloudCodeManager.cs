using System;
using BSCore.Constants.CloudCode;
using Zenject;

namespace BSCore
{
	public class CloudCodeManager
	{
		private ICloudCodeService _cloudCodeService;

		[Inject]
		public CloudCodeManager(ICloudCodeService cloudCodeService)
		{
			_cloudCodeService = cloudCodeService;
		}

		public void Run(FunctionName functionName, object parameters, Action<object> onComplete, Action<FailureReasons> onFailed)
		{
			_cloudCodeService.Run(functionName, parameters, onComplete, onFailed);
		}

		public void Run<T>(FunctionName functionName, object parameters, Action<T> onComplete, Action<FailureReasons> onFailed)
		{
			_cloudCodeService.Run(functionName, parameters, onComplete, onFailed);
		}
	}
}
