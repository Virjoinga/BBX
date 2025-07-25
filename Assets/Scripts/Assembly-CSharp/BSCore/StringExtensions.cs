using System;
using System.Security.Cryptography;
using System.Text;

namespace BSCore
{
	public static class StringExtensions
	{
		public static string MD5Sum(this string strToEncrypt)
		{
			byte[] bytes = new UTF8Encoding().GetBytes(strToEncrypt);
			byte[] array = new MD5CryptoServiceProvider().ComputeHash(bytes);
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			return text.PadLeft(32, '0');
		}
	}
}
