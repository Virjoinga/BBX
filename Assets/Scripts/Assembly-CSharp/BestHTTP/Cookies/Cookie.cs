using System;
using System.Collections.Generic;
using System.IO;
using BestHTTP.Extensions;

namespace BestHTTP.Cookies
{
	public sealed class Cookie : IComparable<Cookie>, IEquatable<Cookie>
	{
		private const int Version = 1;

		public string Name { get; private set; }

		public string Value { get; private set; }

		public DateTime Date { get; internal set; }

		public DateTime LastAccess { get; set; }

		public DateTime Expires { get; private set; }

		public long MaxAge { get; private set; }

		public bool IsSession { get; private set; }

		public string Domain { get; private set; }

		public string Path { get; private set; }

		public bool IsSecure { get; private set; }

		public bool IsHttpOnly { get; private set; }

		public Cookie(string name, string value)
			: this(name, value, "/", string.Empty)
		{
		}

		public Cookie(string name, string value, string path)
			: this(name, value, path, string.Empty)
		{
		}

		public Cookie(string name, string value, string path, string domain)
			: this()
		{
			Name = name;
			Value = value;
			Path = path;
			Domain = domain;
		}

		public Cookie(Uri uri, string name, string value, DateTime expires, bool isSession = true)
			: this(name, value, uri.AbsolutePath, uri.Host)
		{
			Expires = expires;
			IsSession = isSession;
			Date = DateTime.UtcNow;
		}

		public Cookie(Uri uri, string name, string value, long maxAge = -1L, bool isSession = true)
			: this(name, value, uri.AbsolutePath, uri.Host)
		{
			MaxAge = maxAge;
			IsSession = isSession;
			Date = DateTime.UtcNow;
		}

		internal Cookie()
		{
			IsSession = true;
			MaxAge = -1L;
			LastAccess = DateTime.UtcNow;
		}

		public bool WillExpireInTheFuture()
		{
			if (IsSession)
			{
				return true;
			}
			if (MaxAge == -1)
			{
				return Expires > DateTime.UtcNow;
			}
			return Math.Max(0L, (long)(DateTime.UtcNow - Date).TotalSeconds) < MaxAge;
		}

		public uint GuessSize()
		{
			return (uint)(((Name != null) ? (Name.Length * 2) : 0) + ((Value != null) ? (Value.Length * 2) : 0) + ((Domain != null) ? (Domain.Length * 2) : 0) + ((Path != null) ? (Path.Length * 2) : 0) + 32 + 3);
		}

		public static Cookie Parse(string header, Uri defaultDomain)
		{
			Cookie cookie = new Cookie();
			try
			{
				foreach (HeaderValue item in ParseCookieHeader(header))
				{
					switch (item.Key.ToLowerInvariant())
					{
					case "path":
					{
						object path;
						if (!string.IsNullOrEmpty(item.Value) && item.Value.StartsWith("/"))
						{
							string text = (cookie.Path = item.Value);
							path = text;
						}
						else
						{
							path = "/";
						}
						cookie.Path = (string)path;
						break;
					}
					case "domain":
						if (string.IsNullOrEmpty(item.Value))
						{
							return null;
						}
						cookie.Domain = (item.Value.StartsWith(".") ? item.Value.Substring(1) : item.Value);
						break;
					case "expires":
						cookie.Expires = item.Value.ToDateTime(DateTime.FromBinary(0L));
						cookie.IsSession = false;
						break;
					case "max-age":
						cookie.MaxAge = item.Value.ToInt64(-1L);
						cookie.IsSession = false;
						break;
					case "secure":
						cookie.IsSecure = true;
						break;
					case "httponly":
						cookie.IsHttpOnly = true;
						break;
					default:
						cookie.Name = item.Key;
						cookie.Value = item.Value;
						break;
					}
				}
				if (HTTPManager.EnablePrivateBrowsing)
				{
					cookie.IsSession = true;
				}
				if (string.IsNullOrEmpty(cookie.Domain))
				{
					cookie.Domain = defaultDomain.Host;
				}
				if (string.IsNullOrEmpty(cookie.Path))
				{
					cookie.Path = defaultDomain.AbsolutePath;
				}
				DateTime date = (cookie.LastAccess = DateTime.UtcNow);
				cookie.Date = date;
			}
			catch
			{
			}
			return cookie;
		}

		internal void SaveTo(BinaryWriter stream)
		{
			stream.Write(1);
			stream.Write(Name ?? string.Empty);
			stream.Write(Value ?? string.Empty);
			stream.Write(Date.ToBinary());
			stream.Write(LastAccess.ToBinary());
			stream.Write(Expires.ToBinary());
			stream.Write(MaxAge);
			stream.Write(IsSession);
			stream.Write(Domain ?? string.Empty);
			stream.Write(Path ?? string.Empty);
			stream.Write(IsSecure);
			stream.Write(IsHttpOnly);
		}

		internal void LoadFrom(BinaryReader stream)
		{
			stream.ReadInt32();
			Name = stream.ReadString();
			Value = stream.ReadString();
			Date = DateTime.FromBinary(stream.ReadInt64());
			LastAccess = DateTime.FromBinary(stream.ReadInt64());
			Expires = DateTime.FromBinary(stream.ReadInt64());
			MaxAge = stream.ReadInt64();
			IsSession = stream.ReadBoolean();
			Domain = stream.ReadString();
			Path = stream.ReadString();
			IsSecure = stream.ReadBoolean();
			IsHttpOnly = stream.ReadBoolean();
		}

		public override string ToString()
		{
			return Name + "=" + Value;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return Equals(obj as Cookie);
		}

		public bool Equals(Cookie cookie)
		{
			if (cookie == null)
			{
				return false;
			}
			if (this == cookie)
			{
				return true;
			}
			if (Name.Equals(cookie.Name, StringComparison.Ordinal) && ((Domain == null && cookie.Domain == null) || Domain.Equals(cookie.Domain, StringComparison.Ordinal)))
			{
				if (Path != null || cookie.Path != null)
				{
					return Path.Equals(cookie.Path, StringComparison.Ordinal);
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		private static string ReadValue(string str, ref int pos)
		{
			string empty = string.Empty;
			if (str == null)
			{
				return empty;
			}
			return str.Read(ref pos, ';');
		}

		private static List<HeaderValue> ParseCookieHeader(string str)
		{
			List<HeaderValue> list = new List<HeaderValue>();
			if (str == null)
			{
				return list;
			}
			int pos = 0;
			while (pos < str.Length)
			{
				HeaderValue headerValue = new HeaderValue(str.Read(ref pos, (char ch) => ch != '=' && ch != ';').Trim());
				if (pos < str.Length && str[pos - 1] == '=')
				{
					headerValue.Value = ReadValue(str, ref pos);
				}
				list.Add(headerValue);
			}
			return list;
		}

		public int CompareTo(Cookie other)
		{
			return LastAccess.CompareTo(other.LastAccess);
		}
	}
}
