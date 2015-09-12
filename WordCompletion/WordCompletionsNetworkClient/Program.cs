﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WordCompletionsNetworkClient.Properties;

namespace WordCompletionsNetworkClient
{
    class WordCompletionsNetworkClient
    {
        private enum Mode { NormalStart, ShowHelpOnly }
        private int portNumber = -1;
        string serverName = "";
        private Mode mode = Mode.ShowHelpOnly;

        private void ParseCommandLine(string[] args)
        {
            const string serverHostNameSwitch = "-S=";
            const string PortNumberSwitch = "-P=";
            foreach (string argument in args)
            {
                if (argument.StartsWith(serverHostNameSwitch))
                    this.serverName = argument.Substring(serverHostNameSwitch.Length);
                if (argument.StartsWith(PortNumberSwitch))
                    this.portNumber = int.Parse(argument.Substring(PortNumberSwitch.Length));
            }
            if ((serverName != "" && portNumber >= 0))
                this.mode = Mode.NormalStart;
        }

        private void ShowHelp()
        {
            Console.Out.WriteLine(Resources.CommandLineHelp);
        }

        private void GenerateAnswers(TextReader input, TextWriter output)
        {
            IPAddress ipAddress = null;
            foreach (IPAddress testAddress in Dns.GetHostEntry(serverName).AddressList)
                if (testAddress.AddressFamily == AddressFamily.InterNetwork)
                    ipAddress = testAddress;
            if (ipAddress == null)
                throw new ArgumentOutOfRangeException();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNumber);

            Socket server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            server.Connect(localEndPoint);
            int questionsCount = int.Parse(input.ReadLine(), CultureInfo.CurrentCulture);
            byte[] answer = new byte[1024];
            int anLength;
            string question;
            for (int i = 0; i < questionsCount; i++)
            {
                question = input.ReadLine();
                server.Send(Encoding.ASCII.GetBytes("get " + question));
                anLength = server.Receive(answer);
                output.Write(Encoding.ASCII.GetString(answer, 0, anLength).Substring(7));
                if (i != questionsCount - 1)
                    output.WriteLine();
            }
            server.Send(Encoding.ASCII.GetBytes("Bye!"));
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        public WordCompletionsNetworkClient(string[] args)
        {
            this.ParseCommandLine(args);
        }

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
            new WordCompletionsNetworkClient(args).Execute();
        }
    }
}
