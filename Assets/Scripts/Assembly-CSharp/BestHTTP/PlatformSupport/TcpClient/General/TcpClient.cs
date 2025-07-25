using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BestHTTP.PlatformSupport.TcpClient.General
{
	public class TcpClient : IDisposable
	{
		private enum Properties : uint
		{
			LingerState = 1u,
			NoDelay = 2u,
			ReceiveBufferSize = 4u,
			ReceiveTimeout = 8u,
			SendBufferSize = 0x10u,
			SendTimeout = 0x20u
		}

		private NetworkStream stream;

		private bool active;

		private Socket client;

		private bool disposed;

		private Properties values;

		private int recv_timeout;

		private int send_timeout;

		private int recv_buffer_size;

		private int send_buffer_size;

		private LingerOption linger_state;

		private bool no_delay;

		protected bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active = value;
			}
		}

		public Socket Client
		{
			get
			{
				return client;
			}
			set
			{
				client = value;
				stream = null;
			}
		}

		public int Available => client.Available;

		public bool Connected => client.Connected;

		public bool ExclusiveAddressUse
		{
			get
			{
				return client.ExclusiveAddressUse;
			}
			set
			{
				client.ExclusiveAddressUse = value;
			}
		}

		public LingerOption LingerState
		{
			get
			{
				if ((values & Properties.LingerState) != 0)
				{
					return linger_state;
				}
				return (LingerOption)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				if (!client.Connected)
				{
					linger_state = value;
					values |= Properties.LingerState;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
				}
			}
		}

		public bool NoDelay
		{
			get
			{
				if ((values & Properties.NoDelay) != 0)
				{
					return no_delay;
				}
				return (bool)client.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug);
			}
			set
			{
				if (!client.Connected)
				{
					no_delay = value;
					values |= Properties.NoDelay;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, value ? 1 : 0);
				}
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				if ((values & Properties.ReceiveBufferSize) != 0)
				{
					return recv_buffer_size;
				}
				return (int)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				if (!client.Connected)
				{
					recv_buffer_size = value;
					values |= Properties.ReceiveBufferSize;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
				}
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				if ((values & Properties.ReceiveTimeout) != 0)
				{
					return recv_timeout;
				}
				return (int)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				if (!client.Connected)
				{
					recv_timeout = value;
					values |= Properties.ReceiveTimeout;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
				}
			}
		}

		public int SendBufferSize
		{
			get
			{
				if ((values & Properties.SendBufferSize) != 0)
				{
					return send_buffer_size;
				}
				return (int)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				if (!client.Connected)
				{
					send_buffer_size = value;
					values |= Properties.SendBufferSize;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
				}
			}
		}

		public int SendTimeout
		{
			get
			{
				if ((values & Properties.SendTimeout) != 0)
				{
					return send_timeout;
				}
				return (int)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				if (!client.Connected)
				{
					send_timeout = value;
					values |= Properties.SendTimeout;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
				}
			}
		}

		public TimeSpan ConnectTimeout { get; set; }

		private void Init(AddressFamily family)
		{
			active = false;
			if (client != null)
			{
				client.Close();
				client = null;
			}
			client = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
		}

		public TcpClient()
		{
			Init(AddressFamily.InterNetwork);
			ConnectTimeout = TimeSpan.FromSeconds(2.0);
		}

		public TcpClient(AddressFamily family)
		{
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
			}
			Init(family);
			ConnectTimeout = TimeSpan.FromSeconds(2.0);
		}

		public TcpClient(IPEndPoint localEP)
		{
			Init(localEP.AddressFamily);
			ConnectTimeout = TimeSpan.FromSeconds(2.0);
		}

		public TcpClient(string hostname, int port)
		{
			ConnectTimeout = TimeSpan.FromSeconds(2.0);
			Connect(hostname, port);
		}

		public bool IsConnected()
		{
			try
			{
				return !Client.Poll(1, SelectMode.SelectRead) || Client.Available != 0;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal void SetTcpClient(Socket s)
		{
			Client = s;
		}

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		public void Connect(IPEndPoint remoteEP)
		{
			try
			{
				if (ConnectTimeout > TimeSpan.Zero)
				{
					ManualResetEvent mre = new ManualResetEvent(initialState: false);
					IAsyncResult asyncResult = client.BeginConnect(remoteEP, delegate
					{
						mre.Set();
					}, null);
					active = mre.WaitOne(ConnectTimeout);
					if (!active)
					{
						try
						{
							client.Close();
						}
						catch
						{
						}
						throw new TimeoutException("Connection timed out!");
					}
					client.EndConnect(asyncResult);
				}
				else
				{
					client.Connect(remoteEP);
					active = true;
				}
			}
			finally
			{
				CheckDisposed();
			}
		}

		public void Connect(IPAddress address, int port)
		{
			Connect(new IPEndPoint(address, port));
		}

		private void SetOptions()
		{
			Properties num = values;
			values = (Properties)0u;
			if ((num & Properties.LingerState) != 0)
			{
				LingerState = linger_state;
			}
			if ((num & Properties.NoDelay) != 0)
			{
				NoDelay = no_delay;
			}
			if ((num & Properties.ReceiveBufferSize) != 0)
			{
				ReceiveBufferSize = recv_buffer_size;
			}
			if ((num & Properties.ReceiveTimeout) != 0)
			{
				ReceiveTimeout = recv_timeout;
			}
			if ((num & Properties.SendBufferSize) != 0)
			{
				SendBufferSize = send_buffer_size;
			}
			if ((num & Properties.SendTimeout) != 0)
			{
				SendTimeout = send_timeout;
			}
		}

		public void Connect(string hostname, int port)
		{
			if (ConnectTimeout > TimeSpan.Zero)
			{
				ManualResetEvent mre = new ManualResetEvent(initialState: false);
				IAsyncResult asyncResult = Dns.BeginGetHostAddresses(hostname, delegate
				{
					mre.Set();
				}, null);
				if (!mre.WaitOne(ConnectTimeout))
				{
					throw new TimeoutException("DNS resolve timed out!");
				}
				IPAddress[] ipAddresses = Dns.EndGetHostAddresses(asyncResult);
				Connect(ipAddresses, port);
			}
			else
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
				Connect(hostAddresses, port);
			}
		}

		public void Connect(IPAddress[] ipAddresses, int port)
		{
			CheckDisposed();
			if (ipAddresses == null)
			{
				throw new ArgumentNullException("ipAddresses");
			}
			for (int i = 0; i < ipAddresses.Length; i++)
			{
				try
				{
					IPAddress iPAddress = ipAddresses[i];
					if (iPAddress.Equals(IPAddress.Any) || iPAddress.Equals(IPAddress.IPv6Any))
					{
						throw new SocketException(10049);
					}
					Init(iPAddress.AddressFamily);
					if (iPAddress.AddressFamily != AddressFamily.InterNetwork && iPAddress.AddressFamily != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This method is only valid for sockets in the InterNetwork and InterNetworkV6 families");
					}
					Connect(new IPEndPoint(iPAddress, port));
					if (values != 0)
					{
						SetOptions();
					}
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, optionValue: true);
					break;
				}
				catch (Exception ex)
				{
					Init(AddressFamily.InterNetwork);
					if (i == ipAddresses.Length - 1)
					{
						throw ex;
					}
				}
			}
		}

		public void EndConnect(IAsyncResult asyncResult)
		{
			client.EndConnect(asyncResult);
		}

		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
		{
			return client.BeginConnect(address, port, requestCallback, state);
		}

		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
		{
			return client.BeginConnect(addresses, port, requestCallback, state);
		}

		public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
		{
			return client.BeginConnect(host, port, requestCallback, state);
		}

		void IDisposable.Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (disposing)
			{
				NetworkStream networkStream = stream;
				stream = null;
				if (networkStream != null)
				{
					networkStream.Close();
					active = false;
					networkStream = null;
				}
				else if (client != null)
				{
					client.Close();
					client = null;
				}
			}
		}

		~TcpClient()
		{
			Dispose(disposing: false);
		}

		public Stream GetStream()
		{
			try
			{
				if (stream == null)
				{
					stream = new NetworkStream(client, ownsSocket: true);
				}
				return stream;
			}
			finally
			{
				CheckDisposed();
			}
		}

		private void CheckDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
	}
}
