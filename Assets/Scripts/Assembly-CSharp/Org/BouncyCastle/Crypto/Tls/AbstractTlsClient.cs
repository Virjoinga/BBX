using System.Collections;
using System.Collections.Generic;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class AbstractTlsClient : AbstractTlsPeer, TlsClient, TlsPeer
	{
		protected TlsCipherFactory mCipherFactory;

		protected TlsClientContext mContext;

		protected IList mSupportedSignatureAlgorithms;

		protected int[] mNamedCurves;

		protected byte[] mClientECPointFormats;

		protected byte[] mServerECPointFormats;

		protected int mSelectedCipherSuite;

		protected short mSelectedCompressionMethod;

		public List<string> HostNames { get; set; }

		public virtual ProtocolVersion ClientHelloRecordLayerVersion => ClientVersion;

		public virtual ProtocolVersion ClientVersion => ProtocolVersion.TLSv12;

		public virtual bool IsFallback => false;

		public virtual ProtocolVersion MinimumVersion => ProtocolVersion.TLSv10;

		public AbstractTlsClient()
			: this(new DefaultTlsCipherFactory())
		{
		}

		public AbstractTlsClient(TlsCipherFactory cipherFactory)
		{
			mCipherFactory = cipherFactory;
		}

		protected virtual bool AllowUnexpectedServerExtension(int extensionType, byte[] extensionData)
		{
			if (extensionType == 10)
			{
				TlsEccUtilities.ReadSupportedEllipticCurvesExtension(extensionData);
				return true;
			}
			return false;
		}

		protected virtual void CheckForUnexpectedServerExtension(IDictionary serverExtensions, int extensionType)
		{
			byte[] extensionData = TlsUtilities.GetExtensionData(serverExtensions, extensionType);
			if (extensionData != null && !AllowUnexpectedServerExtension(extensionType, extensionData))
			{
				throw new TlsFatalAlert(47);
			}
		}

		public virtual void Init(TlsClientContext context)
		{
			mContext = context;
		}

		public virtual TlsSession GetSessionToResume()
		{
			return null;
		}

		public virtual IDictionary GetClientExtensions()
		{
			IDictionary dictionary = null;
			if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(mContext.ClientVersion))
			{
				mSupportedSignatureAlgorithms = TlsUtilities.GetDefaultSupportedSignatureAlgorithms();
				dictionary = TlsExtensionsUtilities.EnsureExtensionsInitialised(dictionary);
				TlsUtilities.AddSignatureAlgorithmsExtension(dictionary, mSupportedSignatureAlgorithms);
			}
			if (TlsEccUtilities.ContainsEccCipherSuites(GetCipherSuites()))
			{
				mNamedCurves = new int[2] { 23, 24 };
				mClientECPointFormats = new byte[3] { 0, 1, 2 };
				dictionary = TlsExtensionsUtilities.EnsureExtensionsInitialised(dictionary);
				TlsEccUtilities.AddSupportedEllipticCurvesExtension(dictionary, mNamedCurves);
				TlsEccUtilities.AddSupportedPointFormatsExtension(dictionary, mClientECPointFormats);
			}
			if (HostNames != null && HostNames.Count > 0)
			{
				List<ServerName> list = new List<ServerName>(HostNames.Count);
				for (int i = 0; i < HostNames.Count; i++)
				{
					list.Add(new ServerName(0, HostNames[i]));
				}
				TlsExtensionsUtilities.AddServerNameExtension(dictionary, new ServerNameList(list));
			}
			return dictionary;
		}

		public virtual void NotifyServerVersion(ProtocolVersion serverVersion)
		{
			if (!MinimumVersion.IsEqualOrEarlierVersionOf(serverVersion))
			{
				throw new TlsFatalAlert(70);
			}
		}

		public abstract int[] GetCipherSuites();

		public virtual byte[] GetCompressionMethods()
		{
			return new byte[1];
		}

		public virtual void NotifySessionID(byte[] sessionID)
		{
		}

		public virtual void NotifySelectedCipherSuite(int selectedCipherSuite)
		{
			mSelectedCipherSuite = selectedCipherSuite;
		}

		public virtual void NotifySelectedCompressionMethod(byte selectedCompressionMethod)
		{
			mSelectedCompressionMethod = selectedCompressionMethod;
		}

		public virtual void ProcessServerExtensions(IDictionary serverExtensions)
		{
			if (serverExtensions != null)
			{
				CheckForUnexpectedServerExtension(serverExtensions, 13);
				CheckForUnexpectedServerExtension(serverExtensions, 10);
				if (TlsEccUtilities.IsEccCipherSuite(mSelectedCipherSuite))
				{
					mServerECPointFormats = TlsEccUtilities.GetSupportedPointFormatsExtension(serverExtensions);
				}
				else
				{
					CheckForUnexpectedServerExtension(serverExtensions, 11);
				}
				CheckForUnexpectedServerExtension(serverExtensions, 21);
			}
		}

		public virtual void ProcessServerSupplementalData(IList serverSupplementalData)
		{
			if (serverSupplementalData != null)
			{
				throw new TlsFatalAlert(10);
			}
		}

		public abstract TlsKeyExchange GetKeyExchange();

		public abstract TlsAuthentication GetAuthentication();

		public virtual IList GetClientSupplementalData()
		{
			return null;
		}

		public override TlsCompression GetCompression()
		{
			switch (mSelectedCompressionMethod)
			{
			case 0:
				return new TlsNullCompression();
			case 1:
				return new TlsDeflateCompression();
			default:
				throw new TlsFatalAlert(80);
			}
		}

		public override TlsCipher GetCipher()
		{
			int encryptionAlgorithm = TlsUtilities.GetEncryptionAlgorithm(mSelectedCipherSuite);
			int macAlgorithm = TlsUtilities.GetMacAlgorithm(mSelectedCipherSuite);
			return mCipherFactory.CreateCipher(mContext, encryptionAlgorithm, macAlgorithm);
		}

		public virtual void NotifyNewSessionTicket(NewSessionTicket newSessionTicket)
		{
		}
	}
}
