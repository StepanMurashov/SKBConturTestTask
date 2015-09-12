using Sten.WordCompletions.Library;
using Sten.WordCompletions.Server.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sten.WordCompletions.Server
{
    class Program
    {
        private enum ServerMode { NormalStart, ShowHelpOnly }
        private string dictionaryFileName = "";
        private int portNumber = -1;
        private ServerMode mode = ServerMode.ShowHelpOnly;
        private WordCompletionsIOCPServer server;

        private void ParseCommandLine(string[] args)
        {
            const string dictionaryFileNameSwitch = "-F=";
            const string PortNumberSwitch = "-P=";
            foreach (string argument in args)
            {
                if (argument.StartsWith(dictionaryFileNameSwitch, StringComparison.CurrentCultureIgnoreCase))
                    dictionaryFileName = argument.Substring(dictionaryFileNameSwitch.Length);
                if (argument.StartsWith(PortNumberSwitch, StringComparison.CurrentCultureIgnoreCase))
                    portNumber = int.Parse(argument.Substring(PortNumberSwitch.Length));
            }
            if ((dictionaryFileName != "" && portNumber >= 0))
                mode = ServerMode.NormalStart;
        }

        private IWordCompletionsGenerator CreateGenerator(string fileName)
        {
            using (StreamReader reader = File.OpenText(fileName))
                return WordCompletionsGeneratorFactory.CreateFromTextReader(reader,
                    WordCompletionsGeneratorThreadSafetyMode.ThreadSafe);
        }

        private void StartServer()
        {
            server = new WordCompletionsIOCPServer(CreateGenerator(dictionaryFileName), portNumber);
            server.Start();
        }

        private void StopServer()
        {
            server.Stop();
        }

        private void ShowHelp()
        {
            Console.Out.WriteLine(Resources.CommandLineHelp);
        }

        private void Execute()
        {
            switch (mode)
            {
                case ServerMode.NormalStart:
                    StartServer();
                    Console.In.ReadLine();
                    StopServer();
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }

        private Program(string[] args)
        {
            ParseCommandLine(args);
        }

        static void Main(string[] args)
        {
            try
            {
                new Program(args).Execute();
            }
            catch (Exception e)
            {
                Logger.WriteError(e.Message);
                Environment.Exit(-1);
            }
        }
    }
}
