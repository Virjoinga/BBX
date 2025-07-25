using Bolt;
using UdpKit;

public class DisconnectReasonToken : IProtocolToken
{
	public enum Reason
	{
		InvalidProfile = 0,
		DataFetchFailed = 1,
		AuthenticationFailed = 2,
		NotAuthorized = 3
	}

	public Reason reason;

	public void Read(UdpPacket packet)
	{
		reason = (Reason)packet.ReadInt();
	}

	public void Write(UdpPacket packet)
	{
		packet.WriteInt((int)reason);
	}
}
