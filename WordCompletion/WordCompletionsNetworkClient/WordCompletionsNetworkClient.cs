using Sten.WordCompletions.Properties;
using Sten.WordCompletions.Server;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;

[assembly: CLSCompliant(true)]
namespace Sten.WordCompletions.NetworkClient
{
    class WordCompletionsNetworkClient
    {
        /// <summary>
        /// Режим запуска клиента.
        /// </summary>
        private enum Mode { NormalStart, ShowHelpOnly }

        /// <summary>
        /// Имя сервера.
        /// </summary>
        private string serverName;

        /// <summary>
        /// Номер порта сервера.
        /// </summary>
        private int portNumber = -1;

        /// <summary>
        /// Режим запуска клиента.
        /// </summary>
        private Mode mode = Mode.ShowHelpOnly;

        /// <summary>
        /// Разобрать параметры командной строки.
        /// </summary>
        /// <param name="args">Параметры командной строки.</param>
        private void ParseCommandLine(string[] args)
        {
            const string serverHostNameSwitch = "-S=";
            const string PortNumberSwitch = "-P=";
            foreach (string argument in args)
            {
                if (argument.StartsWith(serverHostNameSwitch, StringComparison.CurrentCultureIgnoreCase))
                    this.serverName = argument.Substring(serverHostNameSwitch.Length);
                if (argument.StartsWith(PortNumberSwitch, StringComparison.CurrentCultureIgnoreCase))
                    this.portNumber = int.Parse(argument.Substring(PortNumberSwitch.Length), CultureInfo.CurrentCulture);
            }
            if (!string.IsNullOrEmpty(serverName) && portNumber >= 0)
                this.mode = Mode.NormalStart;
        }

        /// <summary>
        /// Показать справку по командной строке.
        /// </summary>
        private static void ShowHelp()
        {
            Console.Out.WriteLine(Resources.CommandLineHelp);
        }

        /// <summary>
        /// Получить IP-адрес сервера.
        /// </summary>
        /// <returns></returns>
        private IPAddress GetServerIPAddress()
        {
            foreach (IPAddress testAddress in Dns.GetHostEntry(this.serverName).AddressList)
                if (testAddress.AddressFamily == AddressFamily.InterNetwork)
                    return testAddress;
            throw new PlatformNotSupportedException(Resources.ServerDoesNotSupportTCPIPv4);
        }

        /// <summary>
        /// Сгенерировать ответы.
        /// </summary>
        /// <param name="input">Вход с вопросами.</param>
        /// <param name="output">Выход для ответов.</param>
        private void GenerateAnswers(TextReader input, TextWriter output)
        {
            IPEndPoint serverEndPoint = new IPEndPoint(GetServerIPAddress(), this.portNumber);

            using (Socket server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp))
            {
                server.Connect(serverEndPoint);
                int questionsCount = int.Parse(input.ReadLine(), CultureInfo.CurrentCulture);
                byte[] answer = new byte[1024];
                int answerLength;
                string question;
                for (int i = 0; i < questionsCount; i++)
                {
                    question = input.ReadLine();

                    server.Send(TCPCommandsBuilder.GetCompletionsCommand.Build(question));
                    answerLength = server.Receive(answer);
                    output.Write(TCPCommandsBuilder.AnswerCommand.Parse(answer, answerLength));
                }
                server.Send(TCPCommandsBuilder.ShutdownCommand.Build(""));
                server.Shutdown(SocketShutdown.Both);
            }
        }

        /// <summary>
        /// Создать экземпляр класса клиента автодополнения слов.
        /// </summary>
        /// <param name="args">Параметры командной строки.</param>
        public WordCompletionsNetworkClient(string[] args)
        {
            ParseCommandLine(args);
        }

        /// <summary>
        /// Запустить клиента.
        /// </summary>
        public void Execute()
        {
            switch (mode)
            {
                case Mode.NormalStart:
                    GenerateAnswers(Console.In, Console.Out);
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                new WordCompletionsNetworkClient(args).Execute();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                Environment.Exit(-1);
            }
        }
    }
}
