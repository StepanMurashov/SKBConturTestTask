using Sonic.Net;
using Sonic.Net.ThreadPoolTaskFramework;
using Sten.WordCompletions.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sten.WordCompletions.Server
{
    internal class AcceptTask : GenericTask
    {
        private Socket listener;
        public AcceptTask(Socket listener)
        {
            this.listener = listener;
        }

        public override void Execute(Sonic.Net.ThreadPool threadPool)
        {
            Socket client = listener.Accept();
            threadPool.Dispatch(this);
        }
    }

    internal class WordCompletionsIOCPServer
    {
        private Sonic.Net.ThreadPool iocpThreadPool;
        private IWordCompletionsGenerator wordCompletionsGenerator;
        private AutoResetEvent stopEvent = new AutoResetEvent(false);
        private void HandleException(Exception e)
        {
            Logger.WriteError(e.Message);
        }
        public WordCompletionsIOCPServer(IWordCompletionsGenerator wordCompletionsGenerator)
        {
            this.wordCompletionsGenerator = wordCompletionsGenerator;
            this.iocpThreadPool = new Sonic.Net.ThreadPool(
                (short)(Environment.ProcessorCount * 2),
                (short)Environment.ProcessorCount,
                HandleException);
        }

        private void Execute()
        {

            IPAddress ipAddress = null;
            // TODO: Переделать на прослушивание всех адресов.
            foreach (IPAddress testAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                if (testAddress.AddressFamily == AddressFamily.InterNetwork)
                    ipAddress = testAddress;
            if (ipAddress == null)
                throw new ArgumentOutOfRangeException();
            // TODO: Переделать на произвольный порт.
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(100);
            Logger.WriteVerbose("Server started.");
            while (true)
            {
                Socket client = listener.Accept();
                byte[] bytes = new byte[1024];
                int bytesRec = client.Receive(bytes);
                string wordToComplete = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                while (wordToComplete.CompareTo("Bye!") != 0)
                {
                    IEnumerable<IWordCompletion> completions =
                        wordCompletionsGenerator.GetTenBestCompletions(wordToComplete.Substring(4));
                    StringBuilder result = new StringBuilder().Append("answer ");
                    foreach (IWordCompletion completion in completions)
                        result.AppendLine(completion.Word);
                    client.Send(Encoding.ASCII.GetBytes(result.ToString()));
                    bytesRec = client.Receive(bytes);
                    wordToComplete = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                }
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            // iocpThreadPool.Dispatch(new AcceptTask(listener));
            // TODO: Написать остановку сервера.
        }
        public void Start()
        {
            this.Execute();
            //new Thread(() => this.Execute());
        }

        public void Stop()
        {
            //stopEvent.Reset();
            //stopEvent.WaitOne();
        }
    }
}
