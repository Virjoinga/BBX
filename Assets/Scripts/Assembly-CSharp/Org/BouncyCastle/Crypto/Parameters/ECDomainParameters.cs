using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ECDomainParameters
	{
		internal ECCurve curve;

		internal byte[] seed;

		internal ECPoint g;

		internal BigInteger n;

		internal BigInteger h;

		public ECCurve Curve => curve;

		public ECPoint G => g;

		public BigInteger N => n;

		public BigInteger H => h;

		public ECDomainParameters(ECCurve curve, ECPoint g, BigInteger n)
			: this(curve, g, n, BigInteger.One)
		{
		}

		public ECDomainParameters(ECCurve curve, ECPoint g, BigInteger n, BigInteger h)
			: this(curve, g, n, h, null)
		{
		}

		public ECDomainParameters(ECCurve curve, ECPoint g, BigInteger n, BigInteger h, byte[] seed)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			if (n == null)
			{
				throw new ArgumentNullException("n");
			}
			if (h == null)
			{
				throw new ArgumentNullException("h");
			}
			this.curve = curve;
			this.g = g.Normalize();
			this.n = n;
			this.h = h;
			this.seed = Arrays.Clone(seed);
		}

		public byte[] GetSeed()
		{
			return Arrays.Clone(seed);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is ECDomainParameters other))
			{
				return false;
			}
			return Equals(other);
		}

		protected virtual bool Equals(ECDomainParameters other)
		{
			if (curve.Equals(other.curve) && g.Equals(other.g) && n.Equals(other.n))
			{
				return h.Equals(other.h);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((((curve.GetHashCode() * 37) ^ g.GetHashCode()) * 37) ^ n.GetHashCode()) * 37) ^ h.GetHashCode();
		}
	}
}
