using System;

namespace RSG
{
	public class PromiseCancelledException : Exception
	{
		public PromiseCancelledException()
		{
		}

		public PromiseCancelledException(string message)
			: base(message)
		{
		}
	}
}
