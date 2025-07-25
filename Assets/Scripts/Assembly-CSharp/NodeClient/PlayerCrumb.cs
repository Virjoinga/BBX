using System;

namespace NodeClient
{
	public class PlayerCrumb : IEquatable<PlayerCrumb>
	{
		public string i;

		public string n;

		public bool a;

		public int s;

		private PlayerStatus _statusCache = PlayerStatus.Offline;

		public string Id => i;

		public string Nickname => n;

		public bool IsAdmin => a;

		public PlayerStatus Status
		{
			get
			{
				if (_statusCache == PlayerStatus.Offline)
				{
					_statusCache = (PlayerStatus)s;
				}
				return _statusCache;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}-{1}{2}", Id, Nickname, IsAdmin ? "-isAdmin" : "");
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj is PlayerCrumb)
			{
				return Equals((PlayerCrumb)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(PlayerCrumb other)
		{
			if ((object)other == null)
			{
				return false;
			}
			if ((object)this == other)
			{
				return true;
			}
			return i.Trim().Equals(other.i.Trim());
		}

		public static bool operator ==(PlayerCrumb lhs, PlayerCrumb rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PlayerCrumb lhs, PlayerCrumb rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
