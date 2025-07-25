using Bolt;
using UdpKit;

public class RoomProtocolToken : IProtocolToken
{
	public string ArbitraryData;

	public string password;

	public void Read(UdpPacket packet)
	{
		ArbitraryData = packet.ReadString();
		password = packet.ReadString();
	}

	public void Write(UdpPacket packet)
	{
		packet.WriteString(ArbitraryData);
		packet.WriteString(password);
	}
}
