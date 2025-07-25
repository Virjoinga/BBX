using System;

namespace MatchMaking
{
	[Serializable]
	public class BaseResponse<T>
	{
		public bool error;

		public string message;

		public T data;
	}
}
