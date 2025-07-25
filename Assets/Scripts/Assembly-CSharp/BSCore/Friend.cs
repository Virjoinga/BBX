using System.Collections.Generic;
using NodeClient;

namespace BSCore
{
	public struct Friend
	{
		public string ServiceId { get; private set; }

		public string DisplayName { get; private set; }

		public PlayerStatus Status { get; set; }

		public bool IsValid => !string.IsNullOrEmpty(ServiceId);

		public bool IsRequested { get; private set; }

		public bool IsPendingConfirmation { get; private set; }

		public bool IsConfirmed { get; private set; }

		public bool IsSteam { get; private set; }

		public Friend(string serviceId, string displayName, List<string> tags)
		{
			ServiceId = serviceId;
			DisplayName = displayName;
			Status = PlayerStatus.Offline;
			IsRequested = tags.Contains("requested");
			IsPendingConfirmation = tags.Contains("pending");
			IsConfirmed = !IsPendingConfirmation && !IsRequested;
			IsSteam = tags.Contains("steam");
		}

		public override string ToString()
		{
			return $"{ServiceId}:{DisplayName}-IsOnline?{Status}-IsRequested?{IsRequested}-IsAwaitingConfirmation?{IsPendingConfirmation}";
		}

		public override int GetHashCode()
		{
			return ServiceId.GetHashCode();
		}

		public override bool Equals(object b)
		{
			if (b == null && string.IsNullOrEmpty(ServiceId))
			{
				return true;
			}
			if (!(b is Friend))
			{
				return false;
			}
			return ServiceId == ((Friend)b).ServiceId;
		}

		public static bool operator ==(Friend f, object b)
		{
			return f.Equals(b);
		}

		public static bool operator !=(Friend f, object b)
		{
			return !f.Equals(b);
		}
	}
}
