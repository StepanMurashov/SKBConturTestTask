using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Sten.WordCompletions.Server
{
    /// <summary>
    /// Сервер генерации вариантов автодополнений слов.
    /// </summary>
    internal class Server
    {
        /// <summary>
        /// Генератор вариантов автодополнений слов.
        /// </summary>
        private IWordCompletionsGenerator wordCompletionsGenerator;

        /// <summary>
        /// Номер порта сервера.
        /// </summary>
        private int portNumber;

        /// <summary>
        /// Сокеты прослушивания входящих соединений.
        /// </summary>
        private List<Socket> listeners = new List<Socket>();

        /// <summary>
        /// Начать прием соединения.
        /// </summary>
        /// <param name="listener">Сокет прослущивания входящих соединений.</param>
        private void BeginAccept(Socket listener)
        {
            listener.BeginAccept(EndAccept, listener);
        }

        /// <summary>
        /// Завершить прием входящего соединения.
        /// </summary>
        /// <param name="ar">Нужен для соответствия прототипу AsyncCallback.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Sten.WordCompletions.Server.Client")]
        private void EndAccept(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            new Client(listener.EndAccept(ar), wordCompletionsGenerator);
            BeginAccept(listener);
        }

        /// <summary>
        /// Запустить сервер.
        /// </summary>
        public void Start()
        {
            foreach (IPAddress ipAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNumber);
                    Socket listener = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        listener.Bind(localEndPoint);
                        listener.Listen(100);
                        BeginAccept(listener);
                        this.listeners.Add(listener);
                        listener = null;
                    }
                    finally
                    {
                        if (!(listener == null))
                            listener.Dispose();
                    }
                }
        }

        /// <summary>
        /// Остановить сервер.
        /// </summary>
        public void Stop()
        {
            foreach (Socket listener in this.listeners)
                listener.Close();
        }

        /// <summary>
        /// Создать экземпляр класса сервера.
        /// </summary>
        /// <param name="wordCompletionsGenerator">Генератор вариантов автодополнения слов.</param>
        /// <param name="portNumber">Номер порта для сервера.</param>
        public Server(IWordCompletionsGenerator wordCompletionsGenerator, int portNumber)
        {
            this.wordCompletionsGenerator = wordCompletionsGenerator;
            this.portNumber = portNumber;
        }
    }
}
