using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.Utilities.Collections
{
	public abstract class CollectionUtilities
	{
		public static void AddRange(IList to, IEnumerable range)
		{
			foreach (object item in range)
			{
				to.Add(item);
			}
		}

		public static bool CheckElementsAreOfType(IEnumerable e, Type t)
		{
			foreach (object item in e)
			{
				if (!t.IsInstanceOfType(item))
				{
					return false;
				}
			}
			return true;
		}

		public static IDictionary ReadOnly(IDictionary d)
		{
			return d;
		}

		public static IList ReadOnly(IList l)
		{
			return l;
		}

		public static ISet ReadOnly(ISet s)
		{
			return s;
		}

		public static string ToString(IEnumerable c)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			IEnumerator enumerator = c.GetEnumerator();
			if (enumerator.MoveNext())
			{
				stringBuilder.Append(enumerator.Current.ToString());
				while (enumerator.MoveNext())
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(enumerator.Current.ToString());
				}
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
	}
}
