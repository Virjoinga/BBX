using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class ByteQueue
	{
		private const int DefaultCapacity = 1024;

		private byte[] databuf;

		private int skipped;

		private int available;

		public int Available => available;

		public static int NextTwoPow(int i)
		{
			i |= i >> 1;
			i |= i >> 2;
			i |= i >> 4;
			i |= i >> 8;
			i |= i >> 16;
			return i + 1;
		}

		public ByteQueue()
			: this(1024)
		{
		}

		public ByteQueue(int capacity)
		{
			databuf = new byte[capacity];
		}

		public void Read(byte[] buf, int offset, int len, int skip)
		{
			if (buf.Length - offset < len)
			{
				throw new ArgumentException("Buffer size of " + buf.Length + " is too small for a read of " + len + " bytes");
			}
			if (available - skip < len)
			{
				throw new InvalidOperationException("Not enough data to read");
			}
			Array.Copy(databuf, skipped + skip, buf, offset, len);
		}

		public void AddData(byte[] data, int offset, int len)
		{
			if (skipped + available + len > databuf.Length)
			{
				int num = NextTwoPow(available + len);
				if (num > databuf.Length)
				{
					byte[] destinationArray = new byte[num];
					Array.Copy(databuf, skipped, destinationArray, 0, available);
					databuf = destinationArray;
				}
				else
				{
					Array.Copy(databuf, skipped, databuf, 0, available);
				}
				skipped = 0;
			}
			Array.Copy(data, offset, databuf, skipped + available, len);
			available += len;
		}

		public void RemoveData(int i)
		{
			if (i > available)
			{
				throw new InvalidOperationException("Cannot remove " + i + " bytes, only got " + available);
			}
			available -= i;
			skipped += i;
		}

		public void RemoveData(byte[] buf, int off, int len, int skip)
		{
			Read(buf, off, len, skip);
			RemoveData(skip + len);
		}

		public byte[] RemoveData(int len, int skip)
		{
			byte[] array = new byte[len];
			RemoveData(array, 0, len, skip);
			return array;
		}
	}
}
