using System;

namespace RSG.Exceptions
{
	public class PromiseStateException : PromiseException
	{
		public PromiseStateException()
		{
		}

		public PromiseStateException(string message)
			: base(message)
		{
		}

		public PromiseStateException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
