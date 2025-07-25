using Bolt;
using UdpKit;
using UnityEngine;

public class ServerConnectToken : IProtocolToken
{
	public class Data
	{
		public string serviceToken;

		public PlatformType Platform;
	}

	public string data;

	public Data GetData()
	{
		return JsonUtility.FromJson<Data>(data);
	}

	public void SetData(Data data)
	{
		this.data = JsonUtility.ToJson(data);
	}

	public void Read(UdpPacket packet)
	{
		data = packet.ReadString();
	}

	public void Write(UdpPacket packet)
	{
		packet.WriteString(data);
	}
}
