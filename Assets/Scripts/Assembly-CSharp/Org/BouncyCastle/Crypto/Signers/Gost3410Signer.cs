using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class Gost3410Signer : IDsa
	{
		private Gost3410KeyParameters key;

		private SecureRandom random;

		public virtual string AlgorithmName => "GOST3410";

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			if (forSigning)
			{
				if (parameters is ParametersWithRandom)
				{
					ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
					random = parametersWithRandom.Random;
					parameters = parametersWithRandom.Parameters;
				}
				else
				{
					random = new SecureRandom();
				}
				if (!(parameters is Gost3410PrivateKeyParameters))
				{
					throw new InvalidKeyException("GOST3410 private key required for signing");
				}
				key = (Gost3410PrivateKeyParameters)parameters;
			}
			else
			{
				if (!(parameters is Gost3410PublicKeyParameters))
				{
					throw new InvalidKeyException("GOST3410 public key required for signing");
				}
				key = (Gost3410PublicKeyParameters)parameters;
			}
		}

		public virtual BigInteger[] GenerateSignature(byte[] message)
		{
			byte[] array = new byte[message.Length];
			for (int i = 0; i != array.Length; i++)
			{
				array[i] = message[array.Length - 1 - i];
			}
			BigInteger val = new BigInteger(1, array);
			Gost3410Parameters parameters = key.Parameters;
			BigInteger bigInteger;
			do
			{
				bigInteger = new BigInteger(parameters.Q.BitLength, random);
			}
			while (bigInteger.CompareTo(parameters.Q) >= 0);
			BigInteger bigInteger2 = parameters.A.ModPow(bigInteger, parameters.P).Mod(parameters.Q);
			BigInteger bigInteger3 = bigInteger.Multiply(val).Add(((Gost3410PrivateKeyParameters)key).X.Multiply(bigInteger2)).Mod(parameters.Q);
			return new BigInteger[2] { bigInteger2, bigInteger3 };
		}

		public virtual bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
		{
			byte[] array = new byte[message.Length];
			for (int i = 0; i != array.Length; i++)
			{
				array[i] = message[array.Length - 1 - i];
			}
			BigInteger bigInteger = new BigInteger(1, array);
			Gost3410Parameters parameters = key.Parameters;
			if (r.SignValue < 0 || parameters.Q.CompareTo(r) <= 0)
			{
				return false;
			}
			if (s.SignValue < 0 || parameters.Q.CompareTo(s) <= 0)
			{
				return false;
			}
			BigInteger val = bigInteger.ModPow(parameters.Q.Subtract(BigInteger.Two), parameters.Q);
			BigInteger e = s.Multiply(val).Mod(parameters.Q);
			BigInteger e2 = parameters.Q.Subtract(r).Multiply(val).Mod(parameters.Q);
			e = parameters.A.ModPow(e, parameters.P);
			e2 = ((Gost3410PublicKeyParameters)key).Y.ModPow(e2, parameters.P);
			return e.Multiply(e2).Mod(parameters.P).Mod(parameters.Q)
				.Equals(r);
		}
	}
}
