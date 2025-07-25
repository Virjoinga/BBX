using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X9
{
	public class DHValidationParms : Asn1Encodable
	{
		private readonly DerBitString seed;

		private readonly DerInteger pgenCounter;

		public DerBitString Seed => seed;

		public DerInteger PgenCounter => pgenCounter;

		public static DHValidationParms GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static DHValidationParms GetInstance(object obj)
		{
			if (obj == null || obj is DHDomainParameters)
			{
				return (DHValidationParms)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new DHValidationParms((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid DHValidationParms: " + Platform.GetTypeName(obj), "obj");
		}

		public DHValidationParms(DerBitString seed, DerInteger pgenCounter)
		{
			if (seed == null)
			{
				throw new ArgumentNullException("seed");
			}
			if (pgenCounter == null)
			{
				throw new ArgumentNullException("pgenCounter");
			}
			this.seed = seed;
			this.pgenCounter = pgenCounter;
		}

		private DHValidationParms(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			seed = DerBitString.GetInstance(seq[0]);
			pgenCounter = DerInteger.GetInstance(seq[1]);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(seed, pgenCounter);
		}
	}
}
