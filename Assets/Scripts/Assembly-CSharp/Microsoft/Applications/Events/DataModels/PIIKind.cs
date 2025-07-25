using System.CodeDom.Compiler;

namespace Microsoft.Applications.Events.DataModels
{
	[GeneratedCode("gbc", "0.10.0.0")]
	internal enum PIIKind
	{
		NotSet = 0,
		DistinguishedName = 1,
		GenericData = 2,
		IPV4Address = 3,
		IPv6Address = 4,
		MailSubject = 5,
		PhoneNumber = 6,
		QueryString = 7,
		SipAddress = 8,
		SmtpAddress = 9,
		Identity = 10,
		Uri = 11,
		Fqdn = 12,
		IPV4AddressLegacy = 13
	}
}
