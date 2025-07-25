using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Extensions;

namespace BestHTTP.Authentication
{
	internal sealed class Digest
	{
		public Uri Uri { get; private set; }

		public AuthenticationTypes Type { get; private set; }

		public string Realm { get; private set; }

		public bool Stale { get; private set; }

		private string Nonce { get; set; }

		private string Opaque { get; set; }

		private string Algorithm { get; set; }

		public List<string> ProtectedUris { get; private set; }

		private string QualityOfProtections { get; set; }

		private int NonceCount { get; set; }

		private string HA1Sess { get; set; }

		internal Digest(Uri uri)
		{
			Uri = uri;
			Algorithm = "md5";
		}

		public void ParseChallange(string header)
		{
			Type = AuthenticationTypes.Unknown;
			Stale = false;
			Opaque = null;
			HA1Sess = null;
			NonceCount = 0;
			QualityOfProtections = null;
			if (ProtectedUris != null)
			{
				ProtectedUris.Clear();
			}
			foreach (HeaderValue value in new WWWAuthenticateHeaderParser(header).Values)
			{
				switch (value.Key)
				{
				case "basic":
					Type = AuthenticationTypes.Basic;
					break;
				case "digest":
					Type = AuthenticationTypes.Digest;
					break;
				case "realm":
					Realm = value.Value;
					break;
				case "domain":
					if (!string.IsNullOrEmpty(value.Value) && value.Value.Length != 0)
					{
						if (ProtectedUris == null)
						{
							ProtectedUris = new List<string>();
						}
						int pos = 0;
						string item = value.Value.Read(ref pos, ' ');
						do
						{
							ProtectedUris.Add(item);
							item = value.Value.Read(ref pos, ' ');
						}
						while (pos < value.Value.Length);
					}
					break;
				case "nonce":
					Nonce = value.Value;
					break;
				case "qop":
					QualityOfProtections = value.Value;
					break;
				case "stale":
					Stale = bool.Parse(value.Value);
					break;
				case "opaque":
					Opaque = value.Value;
					break;
				case "algorithm":
					Algorithm = value.Value;
					break;
				}
			}
		}

		public string GenerateResponseHeader(HTTPRequest request, Credentials credentials)
		{
			try
			{
				switch (Type)
				{
				case AuthenticationTypes.Basic:
					return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credentials.UserName}:{credentials.Password}"));
				case AuthenticationTypes.Digest:
				{
					NonceCount++;
					string empty = string.Empty;
					string text = new Random(request.GetHashCode()).Next(int.MinValue, int.MaxValue).ToString("X8");
					string text2 = NonceCount.ToString("X8");
					string text3 = Algorithm.TrimAndLower();
					if (!(text3 == "md5"))
					{
						if (!(text3 == "md5-sess"))
						{
							return string.Empty;
						}
						if (string.IsNullOrEmpty(HA1Sess))
						{
							HA1Sess = $"{credentials.UserName}:{Realm}:{credentials.Password}:{Nonce}:{text2}".CalculateMD5Hash();
						}
						empty = HA1Sess;
					}
					else
					{
						empty = $"{credentials.UserName}:{Realm}:{credentials.Password}".CalculateMD5Hash();
					}
					string empty2 = string.Empty;
					string text4 = ((QualityOfProtections != null) ? QualityOfProtections.TrimAndLower() : null);
					if (text4 == null)
					{
						string arg = (request.MethodType.ToString().ToUpper() + ":" + request.CurrentUri.GetRequestPathAndQueryURL()).CalculateMD5Hash();
						empty2 = $"{empty}:{Nonce}:{arg}".CalculateMD5Hash();
					}
					else if (text4.Contains("auth-int"))
					{
						text4 = "auth-int";
						byte[] array = request.GetEntityBody();
						if (array == null)
						{
							array = string.Empty.GetASCIIBytes();
						}
						string text5 = $"{request.MethodType.ToString().ToUpper()}:{request.CurrentUri.GetRequestPathAndQueryURL()}:{array.CalculateMD5Hash()}".CalculateMD5Hash();
						empty2 = $"{empty}:{Nonce}:{text2}:{text}:{text4}:{text5}".CalculateMD5Hash();
					}
					else
					{
						if (!text4.Contains("auth"))
						{
							return string.Empty;
						}
						text4 = "auth";
						string text6 = (request.MethodType.ToString().ToUpper() + ":" + request.CurrentUri.GetRequestPathAndQueryURL()).CalculateMD5Hash();
						empty2 = $"{empty}:{Nonce}:{text2}:{text}:{text4}:{text6}".CalculateMD5Hash();
					}
					string text7 = $"Digest username=\"{credentials.UserName}\", realm=\"{Realm}\", nonce=\"{Nonce}\", uri=\"{request.Uri.GetRequestPathAndQueryURL()}\", cnonce=\"{text}\", response=\"{empty2}\"";
					if (text4 != null)
					{
						text7 = text7 + ", qop=\"" + text4 + "\", nc=" + text2;
					}
					if (!string.IsNullOrEmpty(Opaque))
					{
						text7 = text7 + ", opaque=\"" + Opaque + "\"";
					}
					return text7;
				}
				}
			}
			catch
			{
			}
			return string.Empty;
		}

		public bool IsUriProtected(Uri uri)
		{
			if (string.CompareOrdinal(uri.Host, Uri.Host) != 0)
			{
				return false;
			}
			string text = uri.ToString();
			if (ProtectedUris != null && ProtectedUris.Count > 0)
			{
				for (int i = 0; i < ProtectedUris.Count; i++)
				{
					if (text.Contains(ProtectedUris[i]))
					{
						return true;
					}
				}
			}
			return true;
		}
	}
}
