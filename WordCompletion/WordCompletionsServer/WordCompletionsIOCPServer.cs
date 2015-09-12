using Sten.WordCompletions.Server.Properties;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sten.WordCompletions.Server
{
    internal class Client
    {
        private Socket client;
        private byte[] buffer = new byte[1024];
        private IWordCompletionsGenerator wordCompletionsGenerator;

        private void SendCallback(IAsyncResult ar)
        {
            BeginReceive();
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int bytesRead = this.client.EndReceive(ar);
            string command = Encoding.ASCII.GetString(this.buffer, 0, bytesRead);
            if (command.CompareTo("Bye!") != 0)
            {
                IEnumerable<IWordCompletion> completions =
                    wordCompletionsGenerator.GetTenBestCompletions(command.Substring(4));
                StringBuilder result = new StringBuilder().Append("answer ");
                foreach (IWordCompletion completion in completions)
                    result.AppendLine(completion.Word);
                client.BeginSend(Encoding.ASCII.GetBytes(result.ToString()), 0, result.Length, 0, SendCallback, null);
            }
            else
            {
                // Возможно утечка ресурсов?
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }
        public Client(Socket client, IWordCompletionsGenerator wordCompletionsGenerator)
        {
            this.client = client;
            this.wordCompletionsGenerator = wordCompletionsGenerator;
        }

        public void BeginReceive()
        {
            this.client.BeginReceive(this.buffer, 0, this.buffer.Length, 0, ReceiveCallback, null);
        }
    }

    internal class WordCompletionsIOCPServer
    {
        private IWordCompletionsGenerator wordCompletionsGenerator;
        private int portNumber;

        public WordCompletionsIOCPServer(IWordCompletionsGenerator wordCompletionsGenerator, int portNumber)
        {
            this.wordCompletionsGenerator = wordCompletionsGenerator;
            this.portNumber = portNumber;
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            new Client(listener.EndAccept(ar), wordCompletionsGenerator).BeginReceive();
            listener.BeginAccept(AcceptCallback, null);
        }

        public void Start()
        {
            foreach (IPAddress ipAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNumber);
                    Socket listener = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);
                    listener.Bind(localEndPoint);
                    listener.Listen(100);
                    Logger.WriteInfo(Resources.ServerStarted);
                    listener.BeginAccept(AcceptCallback, listener);
                }
        }

        public void Stop()
        {
        }
    }
}
