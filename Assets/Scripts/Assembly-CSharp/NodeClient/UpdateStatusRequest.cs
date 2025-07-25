using System;

namespace NodeClient
{
	[Serializable]
	public class UpdateStatusRequest
	{
		public int s;

		public UpdateStatusRequest(PlayerStatus status)
		{
			s = (int)status;
		}
	}
}
