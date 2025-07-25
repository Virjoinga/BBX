namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public abstract class AbstractECMultiplier : ECMultiplier
	{
		public virtual ECPoint Multiply(ECPoint p, BigInteger k)
		{
			int signValue = k.SignValue;
			if (signValue == 0 || p.IsInfinity)
			{
				return p.Curve.Infinity;
			}
			ECPoint eCPoint = MultiplyPositive(p, k.Abs());
			return ECAlgorithms.ValidatePoint((signValue > 0) ? eCPoint : eCPoint.Negate());
		}

		protected abstract ECPoint MultiplyPositive(ECPoint p, BigInteger k);
	}
}
