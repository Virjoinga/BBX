using System;

namespace RSG
{
	public struct ProgressHandler
	{
		public Action<float> callback;

		public IRejectable rejectable;
	}
}
