using System;
using System.Net;
using System.Net.Sockets;

namespace Sonic.Net.Network
{
	/// <summary>
	/// Summary description for SocketClient.
	/// </summary>
	public class SocketClient
	{
		public SocketClient(EndPoint ep)
		{
			_readDataHandler = new AsyncCallback(this.ReadDataHandler);

			Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.IP);
			s.Connect(ep);
			int sizeToRead = 0;
			byte[] data = GetNextReceiveBuffer(ref sizeToRead);
			s.BeginReceive(data,0,sizeToRead,SocketFlags.Partial,_readDataHandler,this); 
		}

		private void ReadDataHandler(IAsyncResult ar)
		{

		}
		
		private byte[] GetNextReceiveBuffer(ref int sizeToread)
		{
			return null;
		}

		private AsyncCallback _readDataHandler; 
	}
}
