using System;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Math.Raw
{
	internal abstract class Nat
	{
		private const ulong M = 4294967295uL;

		public static uint Add(int len, uint[] x, uint[] y, uint[] z)
		{
			ulong num = 0uL;
			for (int i = 0; i < len; i++)
			{
				num += (ulong)((long)x[i] + (long)y[i]);
				z[i] = (uint)num;
				num >>= 32;
			}
			return (uint)num;
		}

		public static uint Add33At(int len, uint x, uint[] z, int zPos)
		{
			ulong num = (ulong)z[zPos] + (ulong)x;
			z[zPos] = (uint)num;
			num >>= 32;
			num += (ulong)((long)z[zPos + 1] + 1L);
			z[zPos + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zPos + 2);
			}
			return 0u;
		}

		public static uint Add33At(int len, uint x, uint[] z, int zOff, int zPos)
		{
			ulong num = (ulong)z[zOff + zPos] + (ulong)x;
			z[zOff + zPos] = (uint)num;
			num >>= 32;
			num += (ulong)((long)z[zOff + zPos + 1] + 1L);
			z[zOff + zPos + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zOff, zPos + 2);
			}
			return 0u;
		}

		public static uint Add33To(int len, uint x, uint[] z)
		{
			ulong num = (ulong)z[0] + (ulong)x;
			z[0] = (uint)num;
			num >>= 32;
			num += (ulong)((long)z[1] + 1L);
			z[1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, 2);
			}
			return 0u;
		}

		public static uint Add33To(int len, uint x, uint[] z, int zOff)
		{
			ulong num = (ulong)z[zOff] + (ulong)x;
			z[zOff] = (uint)num;
			num >>= 32;
			num += (ulong)((long)z[zOff + 1] + 1L);
			z[zOff + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zOff, 2);
			}
			return 0u;
		}

		public static uint AddBothTo(int len, uint[] x, uint[] y, uint[] z)
		{
			ulong num = 0uL;
			for (int i = 0; i < len; i++)
			{
				num += (ulong)((long)x[i] + (long)y[i] + z[i]);
				z[i] = (uint)num;
				num >>= 32;
			}
			return (uint)num;
		}

		public static uint AddBothTo(int len, uint[] x, int xOff, uint[] y, int yOff, uint[] z, int zOff)
		{
			ulong num = 0uL;
			for (int i = 0; i < len; i++)
			{
				num += (ulong)((long)x[xOff + i] + (long)y[yOff + i] + z[zOff + i]);
				z[zOff + i] = (uint)num;
				num >>= 32;
			}
			return (uint)num;
		}

		public static uint AddDWordAt(int len, ulong x, uint[] z, int zPos)
		{
			ulong num = z[zPos] + (x & 0xFFFFFFFFu);
			z[zPos] = (uint)num;
			num >>= 32;
			num += z[zPos + 1] + (x >> 32);
			z[zPos + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zPos + 2);
			}
			return 0u;
		}

		public static uint AddDWordAt(int len, ulong x, uint[] z, int zOff, int zPos)
		{
			ulong num = z[zOff + zPos] + (x & 0xFFFFFFFFu);
			z[zOff + zPos] = (uint)num;
			num >>= 32;
			num += z[zOff + zPos + 1] + (x >> 32);
			z[zOff + zPos + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zOff, zPos + 2);
			}
			return 0u;
		}

		public static uint AddDWordTo(int len, ulong x, uint[] z)
		{
			ulong num = z[0] + (x & 0xFFFFFFFFu);
			z[0] = (uint)num;
			num >>= 32;
			num += z[1] + (x >> 32);
			z[1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, 2);
			}
			return 0u;
		}

		public static uint AddDWordTo(int len, ulong x, uint[] z, int zOff)
		{
			ulong num = z[zOff] + (x & 0xFFFFFFFFu);
			z[zOff] = (uint)num;
			num >>= 32;
			num += z[zOff + 1] + (x >> 32);
			z[zOff + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zOff, 2);
			}
			return 0u;
		}

		public static uint AddTo(int len, uint[] x, uint[] z)
		{
			ulong num = 0uL;
			for (int i = 0; i < len; i++)
			{
				num += (ulong)((long)x[i] + (long)z[i]);
				z[i] = (uint)num;
				num >>= 32;
			}
			return (uint)num;
		}

		public static uint AddTo(int len, uint[] x, int xOff, uint[] z, int zOff)
		{
			ulong num = 0uL;
			for (int i = 0; i < len; i++)
			{
				num += (ulong)((long)x[xOff + i] + (long)z[zOff + i]);
				z[zOff + i] = (uint)num;
				num >>= 32;
			}
			return (uint)num;
		}

		public static uint AddWordAt(int len, uint x, uint[] z, int zPos)
		{
			ulong num = (ulong)x + (ulong)z[zPos];
			z[zPos] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zPos + 1);
			}
			return 0u;
		}

		public static uint AddWordAt(int len, uint x, uint[] z, int zOff, int zPos)
		{
			ulong num = (ulong)x + (ulong)z[zOff + zPos];
			z[zOff + zPos] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zOff, zPos + 1);
			}
			return 0u;
		}

		public static uint AddWordTo(int len, uint x, uint[] z)
		{
			ulong num = (ulong)x + (ulong)z[0];
			z[0] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, 1);
			}
			return 0u;
		}

		public static uint AddWordTo(int len, uint x, uint[] z, int zOff)
		{
			ulong num = (ulong)x + (ulong)z[zOff];
			z[zOff] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zOff, 1);
			}
			return 0u;
		}

		public static void Copy(int len, uint[] x, uint[] z)
		{
			Array.Copy(x, 0, z, 0, len);
		}

		public static uint[] Copy(int len, uint[] x)
		{
			uint[] array = new uint[len];
			Array.Copy(x, 0, array, 0, len);
			return array;
		}

		public static uint[] Create(int len)
		{
			return new uint[len];
		}

		public static ulong[] Create64(int len)
		{
			return new ulong[len];
		}

		public static int Dec(int len, uint[] z)
		{
			for (int i = 0; i < len; i++)
			{
				if (--z[i] != uint.MaxValue)
				{
					return 0;
				}
			}
			return -1;
		}

		public static int Dec(int len, uint[] x, uint[] z)
		{
			int i = 0;
			while (i < len)
			{
				uint num = (z[i] = x[i] - 1);
				i++;
				if (num != uint.MaxValue)
				{
					for (; i < len; i++)
					{
						z[i] = x[i];
					}
					return 0;
				}
			}
			return -1;
		}

		public static int DecAt(int len, uint[] z, int zPos)
		{
			for (int i = zPos; i < len; i++)
			{
				if (--z[i] != uint.MaxValue)
				{
					return 0;
				}
			}
			return -1;
		}

		public static int DecAt(int len, uint[] z, int zOff, int zPos)
		{
			for (int i = zPos; i < len; i++)
			{
				if (--z[zOff + i] != uint.MaxValue)
				{
					return 0;
				}
			}
			return -1;
		}

		public static bool Eq(int len, uint[] x, uint[] y)
		{
			for (int num = len - 1; num >= 0; num--)
			{
				if (x[num] != y[num])
				{
					return false;
				}
			}
			return true;
		}

		public static uint[] FromBigInteger(int bits, BigInteger x)
		{
			if (x.SignValue < 0 || x.BitLength > bits)
			{
				throw new ArgumentException();
			}
			uint[] array = Create(bits + 31 >> 5);
			int num = 0;
			while (x.SignValue != 0)
			{
				array[num++] = (uint)x.IntValue;
				x = x.ShiftRight(32);
			}
			return array;
		}

		public static uint GetBit(uint[] x, int bit)
		{
			if (bit == 0)
			{
				return x[0] & 1;
			}
			int num = bit >> 5;
			if (num < 0 || num >= x.Length)
			{
				return 0u;
			}
			int num2 = bit & 0x1F;
			return (x[num] >> num2) & 1;
		}

		public static bool Gte(int len, uint[] x, uint[] y)
		{
			for (int num = len - 1; num >= 0; num--)
			{
				uint num2 = x[num];
				uint num3 = y[num];
				if (num2 < num3)
				{
					return false;
				}
				if (num2 > num3)
				{
					return true;
				}
			}
			return true;
		}

		public static uint Inc(int len, uint[] z)
		{
			for (int i = 0; i < len; i++)
			{
				if (++z[i] != 0)
				{
					return 0u;
				}
			}
			return 1u;
		}

		public static uint Inc(int len, uint[] x, uint[] z)
		{
			int i = 0;
			while (i < len)
			{
				uint num = (z[i] = x[i] + 1);
				i++;
				if (num != 0)
				{
					for (; i < len; i++)
					{
						z[i] = x[i];
					}
					return 0u;
				}
			}
			return 1u;
		}

		public static uint IncAt(int len, uint[] z, int zPos)
		{
			for (int i = zPos; i < len; i++)
			{
				if (++z[i] != 0)
				{
					return 0u;
				}
			}
			return 1u;
		}

		public static uint IncAt(int len, uint[] z, int zOff, int zPos)
		{
			for (int i = zPos; i < len; i++)
			{
				if (++z[zOff + i] != 0)
				{
					return 0u;
				}
			}
			return 1u;
		}

		public static bool IsOne(int len, uint[] x)
		{
			if (x[0] != 1)
			{
				return false;
			}
			for (int i = 1; i < len; i++)
			{
				if (x[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsZero(int len, uint[] x)
		{
			if (x[0] != 0)
			{
				return false;
			}
			for (int i = 1; i < len; i++)
			{
				if (x[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static void Mul(int len, uint[] x, uint[] y, uint[] zz)
		{
			zz[len] = MulWord(len, x[0], y, zz);
			for (int i = 1; i < len; i++)
			{
				zz[i + len] = MulWordAddTo(len, x[i], y, 0, zz, i);
			}
		}

		public static void Mul(int len, uint[] x, int xOff, uint[] y, int yOff, uint[] zz, int zzOff)
		{
			zz[zzOff + len] = MulWord(len, x[xOff], y, yOff, zz, zzOff);
			for (int i = 1; i < len; i++)
			{
				zz[zzOff + i + len] = MulWordAddTo(len, x[xOff + i], y, yOff, zz, zzOff + i);
			}
		}

		public static uint Mul31BothAdd(int len, uint a, uint[] x, uint b, uint[] y, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = a;
			ulong num3 = b;
			int num4 = 0;
			do
			{
				num += num2 * x[num4] + num3 * y[num4] + z[zOff + num4];
				z[zOff + num4] = (uint)num;
				num >>= 32;
			}
			while (++num4 < len);
			return (uint)num;
		}

		public static uint MulWord(int len, uint x, uint[] y, uint[] z)
		{
			ulong num = 0uL;
			ulong num2 = x;
			int num3 = 0;
			do
			{
				num += num2 * y[num3];
				z[num3] = (uint)num;
				num >>= 32;
			}
			while (++num3 < len);
			return (uint)num;
		}

		public static uint MulWord(int len, uint x, uint[] y, int yOff, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = x;
			int num3 = 0;
			do
			{
				num += num2 * y[yOff + num3];
				z[zOff + num3] = (uint)num;
				num >>= 32;
			}
			while (++num3 < len);
			return (uint)num;
		}

		public static uint MulWordAddTo(int len, uint x, uint[] y, int yOff, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = x;
			int num3 = 0;
			do
			{
				num += num2 * y[yOff + num3] + z[zOff + num3];
				z[zOff + num3] = (uint)num;
				num >>= 32;
			}
			while (++num3 < len);
			return (uint)num;
		}

		public static uint MulWordDwordAddAt(int len, uint x, ulong y, uint[] z, int zPos)
		{
			ulong num = 0uL;
			ulong num2 = x;
			num += num2 * (uint)y + z[zPos];
			z[zPos] = (uint)num;
			num >>= 32;
			num += num2 * (y >> 32) + z[zPos + 1];
			z[zPos + 1] = (uint)num;
			num >>= 32;
			num += z[zPos + 2];
			z[zPos + 2] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return IncAt(len, z, zPos + 3);
			}
			return 0u;
		}

		public static uint ShiftDownBit(int len, uint[] z, uint c)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = z[num];
				z[num] = (num2 >> 1) | (c << 31);
				c = num2;
			}
			return c << 31;
		}

		public static uint ShiftDownBit(int len, uint[] z, int zOff, uint c)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = z[zOff + num];
				z[zOff + num] = (num2 >> 1) | (c << 31);
				c = num2;
			}
			return c << 31;
		}

		public static uint ShiftDownBit(int len, uint[] x, uint c, uint[] z)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = x[num];
				z[num] = (num2 >> 1) | (c << 31);
				c = num2;
			}
			return c << 31;
		}

		public static uint ShiftDownBit(int len, uint[] x, int xOff, uint c, uint[] z, int zOff)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = x[xOff + num];
				z[zOff + num] = (num2 >> 1) | (c << 31);
				c = num2;
			}
			return c << 31;
		}

		public static uint ShiftDownBits(int len, uint[] z, int bits, uint c)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = z[num];
				z[num] = (num2 >> bits) | (c << -bits);
				c = num2;
			}
			return c << -bits;
		}

		public static uint ShiftDownBits(int len, uint[] z, int zOff, int bits, uint c)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = z[zOff + num];
				z[zOff + num] = (num2 >> bits) | (c << -bits);
				c = num2;
			}
			return c << -bits;
		}

		public static uint ShiftDownBits(int len, uint[] x, int bits, uint c, uint[] z)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = x[num];
				z[num] = (num2 >> bits) | (c << -bits);
				c = num2;
			}
			return c << -bits;
		}

		public static uint ShiftDownBits(int len, uint[] x, int xOff, int bits, uint c, uint[] z, int zOff)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = x[xOff + num];
				z[zOff + num] = (num2 >> bits) | (c << -bits);
				c = num2;
			}
			return c << -bits;
		}

		public static uint ShiftDownWord(int len, uint[] z, uint c)
		{
			int num = len;
			while (--num >= 0)
			{
				uint num2 = z[num];
				z[num] = c;
				c = num2;
			}
			return c;
		}

		public static uint ShiftUpBit(int len, uint[] z, uint c)
		{
			for (int i = 0; i < len; i++)
			{
				uint num = z[i];
				z[i] = (num << 1) | (c >> 31);
				c = num;
			}
			return c >> 31;
		}

		public static uint ShiftUpBit(int len, uint[] z, int zOff, uint c)
		{
			for (int i = 0; i < len; i++)
			{
				uint num = z[zOff + i];
				z[zOff + i] = (num << 1) | (c >> 31);
				c = num;
			}
			return c >> 31;
		}

		public static uint ShiftUpBit(int len, uint[] x, uint c, uint[] z)
		{
			for (int i = 0; i < len; i++)
			{
				uint num = x[i];
				z[i] = (num << 1) | (c >> 31);
				c = num;
			}
			return c >> 31;
		}

		public static uint ShiftUpBit(int len, uint[] x, int xOff, uint c, uint[] z, int zOff)
		{
			for (int i = 0; i < len; i++)
			{
				uint num = x[xOff + i];
				z[zOff + i] = (num << 1) | (c >> 31);
				c = num;
			}
			return c >> 31;
		}

		public static ulong ShiftUpBit64(int len, ulong[] x, int xOff, ulong c, ulong[] z, int zOff)
		{
			for (int i = 0; i < len; i++)
			{
				ulong num = x[xOff + i];
				z[zOff + i] = (num << 1) | (c >> 63);
				c = num;
			}
			return c >> 63;
		}

		public static uint ShiftUpBits(int len, uint[] z, int bits, uint c)
		{
			for (int i = 0; i < len; i++)
			{
				uint num = z[i];
				z[i] = (num << bits) | (c >> -bits);
				c = num;
			}
			return c >> -bits;
		}

		public static uint ShiftUpBits(int len, uint[] z, int zOff, int bits, uint c)
		{
			for (int i = 0; i < len; i++)
			{
				uint num = z[zOff + i];
				z[zOff + i] = (num << bits) | (c >> -bits);
				c = num;
			}
			return c >> -bits;
		}

		public static ulong ShiftUpBits64(int len, ulong[] z, int zOff, int bits, ulong c)
		{
			for (int i = 0; i < len; i++)
			{
				ulong num = z[zOff + i];
				z[zOff + i] = (num << bits) | (c >> -bits);
				c = num;
			}
			return c >> -bits;
		}

		public static uint ShiftUpBits(int len, uint[] x, int bits, uint c, uint[] z)
		{
			for (int i = 0; i < len; i++)
			{
				uint num = x[i];
				z[i] = (num << bits) | (c >> -bits);
				c = num;
			}
			return c >> -bits;
		}

		public static uint ShiftUpBits(int len, uint[] x, int xOff, int bits, uint c, uint[] z, int zOff)
		{
			for (int i = 0; i < len; i++)
			{
				uint num = x[xOff + i];
				z[zOff + i] = (num << bits) | (c >> -bits);
				c = num;
			}
			return c >> -bits;
		}

		public static ulong ShiftUpBits64(int len, ulong[] x, int xOff, int bits, ulong c, ulong[] z, int zOff)
		{
			for (int i = 0; i < len; i++)
			{
				ulong num = x[xOff + i];
				z[zOff + i] = (num << bits) | (c >> -bits);
				c = num;
			}
			return c >> -bits;
		}

		public static void Square(int len, uint[] x, uint[] zz)
		{
			int num = len << 1;
			uint num2 = 0u;
			int num3 = len;
			int num4 = num;
			do
			{
				long num5 = x[--num3];
				ulong num6 = (ulong)(num5 * num5);
				zz[--num4] = (num2 << 31) | (uint)(int)(num6 >> 33);
				zz[--num4] = (uint)(num6 >> 1);
				num2 = (uint)num6;
			}
			while (num3 > 0);
			for (int i = 1; i < len; i++)
			{
				num2 = SquareWordAdd(x, i, zz);
				AddWordAt(num, num2, zz, i << 1);
			}
			ShiftUpBit(num, zz, x[0] << 31);
		}

		public static void Square(int len, uint[] x, int xOff, uint[] zz, int zzOff)
		{
			int num = len << 1;
			uint num2 = 0u;
			int num3 = len;
			int num4 = num;
			do
			{
				long num5 = x[xOff + --num3];
				ulong num6 = (ulong)(num5 * num5);
				zz[zzOff + --num4] = (num2 << 31) | (uint)(int)(num6 >> 33);
				zz[zzOff + --num4] = (uint)(num6 >> 1);
				num2 = (uint)num6;
			}
			while (num3 > 0);
			for (int i = 1; i < len; i++)
			{
				num2 = SquareWordAdd(x, xOff, i, zz, zzOff);
				AddWordAt(num, num2, zz, zzOff, i << 1);
			}
			ShiftUpBit(num, zz, zzOff, x[xOff] << 31);
		}

		public static uint SquareWordAdd(uint[] x, int xPos, uint[] z)
		{
			ulong num = 0uL;
			ulong num2 = x[xPos];
			int num3 = 0;
			do
			{
				num += num2 * x[num3] + z[xPos + num3];
				z[xPos + num3] = (uint)num;
				num >>= 32;
			}
			while (++num3 < xPos);
			return (uint)num;
		}

		public static uint SquareWordAdd(uint[] x, int xOff, int xPos, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = x[xOff + xPos];
			int num3 = 0;
			do
			{
				num += num2 * ((ulong)x[xOff + num3] & 0xFFFFFFFFuL) + ((ulong)z[xPos + zOff] & 0xFFFFFFFFuL);
				z[xPos + zOff] = (uint)num;
				num >>= 32;
				zOff++;
			}
			while (++num3 < xPos);
			return (uint)num;
		}

		public static int Sub(int len, uint[] x, uint[] y, uint[] z)
		{
			long num = 0L;
			for (int i = 0; i < len; i++)
			{
				num += (long)x[i] - (long)y[i];
				z[i] = (uint)num;
				num >>= 32;
			}
			return (int)num;
		}

		public static int Sub(int len, uint[] x, int xOff, uint[] y, int yOff, uint[] z, int zOff)
		{
			long num = 0L;
			for (int i = 0; i < len; i++)
			{
				num += (long)x[xOff + i] - (long)y[yOff + i];
				z[zOff + i] = (uint)num;
				num >>= 32;
			}
			return (int)num;
		}

		public static int Sub33At(int len, uint x, uint[] z, int zPos)
		{
			long num = (long)z[zPos] - (long)x;
			z[zPos] = (uint)num;
			num >>= 32;
			num += (long)z[zPos + 1] - 1L;
			z[zPos + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zPos + 2);
			}
			return 0;
		}

		public static int Sub33At(int len, uint x, uint[] z, int zOff, int zPos)
		{
			long num = (long)z[zOff + zPos] - (long)x;
			z[zOff + zPos] = (uint)num;
			num >>= 32;
			num += (long)z[zOff + zPos + 1] - 1L;
			z[zOff + zPos + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zOff, zPos + 2);
			}
			return 0;
		}

		public static int Sub33From(int len, uint x, uint[] z)
		{
			long num = (long)z[0] - (long)x;
			z[0] = (uint)num;
			num >>= 32;
			num += (long)z[1] - 1L;
			z[1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, 2);
			}
			return 0;
		}

		public static int Sub33From(int len, uint x, uint[] z, int zOff)
		{
			long num = (long)z[zOff] - (long)x;
			z[zOff] = (uint)num;
			num >>= 32;
			num += (long)z[zOff + 1] - 1L;
			z[zOff + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zOff, 2);
			}
			return 0;
		}

		public static int SubBothFrom(int len, uint[] x, uint[] y, uint[] z)
		{
			long num = 0L;
			for (int i = 0; i < len; i++)
			{
				num += (long)z[i] - (long)x[i] - y[i];
				z[i] = (uint)num;
				num >>= 32;
			}
			return (int)num;
		}

		public static int SubBothFrom(int len, uint[] x, int xOff, uint[] y, int yOff, uint[] z, int zOff)
		{
			long num = 0L;
			for (int i = 0; i < len; i++)
			{
				num += (long)z[zOff + i] - (long)x[xOff + i] - y[yOff + i];
				z[zOff + i] = (uint)num;
				num >>= 32;
			}
			return (int)num;
		}

		public static int SubDWordAt(int len, ulong x, uint[] z, int zPos)
		{
			long num = (long)(z[zPos] - (x & 0xFFFFFFFFu));
			z[zPos] = (uint)num;
			num >>= 32;
			num += (long)(z[zPos + 1] - (x >> 32));
			z[zPos + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zPos + 2);
			}
			return 0;
		}

		public static int SubDWordAt(int len, ulong x, uint[] z, int zOff, int zPos)
		{
			long num = (long)(z[zOff + zPos] - (x & 0xFFFFFFFFu));
			z[zOff + zPos] = (uint)num;
			num >>= 32;
			num += (long)(z[zOff + zPos + 1] - (x >> 32));
			z[zOff + zPos + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zOff, zPos + 2);
			}
			return 0;
		}

		public static int SubDWordFrom(int len, ulong x, uint[] z)
		{
			long num = (long)(z[0] - (x & 0xFFFFFFFFu));
			z[0] = (uint)num;
			num >>= 32;
			num += (long)(z[1] - (x >> 32));
			z[1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, 2);
			}
			return 0;
		}

		public static int SubDWordFrom(int len, ulong x, uint[] z, int zOff)
		{
			long num = (long)(z[zOff] - (x & 0xFFFFFFFFu));
			z[zOff] = (uint)num;
			num >>= 32;
			num += (long)(z[zOff + 1] - (x >> 32));
			z[zOff + 1] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zOff, 2);
			}
			return 0;
		}

		public static int SubFrom(int len, uint[] x, uint[] z)
		{
			long num = 0L;
			for (int i = 0; i < len; i++)
			{
				num += (long)z[i] - (long)x[i];
				z[i] = (uint)num;
				num >>= 32;
			}
			return (int)num;
		}

		public static int SubFrom(int len, uint[] x, int xOff, uint[] z, int zOff)
		{
			long num = 0L;
			for (int i = 0; i < len; i++)
			{
				num += (long)z[zOff + i] - (long)x[xOff + i];
				z[zOff + i] = (uint)num;
				num >>= 32;
			}
			return (int)num;
		}

		public static int SubWordAt(int len, uint x, uint[] z, int zPos)
		{
			long num = (long)z[zPos] - (long)x;
			z[zPos] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zPos + 1);
			}
			return 0;
		}

		public static int SubWordAt(int len, uint x, uint[] z, int zOff, int zPos)
		{
			long num = (long)z[zOff + zPos] - (long)x;
			z[zOff + zPos] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zOff, zPos + 1);
			}
			return 0;
		}

		public static int SubWordFrom(int len, uint x, uint[] z)
		{
			long num = (long)z[0] - (long)x;
			z[0] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, 1);
			}
			return 0;
		}

		public static int SubWordFrom(int len, uint x, uint[] z, int zOff)
		{
			long num = (long)z[zOff] - (long)x;
			z[zOff] = (uint)num;
			num >>= 32;
			if (num != 0L)
			{
				return DecAt(len, z, zOff, 1);
			}
			return 0;
		}

		public static BigInteger ToBigInteger(int len, uint[] x)
		{
			byte[] array = new byte[len << 2];
			for (int i = 0; i < len; i++)
			{
				uint num = x[i];
				if (num != 0)
				{
					Pack.UInt32_To_BE(num, array, len - 1 - i << 2);
				}
			}
			return new BigInteger(1, array);
		}

		public static void Zero(int len, uint[] z)
		{
			for (int i = 0; i < len; i++)
			{
				z[i] = 0u;
			}
		}
	}
}
