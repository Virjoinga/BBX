using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class TlsUtilities
	{
		public static readonly byte[] EmptyBytes = new byte[0];

		public static readonly short[] EmptyShorts = new short[0];

		public static readonly int[] EmptyInts = new int[0];

		public static readonly long[] EmptyLongs = new long[0];

		internal static readonly byte[] SSL_CLIENT = new byte[4] { 67, 76, 78, 84 };

		internal static readonly byte[] SSL_SERVER = new byte[4] { 83, 82, 86, 82 };

		internal static readonly byte[][] SSL3_CONST = GenSsl3Const();

		public static void CheckUint8(int i)
		{
			if (!IsValidUint8(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static void CheckUint8(long i)
		{
			if (!IsValidUint8(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static void CheckUint16(int i)
		{
			if (!IsValidUint16(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static void CheckUint16(long i)
		{
			if (!IsValidUint16(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static void CheckUint24(int i)
		{
			if (!IsValidUint24(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static void CheckUint24(long i)
		{
			if (!IsValidUint24(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static void CheckUint32(long i)
		{
			if (!IsValidUint32(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static void CheckUint48(long i)
		{
			if (!IsValidUint48(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static void CheckUint64(long i)
		{
			if (!IsValidUint64(i))
			{
				throw new TlsFatalAlert(80);
			}
		}

		public static bool IsValidUint8(int i)
		{
			return (i & 0xFF) == i;
		}

		public static bool IsValidUint8(long i)
		{
			return (i & 0xFF) == i;
		}

		public static bool IsValidUint16(int i)
		{
			return (i & 0xFFFF) == i;
		}

		public static bool IsValidUint16(long i)
		{
			return (i & 0xFFFF) == i;
		}

		public static bool IsValidUint24(int i)
		{
			return (i & 0xFFFFFF) == i;
		}

		public static bool IsValidUint24(long i)
		{
			return (i & 0xFFFFFF) == i;
		}

		public static bool IsValidUint32(long i)
		{
			return (i & 0xFFFFFFFFu) == i;
		}

		public static bool IsValidUint48(long i)
		{
			return (i & 0xFFFFFFFFFFFFL) == i;
		}

		public static bool IsValidUint64(long i)
		{
			return true;
		}

		public static bool IsSsl(TlsContext context)
		{
			return context.ServerVersion.IsSsl;
		}

		public static bool IsTlsV11(ProtocolVersion version)
		{
			return ProtocolVersion.TLSv11.IsEqualOrEarlierVersionOf(version.GetEquivalentTLSVersion());
		}

		public static bool IsTlsV11(TlsContext context)
		{
			return IsTlsV11(context.ServerVersion);
		}

		public static bool IsTlsV12(ProtocolVersion version)
		{
			return ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(version.GetEquivalentTLSVersion());
		}

		public static bool IsTlsV12(TlsContext context)
		{
			return IsTlsV12(context.ServerVersion);
		}

		public static void WriteUint8(byte i, Stream output)
		{
			output.WriteByte(i);
		}

		public static void WriteUint8(byte i, byte[] buf, int offset)
		{
			buf[offset] = i;
		}

		public static void WriteUint16(int i, Stream output)
		{
			output.WriteByte((byte)(i >> 8));
			output.WriteByte((byte)i);
		}

		public static void WriteUint16(int i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 8);
			buf[offset + 1] = (byte)i;
		}

		public static void WriteUint24(int i, Stream output)
		{
			output.WriteByte((byte)(i >> 16));
			output.WriteByte((byte)(i >> 8));
			output.WriteByte((byte)i);
		}

		public static void WriteUint24(int i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 16);
			buf[offset + 1] = (byte)(i >> 8);
			buf[offset + 2] = (byte)i;
		}

		public static void WriteUint32(long i, Stream output)
		{
			output.WriteByte((byte)(i >> 24));
			output.WriteByte((byte)(i >> 16));
			output.WriteByte((byte)(i >> 8));
			output.WriteByte((byte)i);
		}

		public static void WriteUint32(long i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 24);
			buf[offset + 1] = (byte)(i >> 16);
			buf[offset + 2] = (byte)(i >> 8);
			buf[offset + 3] = (byte)i;
		}

		public static void WriteUint48(long i, Stream output)
		{
			output.WriteByte((byte)(i >> 40));
			output.WriteByte((byte)(i >> 32));
			output.WriteByte((byte)(i >> 24));
			output.WriteByte((byte)(i >> 16));
			output.WriteByte((byte)(i >> 8));
			output.WriteByte((byte)i);
		}

		public static void WriteUint48(long i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 40);
			buf[offset + 1] = (byte)(i >> 32);
			buf[offset + 2] = (byte)(i >> 24);
			buf[offset + 3] = (byte)(i >> 16);
			buf[offset + 4] = (byte)(i >> 8);
			buf[offset + 5] = (byte)i;
		}

		public static void WriteUint64(long i, Stream output)
		{
			output.WriteByte((byte)(i >> 56));
			output.WriteByte((byte)(i >> 48));
			output.WriteByte((byte)(i >> 40));
			output.WriteByte((byte)(i >> 32));
			output.WriteByte((byte)(i >> 24));
			output.WriteByte((byte)(i >> 16));
			output.WriteByte((byte)(i >> 8));
			output.WriteByte((byte)i);
		}

		public static void WriteUint64(long i, byte[] buf, int offset)
		{
			buf[offset] = (byte)(i >> 56);
			buf[offset + 1] = (byte)(i >> 48);
			buf[offset + 2] = (byte)(i >> 40);
			buf[offset + 3] = (byte)(i >> 32);
			buf[offset + 4] = (byte)(i >> 24);
			buf[offset + 5] = (byte)(i >> 16);
			buf[offset + 6] = (byte)(i >> 8);
			buf[offset + 7] = (byte)i;
		}

		public static void WriteOpaque8(byte[] buf, Stream output)
		{
			WriteUint8((byte)buf.Length, output);
			output.Write(buf, 0, buf.Length);
		}

		public static void WriteOpaque16(byte[] buf, Stream output)
		{
			WriteUint16(buf.Length, output);
			output.Write(buf, 0, buf.Length);
		}

		public static void WriteOpaque24(byte[] buf, Stream output)
		{
			WriteUint24(buf.Length, output);
			output.Write(buf, 0, buf.Length);
		}

		public static void WriteUint8Array(byte[] uints, Stream output)
		{
			output.Write(uints, 0, uints.Length);
		}

		public static void WriteUint8Array(byte[] uints, byte[] buf, int offset)
		{
			for (int i = 0; i < uints.Length; i++)
			{
				WriteUint8(uints[i], buf, offset);
				offset++;
			}
		}

		public static void WriteUint8ArrayWithUint8Length(byte[] uints, Stream output)
		{
			CheckUint8(uints.Length);
			WriteUint8((byte)uints.Length, output);
			WriteUint8Array(uints, output);
		}

		public static void WriteUint8ArrayWithUint8Length(byte[] uints, byte[] buf, int offset)
		{
			CheckUint8(uints.Length);
			WriteUint8((byte)uints.Length, buf, offset);
			WriteUint8Array(uints, buf, offset + 1);
		}

		public static void WriteUint16Array(int[] uints, Stream output)
		{
			for (int i = 0; i < uints.Length; i++)
			{
				WriteUint16(uints[i], output);
			}
		}

		public static void WriteUint16Array(int[] uints, byte[] buf, int offset)
		{
			for (int i = 0; i < uints.Length; i++)
			{
				WriteUint16(uints[i], buf, offset);
				offset += 2;
			}
		}

		public static void WriteUint16ArrayWithUint16Length(int[] uints, Stream output)
		{
			int i = 2 * uints.Length;
			CheckUint16(i);
			WriteUint16(i, output);
			WriteUint16Array(uints, output);
		}

		public static void WriteUint16ArrayWithUint16Length(int[] uints, byte[] buf, int offset)
		{
			int i = 2 * uints.Length;
			CheckUint16(i);
			WriteUint16(i, buf, offset);
			WriteUint16Array(uints, buf, offset + 2);
		}

		public static byte[] EncodeOpaque8(byte[] buf)
		{
			CheckUint8(buf.Length);
			return Arrays.Prepend(buf, (byte)buf.Length);
		}

		public static byte[] EncodeUint8ArrayWithUint8Length(byte[] uints)
		{
			byte[] array = new byte[1 + uints.Length];
			WriteUint8ArrayWithUint8Length(uints, array, 0);
			return array;
		}

		public static byte[] EncodeUint16ArrayWithUint16Length(int[] uints)
		{
			int num = 2 * uints.Length;
			byte[] array = new byte[2 + num];
			WriteUint16ArrayWithUint16Length(uints, array, 0);
			return array;
		}

		public static byte ReadUint8(Stream input)
		{
			int num = input.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException();
			}
			return (byte)num;
		}

		public static byte ReadUint8(byte[] buf, int offset)
		{
			return buf[offset];
		}

		public static int ReadUint16(Stream input)
		{
			int num = input.ReadByte();
			int num2 = input.ReadByte();
			if (num2 < 0)
			{
				throw new EndOfStreamException();
			}
			return (num << 8) | num2;
		}

		public static int ReadUint16(byte[] buf, int offset)
		{
			return (buf[offset] << 8) | buf[++offset];
		}

		public static int ReadUint24(Stream input)
		{
			int num = input.ReadByte();
			int num2 = input.ReadByte();
			int num3 = input.ReadByte();
			if (num3 < 0)
			{
				throw new EndOfStreamException();
			}
			return (num << 16) | (num2 << 8) | num3;
		}

		public static int ReadUint24(byte[] buf, int offset)
		{
			return (buf[offset] << 16) | (buf[++offset] << 8) | buf[++offset];
		}

		public static long ReadUint32(Stream input)
		{
			int num = input.ReadByte();
			int num2 = input.ReadByte();
			int num3 = input.ReadByte();
			int num4 = input.ReadByte();
			if (num4 < 0)
			{
				throw new EndOfStreamException();
			}
			return (uint)((num << 24) | (num2 << 16) | (num3 << 8) | num4);
		}

		public static long ReadUint32(byte[] buf, int offset)
		{
			return (uint)((buf[offset] << 24) | (buf[++offset] << 16) | (buf[++offset] << 8) | buf[++offset]);
		}

		public static long ReadUint48(Stream input)
		{
			int num = ReadUint24(input);
			int num2 = ReadUint24(input);
			return ((num & 0xFFFFFFFFu) << 24) | (num2 & 0xFFFFFFFFu);
		}

		public static long ReadUint48(byte[] buf, int offset)
		{
			int num = ReadUint24(buf, offset);
			int num2 = ReadUint24(buf, offset + 3);
			return ((num & 0xFFFFFFFFu) << 24) | (num2 & 0xFFFFFFFFu);
		}

		public static byte[] ReadAllOrNothing(int length, Stream input)
		{
			if (length < 1)
			{
				return EmptyBytes;
			}
			byte[] array = new byte[length];
			int num = Streams.ReadFully(input, array);
			if (num == 0)
			{
				return null;
			}
			if (num != length)
			{
				throw new EndOfStreamException();
			}
			return array;
		}

		public static byte[] ReadFully(int length, Stream input)
		{
			if (length < 1)
			{
				return EmptyBytes;
			}
			byte[] array = new byte[length];
			if (length != Streams.ReadFully(input, array))
			{
				throw new EndOfStreamException();
			}
			return array;
		}

		public static void ReadFully(byte[] buf, Stream input)
		{
			if (Streams.ReadFully(input, buf, 0, buf.Length) < buf.Length)
			{
				throw new EndOfStreamException();
			}
		}

		public static byte[] ReadOpaque8(Stream input)
		{
			byte[] array = new byte[ReadUint8(input)];
			ReadFully(array, input);
			return array;
		}

		public static byte[] ReadOpaque16(Stream input)
		{
			byte[] array = new byte[ReadUint16(input)];
			ReadFully(array, input);
			return array;
		}

		public static byte[] ReadOpaque24(Stream input)
		{
			return ReadFully(ReadUint24(input), input);
		}

		public static byte[] ReadUint8Array(int count, Stream input)
		{
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = ReadUint8(input);
			}
			return array;
		}

		public static int[] ReadUint16Array(int count, Stream input)
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = ReadUint16(input);
			}
			return array;
		}

		public static ProtocolVersion ReadVersion(byte[] buf, int offset)
		{
			return ProtocolVersion.Get(buf[offset], buf[offset + 1]);
		}

		public static ProtocolVersion ReadVersion(Stream input)
		{
			int major = input.ReadByte();
			int num = input.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException();
			}
			return ProtocolVersion.Get(major, num);
		}

		public static int ReadVersionRaw(byte[] buf, int offset)
		{
			return (buf[offset] << 8) | buf[offset + 1];
		}

		public static int ReadVersionRaw(Stream input)
		{
			int num = input.ReadByte();
			int num2 = input.ReadByte();
			if (num2 < 0)
			{
				throw new EndOfStreamException();
			}
			return (num << 8) | num2;
		}

		public static Asn1Object ReadAsn1Object(byte[] encoding)
		{
			MemoryStream memoryStream = new MemoryStream(encoding, writable: false);
			Asn1Object asn1Object = new Asn1InputStream(memoryStream, encoding.Length).ReadObject();
			if (asn1Object == null)
			{
				throw new TlsFatalAlert(50);
			}
			if (memoryStream.Position != memoryStream.Length)
			{
				throw new TlsFatalAlert(50);
			}
			return asn1Object;
		}

		public static Asn1Object ReadDerObject(byte[] encoding)
		{
			Asn1Object asn1Object = ReadAsn1Object(encoding);
			if (!Arrays.AreEqual(asn1Object.GetEncoded("DER"), encoding))
			{
				throw new TlsFatalAlert(50);
			}
			return asn1Object;
		}

		public static void WriteGmtUnixTime(byte[] buf, int offset)
		{
			int num = (int)(DateTimeUtilities.CurrentUnixMs() / 1000);
			buf[offset] = (byte)(num >> 24);
			buf[offset + 1] = (byte)(num >> 16);
			buf[offset + 2] = (byte)(num >> 8);
			buf[offset + 3] = (byte)num;
		}

		public static void WriteVersion(ProtocolVersion version, Stream output)
		{
			output.WriteByte((byte)version.MajorVersion);
			output.WriteByte((byte)version.MinorVersion);
		}

		public static void WriteVersion(ProtocolVersion version, byte[] buf, int offset)
		{
			buf[offset] = (byte)version.MajorVersion;
			buf[offset + 1] = (byte)version.MinorVersion;
		}

		public static IList GetDefaultDssSignatureAlgorithms()
		{
			return VectorOfOne(new SignatureAndHashAlgorithm(2, 2));
		}

		public static IList GetDefaultECDsaSignatureAlgorithms()
		{
			return VectorOfOne(new SignatureAndHashAlgorithm(2, 3));
		}

		public static IList GetDefaultRsaSignatureAlgorithms()
		{
			return VectorOfOne(new SignatureAndHashAlgorithm(2, 1));
		}

		public static byte[] GetExtensionData(IDictionary extensions, int extensionType)
		{
			if (extensions != null)
			{
				return (byte[])extensions[extensionType];
			}
			return null;
		}

		public static IList GetDefaultSupportedSignatureAlgorithms()
		{
			byte[] array = new byte[5] { 2, 3, 4, 5, 6 };
			byte[] array2 = new byte[3] { 1, 2, 3 };
			IList list = Platform.CreateArrayList();
			for (int i = 0; i < array2.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					list.Add(new SignatureAndHashAlgorithm(array[j], array2[i]));
				}
			}
			return list;
		}

		public static SignatureAndHashAlgorithm GetSignatureAndHashAlgorithm(TlsContext context, TlsSignerCredentials signerCredentials)
		{
			SignatureAndHashAlgorithm signatureAndHashAlgorithm = null;
			if (IsTlsV12(context))
			{
				signatureAndHashAlgorithm = signerCredentials.SignatureAndHashAlgorithm;
				if (signatureAndHashAlgorithm == null)
				{
					throw new TlsFatalAlert(80);
				}
			}
			return signatureAndHashAlgorithm;
		}

		public static bool HasExpectedEmptyExtensionData(IDictionary extensions, int extensionType, byte alertDescription)
		{
			byte[] extensionData = GetExtensionData(extensions, extensionType);
			if (extensionData == null)
			{
				return false;
			}
			if (extensionData.Length != 0)
			{
				throw new TlsFatalAlert(alertDescription);
			}
			return true;
		}

		public static TlsSession ImportSession(byte[] sessionID, SessionParameters sessionParameters)
		{
			return new TlsSessionImpl(sessionID, sessionParameters);
		}

		public static bool IsSignatureAlgorithmsExtensionAllowed(ProtocolVersion clientVersion)
		{
			return ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(clientVersion.GetEquivalentTLSVersion());
		}

		public static void AddSignatureAlgorithmsExtension(IDictionary extensions, IList supportedSignatureAlgorithms)
		{
			extensions[13] = CreateSignatureAlgorithmsExtension(supportedSignatureAlgorithms);
		}

		public static IList GetSignatureAlgorithmsExtension(IDictionary extensions)
		{
			byte[] extensionData = GetExtensionData(extensions, 13);
			if (extensionData != null)
			{
				return ReadSignatureAlgorithmsExtension(extensionData);
			}
			return null;
		}

		public static byte[] CreateSignatureAlgorithmsExtension(IList supportedSignatureAlgorithms)
		{
			MemoryStream memoryStream = new MemoryStream();
			EncodeSupportedSignatureAlgorithms(supportedSignatureAlgorithms, allowAnonymous: false, memoryStream);
			return memoryStream.ToArray();
		}

		public static IList ReadSignatureAlgorithmsExtension(byte[] extensionData)
		{
			if (extensionData == null)
			{
				throw new ArgumentNullException("extensionData");
			}
			MemoryStream memoryStream = new MemoryStream(extensionData, writable: false);
			IList result = ParseSupportedSignatureAlgorithms(allowAnonymous: false, memoryStream);
			TlsProtocol.AssertEmpty(memoryStream);
			return result;
		}

		public static void EncodeSupportedSignatureAlgorithms(IList supportedSignatureAlgorithms, bool allowAnonymous, Stream output)
		{
			if (supportedSignatureAlgorithms == null)
			{
				throw new ArgumentNullException("supportedSignatureAlgorithms");
			}
			if (supportedSignatureAlgorithms.Count < 1 || supportedSignatureAlgorithms.Count >= 32768)
			{
				throw new ArgumentException("must have length from 1 to (2^15 - 1)", "supportedSignatureAlgorithms");
			}
			int i = 2 * supportedSignatureAlgorithms.Count;
			CheckUint16(i);
			WriteUint16(i, output);
			foreach (SignatureAndHashAlgorithm supportedSignatureAlgorithm in supportedSignatureAlgorithms)
			{
				if (!allowAnonymous && supportedSignatureAlgorithm.Signature == 0)
				{
					throw new ArgumentException("SignatureAlgorithm.anonymous MUST NOT appear in the signature_algorithms extension");
				}
				supportedSignatureAlgorithm.Encode(output);
			}
		}

		public static IList ParseSupportedSignatureAlgorithms(bool allowAnonymous, Stream input)
		{
			int num = ReadUint16(input);
			if (num < 2 || (num & 1) != 0)
			{
				throw new TlsFatalAlert(50);
			}
			int num2 = num / 2;
			IList list = Platform.CreateArrayList(num2);
			for (int i = 0; i < num2; i++)
			{
				SignatureAndHashAlgorithm signatureAndHashAlgorithm = SignatureAndHashAlgorithm.Parse(input);
				if (!allowAnonymous && signatureAndHashAlgorithm.Signature == 0)
				{
					throw new TlsFatalAlert(47);
				}
				list.Add(signatureAndHashAlgorithm);
			}
			return list;
		}

		public static void VerifySupportedSignatureAlgorithm(IList supportedSignatureAlgorithms, SignatureAndHashAlgorithm signatureAlgorithm)
		{
			if (supportedSignatureAlgorithms == null)
			{
				throw new ArgumentNullException("supportedSignatureAlgorithms");
			}
			if (supportedSignatureAlgorithms.Count < 1 || supportedSignatureAlgorithms.Count >= 32768)
			{
				throw new ArgumentException("must have length from 1 to (2^15 - 1)", "supportedSignatureAlgorithms");
			}
			if (signatureAlgorithm == null)
			{
				throw new ArgumentNullException("signatureAlgorithm");
			}
			if (signatureAlgorithm.Signature != 0)
			{
				foreach (SignatureAndHashAlgorithm supportedSignatureAlgorithm in supportedSignatureAlgorithms)
				{
					if (supportedSignatureAlgorithm.Hash == signatureAlgorithm.Hash && supportedSignatureAlgorithm.Signature == signatureAlgorithm.Signature)
					{
						return;
					}
				}
			}
			throw new TlsFatalAlert(47);
		}

		public static byte[] PRF(TlsContext context, byte[] secret, string asciiLabel, byte[] seed, int size)
		{
			if (context.ServerVersion.IsSsl)
			{
				throw new InvalidOperationException("No PRF available for SSLv3 session");
			}
			byte[] array = Strings.ToByteArray(asciiLabel);
			byte[] array2 = Concat(array, seed);
			int prfAlgorithm = context.SecurityParameters.PrfAlgorithm;
			if (prfAlgorithm == 0)
			{
				return PRF_legacy(secret, array, array2, size);
			}
			IDigest digest = CreatePrfHash(prfAlgorithm);
			byte[] array3 = new byte[size];
			HMacHash(digest, secret, array2, array3);
			return array3;
		}

		public static byte[] PRF_legacy(byte[] secret, string asciiLabel, byte[] seed, int size)
		{
			byte[] array = Strings.ToByteArray(asciiLabel);
			byte[] labelSeed = Concat(array, seed);
			return PRF_legacy(secret, array, labelSeed, size);
		}

		internal static byte[] PRF_legacy(byte[] secret, byte[] label, byte[] labelSeed, int size)
		{
			int num = (secret.Length + 1) / 2;
			byte[] array = new byte[num];
			byte[] array2 = new byte[num];
			Array.Copy(secret, 0, array, 0, num);
			Array.Copy(secret, secret.Length - num, array2, 0, num);
			byte[] array3 = new byte[size];
			byte[] array4 = new byte[size];
			HMacHash(CreateHash(1), array, labelSeed, array3);
			HMacHash(CreateHash(2), array2, labelSeed, array4);
			for (int i = 0; i < size; i++)
			{
				array3[i] ^= array4[i];
			}
			return array3;
		}

		internal static byte[] Concat(byte[] a, byte[] b)
		{
			byte[] array = new byte[a.Length + b.Length];
			Array.Copy(a, 0, array, 0, a.Length);
			Array.Copy(b, 0, array, a.Length, b.Length);
			return array;
		}

		internal static void HMacHash(IDigest digest, byte[] secret, byte[] seed, byte[] output)
		{
			HMac hMac = new HMac(digest);
			hMac.Init(new KeyParameter(secret));
			byte[] array = seed;
			int digestSize = digest.GetDigestSize();
			int num = (output.Length + digestSize - 1) / digestSize;
			byte[] array2 = new byte[hMac.GetMacSize()];
			byte[] array3 = new byte[hMac.GetMacSize()];
			for (int i = 0; i < num; i++)
			{
				hMac.BlockUpdate(array, 0, array.Length);
				hMac.DoFinal(array2, 0);
				array = array2;
				hMac.BlockUpdate(array, 0, array.Length);
				hMac.BlockUpdate(seed, 0, seed.Length);
				hMac.DoFinal(array3, 0);
				Array.Copy(array3, 0, output, digestSize * i, System.Math.Min(digestSize, output.Length - digestSize * i));
			}
		}

		internal static void ValidateKeyUsage(X509CertificateStructure c, int keyUsageBits)
		{
			X509Extensions extensions = c.TbsCertificate.Extensions;
			if (extensions != null)
			{
				X509Extension extension = extensions.GetExtension(X509Extensions.KeyUsage);
				if (extension != null && (KeyUsage.GetInstance(extension).GetBytes()[0] & keyUsageBits) != keyUsageBits)
				{
					throw new TlsFatalAlert(46);
				}
			}
		}

		internal static byte[] CalculateKeyBlock(TlsContext context, int size)
		{
			SecurityParameters securityParameters = context.SecurityParameters;
			byte[] masterSecret = securityParameters.MasterSecret;
			byte[] array = Concat(securityParameters.ServerRandom, securityParameters.ClientRandom);
			if (IsSsl(context))
			{
				return CalculateKeyBlock_Ssl(masterSecret, array, size);
			}
			return PRF(context, masterSecret, "key expansion", array, size);
		}

		internal static byte[] CalculateKeyBlock_Ssl(byte[] master_secret, byte[] random, int size)
		{
			IDigest digest = CreateHash(1);
			IDigest digest2 = CreateHash(2);
			int digestSize = digest.GetDigestSize();
			byte[] array = new byte[digest2.GetDigestSize()];
			byte[] array2 = new byte[size + digestSize];
			int num = 0;
			int num2 = 0;
			while (num2 < size)
			{
				byte[] array3 = SSL3_CONST[num];
				digest2.BlockUpdate(array3, 0, array3.Length);
				digest2.BlockUpdate(master_secret, 0, master_secret.Length);
				digest2.BlockUpdate(random, 0, random.Length);
				digest2.DoFinal(array, 0);
				digest.BlockUpdate(master_secret, 0, master_secret.Length);
				digest.BlockUpdate(array, 0, array.Length);
				digest.DoFinal(array2, num2);
				num2 += digestSize;
				num++;
			}
			return Arrays.CopyOfRange(array2, 0, size);
		}

		internal static byte[] CalculateMasterSecret(TlsContext context, byte[] pre_master_secret)
		{
			SecurityParameters securityParameters = context.SecurityParameters;
			byte[] array = (securityParameters.extendedMasterSecret ? securityParameters.SessionHash : Concat(securityParameters.ClientRandom, securityParameters.ServerRandom));
			if (IsSsl(context))
			{
				return CalculateMasterSecret_Ssl(pre_master_secret, array);
			}
			string asciiLabel = (securityParameters.extendedMasterSecret ? ExporterLabel.extended_master_secret : "master secret");
			return PRF(context, pre_master_secret, asciiLabel, array, 48);
		}

		internal static byte[] CalculateMasterSecret_Ssl(byte[] pre_master_secret, byte[] random)
		{
			IDigest digest = CreateHash(1);
			IDigest digest2 = CreateHash(2);
			int digestSize = digest.GetDigestSize();
			byte[] array = new byte[digest2.GetDigestSize()];
			byte[] array2 = new byte[digestSize * 3];
			int num = 0;
			for (int i = 0; i < 3; i++)
			{
				byte[] array3 = SSL3_CONST[i];
				digest2.BlockUpdate(array3, 0, array3.Length);
				digest2.BlockUpdate(pre_master_secret, 0, pre_master_secret.Length);
				digest2.BlockUpdate(random, 0, random.Length);
				digest2.DoFinal(array, 0);
				digest.BlockUpdate(pre_master_secret, 0, pre_master_secret.Length);
				digest.BlockUpdate(array, 0, array.Length);
				digest.DoFinal(array2, num);
				num += digestSize;
			}
			return array2;
		}

		internal static byte[] CalculateVerifyData(TlsContext context, string asciiLabel, byte[] handshakeHash)
		{
			if (IsSsl(context))
			{
				return handshakeHash;
			}
			SecurityParameters securityParameters = context.SecurityParameters;
			byte[] masterSecret = securityParameters.MasterSecret;
			int verifyDataLength = securityParameters.VerifyDataLength;
			return PRF(context, masterSecret, asciiLabel, handshakeHash, verifyDataLength);
		}

		public static IDigest CreateHash(byte hashAlgorithm)
		{
			switch (hashAlgorithm)
			{
			case 1:
				return new MD5Digest();
			case 2:
				return new Sha1Digest();
			case 3:
				return new Sha224Digest();
			case 4:
				return new Sha256Digest();
			case 5:
				return new Sha384Digest();
			case 6:
				return new Sha512Digest();
			default:
				throw new ArgumentException("unknown HashAlgorithm", "hashAlgorithm");
			}
		}

		public static IDigest CreateHash(SignatureAndHashAlgorithm signatureAndHashAlgorithm)
		{
			if (signatureAndHashAlgorithm != null)
			{
				return CreateHash(signatureAndHashAlgorithm.Hash);
			}
			return new CombinedHash();
		}

		public static IDigest CloneHash(byte hashAlgorithm, IDigest hash)
		{
			switch (hashAlgorithm)
			{
			case 1:
				return new MD5Digest((MD5Digest)hash);
			case 2:
				return new Sha1Digest((Sha1Digest)hash);
			case 3:
				return new Sha224Digest((Sha224Digest)hash);
			case 4:
				return new Sha256Digest((Sha256Digest)hash);
			case 5:
				return new Sha384Digest((Sha384Digest)hash);
			case 6:
				return new Sha512Digest((Sha512Digest)hash);
			default:
				throw new ArgumentException("unknown HashAlgorithm", "hashAlgorithm");
			}
		}

		public static IDigest CreatePrfHash(int prfAlgorithm)
		{
			if (prfAlgorithm == 0)
			{
				return new CombinedHash();
			}
			return CreateHash(GetHashAlgorithmForPrfAlgorithm(prfAlgorithm));
		}

		public static IDigest ClonePrfHash(int prfAlgorithm, IDigest hash)
		{
			if (prfAlgorithm == 0)
			{
				return new CombinedHash((CombinedHash)hash);
			}
			return CloneHash(GetHashAlgorithmForPrfAlgorithm(prfAlgorithm), hash);
		}

		public static byte GetHashAlgorithmForPrfAlgorithm(int prfAlgorithm)
		{
			switch (prfAlgorithm)
			{
			case 0:
				throw new ArgumentException("legacy PRF not a valid algorithm", "prfAlgorithm");
			case 1:
				return 4;
			case 2:
				return 5;
			default:
				throw new ArgumentException("unknown PrfAlgorithm", "prfAlgorithm");
			}
		}

		public static DerObjectIdentifier GetOidForHashAlgorithm(byte hashAlgorithm)
		{
			switch (hashAlgorithm)
			{
			case 1:
				return PkcsObjectIdentifiers.MD5;
			case 2:
				return X509ObjectIdentifiers.IdSha1;
			case 3:
				return NistObjectIdentifiers.IdSha224;
			case 4:
				return NistObjectIdentifiers.IdSha256;
			case 5:
				return NistObjectIdentifiers.IdSha384;
			case 6:
				return NistObjectIdentifiers.IdSha512;
			default:
				throw new ArgumentException("unknown HashAlgorithm", "hashAlgorithm");
			}
		}

		internal static short GetClientCertificateType(Certificate clientCertificate, Certificate serverCertificate)
		{
			if (clientCertificate.IsEmpty)
			{
				return -1;
			}
			X509CertificateStructure certificateAt = clientCertificate.GetCertificateAt(0);
			SubjectPublicKeyInfo subjectPublicKeyInfo = certificateAt.SubjectPublicKeyInfo;
			try
			{
				AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(subjectPublicKeyInfo);
				if (asymmetricKeyParameter.IsPrivate)
				{
					throw new TlsFatalAlert(80);
				}
				if (asymmetricKeyParameter is RsaKeyParameters)
				{
					ValidateKeyUsage(certificateAt, 128);
					return 1;
				}
				if (asymmetricKeyParameter is DsaPublicKeyParameters)
				{
					ValidateKeyUsage(certificateAt, 128);
					return 2;
				}
				if (asymmetricKeyParameter is ECPublicKeyParameters)
				{
					ValidateKeyUsage(certificateAt, 128);
					return 64;
				}
				throw new TlsFatalAlert(43);
			}
			catch (Exception alertCause)
			{
				throw new TlsFatalAlert(43, alertCause);
			}
		}

		internal static void TrackHashAlgorithms(TlsHandshakeHash handshakeHash, IList supportedSignatureAlgorithms)
		{
			if (supportedSignatureAlgorithms == null)
			{
				return;
			}
			foreach (SignatureAndHashAlgorithm supportedSignatureAlgorithm in supportedSignatureAlgorithms)
			{
				byte hash = supportedSignatureAlgorithm.Hash;
				if (!HashAlgorithm.IsPrivate(hash))
				{
					handshakeHash.TrackHashAlgorithm(hash);
				}
			}
		}

		public static bool HasSigningCapability(byte clientCertificateType)
		{
			if ((uint)(clientCertificateType - 1) <= 1u || clientCertificateType == 64)
			{
				return true;
			}
			return false;
		}

		public static TlsSigner CreateTlsSigner(byte clientCertificateType)
		{
			switch (clientCertificateType)
			{
			case 2:
				return new TlsDssSigner();
			case 64:
				return new TlsECDsaSigner();
			case 1:
				return new TlsRsaSigner();
			default:
				throw new ArgumentException("not a type with signing capability", "clientCertificateType");
			}
		}

		private static byte[][] GenSsl3Const()
		{
			int num = 10;
			byte[][] array = new byte[num][];
			for (int i = 0; i < num; i++)
			{
				byte[] array2 = new byte[i + 1];
				Arrays.Fill(array2, (byte)(65 + i));
				array[i] = array2;
			}
			return array;
		}

		private static IList VectorOfOne(object obj)
		{
			IList list = Platform.CreateArrayList(1);
			list.Add(obj);
			return list;
		}

		public static int GetCipherType(int ciphersuite)
		{
			switch (GetEncryptionAlgorithm(ciphersuite))
			{
			case 10:
			case 11:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 102:
			case 103:
			case 104:
				return 2;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 12:
			case 13:
			case 14:
				return 1;
			case 1:
			case 2:
				return 0;
			default:
				throw new TlsFatalAlert(80);
			}
		}

		public static int GetEncryptionAlgorithm(int ciphersuite)
		{
			switch (ciphersuite)
			{
			case 10:
			case 13:
			case 16:
			case 19:
			case 22:
			case 139:
			case 143:
			case 147:
			case 49155:
			case 49160:
			case 49165:
			case 49170:
			case 49175:
			case 49178:
			case 49179:
			case 49180:
			case 49204:
				return 7;
			case 47:
			case 48:
			case 49:
			case 50:
			case 51:
			case 60:
			case 62:
			case 63:
			case 64:
			case 103:
			case 140:
			case 144:
			case 148:
			case 174:
			case 178:
			case 182:
			case 49156:
			case 49161:
			case 49166:
			case 49171:
			case 49176:
			case 49181:
			case 49182:
			case 49183:
			case 49187:
			case 49189:
			case 49191:
			case 49193:
			case 49205:
			case 49207:
				return 8;
			case 49308:
			case 49310:
			case 49316:
			case 49318:
			case 49324:
				return 15;
			case 49312:
			case 49314:
			case 49320:
			case 49322:
			case 49326:
				return 16;
			case 156:
			case 158:
			case 160:
			case 162:
			case 164:
			case 168:
			case 170:
			case 172:
			case 49195:
			case 49197:
			case 49199:
			case 49201:
				return 10;
			case 65280:
			case 65282:
			case 65284:
			case 65296:
			case 65298:
			case 65300:
				return 103;
			case 53:
			case 54:
			case 55:
			case 56:
			case 57:
			case 61:
			case 104:
			case 105:
			case 106:
			case 107:
			case 141:
			case 145:
			case 149:
			case 175:
			case 179:
			case 183:
			case 49157:
			case 49162:
			case 49167:
			case 49172:
			case 49177:
			case 49184:
			case 49185:
			case 49186:
			case 49188:
			case 49190:
			case 49192:
			case 49194:
			case 49206:
			case 49208:
				return 9;
			case 49309:
			case 49311:
			case 49317:
			case 49319:
			case 49325:
				return 17;
			case 49313:
			case 49315:
			case 49321:
			case 49323:
			case 49327:
				return 18;
			case 157:
			case 159:
			case 161:
			case 163:
			case 165:
			case 169:
			case 171:
			case 173:
			case 49196:
			case 49198:
			case 49200:
			case 49202:
				return 11;
			case 65281:
			case 65283:
			case 65285:
			case 65297:
			case 65299:
			case 65301:
				return 104;
			case 65:
			case 66:
			case 67:
			case 68:
			case 69:
				return 12;
			case 186:
			case 187:
			case 188:
			case 189:
			case 190:
			case 49266:
			case 49268:
			case 49270:
			case 49272:
			case 49300:
			case 49302:
			case 49304:
			case 49306:
				return 12;
			case 49274:
			case 49276:
			case 49278:
			case 49280:
			case 49282:
			case 49286:
			case 49288:
			case 49290:
			case 49292:
			case 49294:
			case 49296:
			case 49298:
				return 19;
			case 132:
			case 133:
			case 134:
			case 135:
			case 136:
				return 13;
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
				return 13;
			case 49267:
			case 49269:
			case 49271:
			case 49273:
			case 49301:
			case 49303:
			case 49305:
			case 49307:
				return 13;
			case 49275:
			case 49277:
			case 49279:
			case 49281:
			case 49283:
			case 49287:
			case 49289:
			case 49291:
			case 49293:
			case 49295:
			case 49297:
			case 49299:
				return 20;
			case 52392:
			case 52393:
			case 52394:
			case 52395:
			case 52396:
			case 52397:
			case 52398:
				return 102;
			case 1:
				return 0;
			case 2:
			case 44:
			case 45:
			case 46:
			case 49153:
			case 49158:
			case 49163:
			case 49168:
			case 49173:
			case 49209:
				return 0;
			case 59:
			case 176:
			case 180:
			case 184:
			case 49210:
				return 0;
			case 177:
			case 181:
			case 185:
			case 49211:
				return 0;
			case 4:
			case 24:
				return 2;
			case 5:
			case 138:
			case 142:
			case 146:
			case 49154:
			case 49159:
			case 49164:
			case 49169:
			case 49174:
			case 49203:
				return 2;
			case 150:
			case 151:
			case 152:
			case 153:
			case 154:
				return 14;
			default:
				throw new TlsFatalAlert(80);
			}
		}

		public static int GetKeyExchangeAlgorithm(int ciphersuite)
		{
			switch (ciphersuite)
			{
			case 13:
			case 48:
			case 54:
			case 62:
			case 66:
			case 104:
			case 133:
			case 151:
			case 164:
			case 165:
			case 187:
			case 193:
			case 49282:
			case 49283:
				return 7;
			case 16:
			case 49:
			case 55:
			case 63:
			case 67:
			case 105:
			case 134:
			case 152:
			case 160:
			case 161:
			case 188:
			case 194:
			case 49278:
			case 49279:
				return 9;
			case 19:
			case 50:
			case 56:
			case 64:
			case 68:
			case 106:
			case 135:
			case 153:
			case 162:
			case 163:
			case 189:
			case 195:
			case 49280:
			case 49281:
				return 3;
			case 45:
			case 142:
			case 143:
			case 144:
			case 145:
			case 170:
			case 171:
			case 178:
			case 179:
			case 180:
			case 181:
			case 49296:
			case 49297:
			case 49302:
			case 49303:
			case 49318:
			case 49319:
			case 49322:
			case 49323:
			case 52397:
			case 65298:
			case 65299:
				return 14;
			case 22:
			case 51:
			case 57:
			case 69:
			case 103:
			case 107:
			case 136:
			case 154:
			case 158:
			case 159:
			case 190:
			case 196:
			case 49276:
			case 49277:
			case 49310:
			case 49311:
			case 49314:
			case 49315:
			case 52394:
			case 65280:
			case 65281:
				return 5;
			case 49173:
			case 49174:
			case 49175:
			case 49176:
			case 49177:
				return 20;
			case 49153:
			case 49154:
			case 49155:
			case 49156:
			case 49157:
			case 49189:
			case 49190:
			case 49197:
			case 49198:
			case 49268:
			case 49269:
			case 49288:
			case 49289:
			case 65284:
			case 65285:
				return 16;
			case 49163:
			case 49164:
			case 49165:
			case 49166:
			case 49167:
			case 49193:
			case 49194:
			case 49201:
			case 49202:
			case 49272:
			case 49273:
			case 49292:
			case 49293:
				return 18;
			case 49158:
			case 49159:
			case 49160:
			case 49161:
			case 49162:
			case 49187:
			case 49188:
			case 49195:
			case 49196:
			case 49266:
			case 49267:
			case 49286:
			case 49287:
			case 49324:
			case 49325:
			case 49326:
			case 49327:
			case 52393:
				return 17;
			case 49203:
			case 49204:
			case 49205:
			case 49206:
			case 49207:
			case 49208:
			case 49209:
			case 49210:
			case 49211:
			case 49306:
			case 49307:
			case 52396:
			case 65300:
			case 65301:
				return 24;
			case 49168:
			case 49169:
			case 49170:
			case 49171:
			case 49172:
			case 49191:
			case 49192:
			case 49199:
			case 49200:
			case 49270:
			case 49271:
			case 49290:
			case 49291:
			case 52392:
			case 65282:
			case 65283:
				return 19;
			case 44:
			case 138:
			case 139:
			case 140:
			case 141:
			case 168:
			case 169:
			case 174:
			case 175:
			case 176:
			case 177:
			case 49294:
			case 49295:
			case 49300:
			case 49301:
			case 49316:
			case 49317:
			case 49320:
			case 49321:
			case 52395:
			case 65296:
			case 65297:
				return 13;
			case 1:
			case 2:
			case 4:
			case 5:
			case 10:
			case 47:
			case 53:
			case 59:
			case 60:
			case 61:
			case 65:
			case 132:
			case 150:
			case 156:
			case 157:
			case 186:
			case 192:
			case 49274:
			case 49275:
			case 49308:
			case 49309:
			case 49312:
			case 49313:
			case 52398:
				return 1;
			case 46:
			case 146:
			case 147:
			case 148:
			case 149:
			case 172:
			case 173:
			case 182:
			case 183:
			case 184:
			case 185:
			case 49298:
			case 49299:
			case 49304:
			case 49305:
				return 15;
			case 49178:
			case 49181:
			case 49184:
				return 21;
			case 49180:
			case 49183:
			case 49186:
				return 22;
			case 49179:
			case 49182:
			case 49185:
				return 23;
			default:
				throw new TlsFatalAlert(80);
			}
		}

		public static int GetMacAlgorithm(int ciphersuite)
		{
			switch (ciphersuite)
			{
			case 156:
			case 157:
			case 158:
			case 159:
			case 160:
			case 161:
			case 162:
			case 163:
			case 164:
			case 165:
			case 168:
			case 169:
			case 170:
			case 171:
			case 172:
			case 173:
			case 49195:
			case 49196:
			case 49197:
			case 49198:
			case 49199:
			case 49200:
			case 49201:
			case 49202:
			case 49274:
			case 49275:
			case 49276:
			case 49277:
			case 49278:
			case 49279:
			case 49280:
			case 49281:
			case 49282:
			case 49283:
			case 49286:
			case 49287:
			case 49288:
			case 49289:
			case 49290:
			case 49291:
			case 49292:
			case 49293:
			case 49294:
			case 49295:
			case 49296:
			case 49297:
			case 49298:
			case 49299:
			case 49308:
			case 49309:
			case 49310:
			case 49311:
			case 49312:
			case 49313:
			case 49314:
			case 49315:
			case 49316:
			case 49317:
			case 49318:
			case 49319:
			case 49320:
			case 49321:
			case 49322:
			case 49323:
			case 49324:
			case 49325:
			case 49326:
			case 49327:
			case 52392:
			case 52393:
			case 52394:
			case 52395:
			case 52396:
			case 52397:
			case 52398:
			case 65280:
			case 65281:
			case 65282:
			case 65283:
			case 65284:
			case 65285:
			case 65296:
			case 65297:
			case 65298:
			case 65299:
			case 65300:
			case 65301:
				return 0;
			case 1:
			case 4:
				return 1;
			case 2:
			case 5:
			case 10:
			case 13:
			case 16:
			case 19:
			case 22:
			case 44:
			case 45:
			case 46:
			case 47:
			case 48:
			case 49:
			case 50:
			case 51:
			case 53:
			case 54:
			case 55:
			case 56:
			case 57:
			case 65:
			case 66:
			case 67:
			case 68:
			case 69:
			case 132:
			case 133:
			case 134:
			case 135:
			case 136:
			case 138:
			case 139:
			case 140:
			case 141:
			case 142:
			case 143:
			case 144:
			case 145:
			case 146:
			case 147:
			case 148:
			case 149:
			case 150:
			case 151:
			case 152:
			case 153:
			case 154:
			case 49153:
			case 49154:
			case 49155:
			case 49156:
			case 49157:
			case 49158:
			case 49159:
			case 49160:
			case 49161:
			case 49162:
			case 49163:
			case 49164:
			case 49165:
			case 49166:
			case 49167:
			case 49168:
			case 49169:
			case 49170:
			case 49171:
			case 49172:
			case 49173:
			case 49174:
			case 49175:
			case 49176:
			case 49177:
			case 49178:
			case 49179:
			case 49180:
			case 49181:
			case 49182:
			case 49183:
			case 49184:
			case 49185:
			case 49186:
			case 49203:
			case 49204:
			case 49205:
			case 49206:
			case 49209:
				return 2;
			case 59:
			case 60:
			case 61:
			case 62:
			case 63:
			case 64:
			case 103:
			case 104:
			case 105:
			case 106:
			case 107:
			case 174:
			case 176:
			case 178:
			case 180:
			case 182:
			case 184:
			case 186:
			case 187:
			case 188:
			case 189:
			case 190:
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
			case 49187:
			case 49189:
			case 49191:
			case 49193:
			case 49207:
			case 49210:
			case 49266:
			case 49268:
			case 49270:
			case 49272:
			case 49300:
			case 49302:
			case 49304:
			case 49306:
				return 3;
			case 175:
			case 177:
			case 179:
			case 181:
			case 183:
			case 185:
			case 49188:
			case 49190:
			case 49192:
			case 49194:
			case 49208:
			case 49211:
			case 49267:
			case 49269:
			case 49271:
			case 49273:
			case 49301:
			case 49303:
			case 49305:
			case 49307:
				return 4;
			default:
				throw new TlsFatalAlert(80);
			}
		}

		public static ProtocolVersion GetMinimumVersion(int ciphersuite)
		{
			if (ciphersuite <= 49202)
			{
				switch (ciphersuite)
				{
				case 59:
				case 60:
				case 61:
				case 62:
				case 63:
				case 64:
				case 103:
				case 104:
				case 105:
				case 106:
				case 107:
				case 156:
				case 157:
				case 158:
				case 159:
				case 160:
				case 161:
				case 162:
				case 163:
				case 164:
				case 165:
				case 168:
				case 169:
				case 170:
				case 171:
				case 172:
				case 173:
				case 186:
				case 187:
				case 188:
				case 189:
				case 190:
				case 191:
				case 192:
				case 193:
				case 194:
				case 195:
				case 196:
				case 197:
				case 49187:
				case 49188:
				case 49189:
				case 49190:
				case 49191:
				case 49192:
				case 49193:
				case 49194:
				case 49195:
				case 49196:
				case 49197:
				case 49198:
				case 49199:
				case 49200:
				case 49201:
				case 49202:
					break;
				default:
					goto IL_012e;
				}
				goto IL_0128;
			}
			if (ciphersuite <= 49327)
			{
				if ((uint)(ciphersuite - 49266) <= 33u || (uint)(ciphersuite - 49308) <= 19u)
				{
					goto IL_0128;
				}
			}
			else if ((uint)(ciphersuite - 52392) <= 6u || (uint)(ciphersuite - 65280) <= 5u || (uint)(ciphersuite - 65296) <= 5u)
			{
				goto IL_0128;
			}
			goto IL_012e;
			IL_012e:
			return ProtocolVersion.SSLv3;
			IL_0128:
			return ProtocolVersion.TLSv12;
		}

		public static bool IsAeadCipherSuite(int ciphersuite)
		{
			return 2 == GetCipherType(ciphersuite);
		}

		public static bool IsBlockCipherSuite(int ciphersuite)
		{
			return 1 == GetCipherType(ciphersuite);
		}

		public static bool IsStreamCipherSuite(int ciphersuite)
		{
			return GetCipherType(ciphersuite) == 0;
		}

		public static bool IsValidCipherSuiteForVersion(int cipherSuite, ProtocolVersion serverVersion)
		{
			return GetMinimumVersion(cipherSuite).IsEqualOrEarlierVersionOf(serverVersion.GetEquivalentTLSVersion());
		}
	}
}
