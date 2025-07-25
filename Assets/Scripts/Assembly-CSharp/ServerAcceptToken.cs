using Bolt;
using UdpKit;

public class ServerAcceptToken : IProtocolToken
{
	public string data;

	public void Read(UdpPacket packet)
	{
		data = packet.ReadString();
	}

	public void Write(UdpPacket packet)
	{
		packet.WriteString(data);
	}
}
