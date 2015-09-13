using Sten.WordCompletions.Server.Properties;
using System;
using System.Globalization;
using System.IO;

[assembly: CLSCompliant(true)]
namespace Sten.WordCompletions.Server
{
    /// <summary>
    /// Загрузчик сервера автодополнений слов.
    /// </summary>
    internal class Loader
    {
        /// <summary>
        /// Режим запуска сервера.
        /// </summary>
        private enum ServerMode { NormalStart, ShowHelpOnly }

        /// <summary>
        /// Имя словаря автодополнений слов.
        /// </summary>
        private string dictionaryFileName;

        /// <summary>
        /// Номер порта сервера.
        /// </summary>
        private int portNumber = -1;

        /// <summary>
        /// Режим запуска сервера.
        /// </summary>
        private ServerMode mode = ServerMode.ShowHelpOnly;

        /// <summary>
        /// Сервер автодополнений слов.
        /// </summary>
        private Server server;

        /// <summary>
        /// Разобрать параметры командной строки.
        /// </summary>
        /// <param name="args">Параметры командной строки.</param>
        private void ParseCommandLine(string[] args)
        {
            const string dictionaryFileNameSwitch = "-F=";
            const string PortNumberSwitch = "-P=";
            foreach (string argument in args)
            {
                if (argument.StartsWith(dictionaryFileNameSwitch, StringComparison.CurrentCultureIgnoreCase))
                    this.dictionaryFileName = argument.Substring(dictionaryFileNameSwitch.Length);
                if (argument.StartsWith(PortNumberSwitch, StringComparison.CurrentCultureIgnoreCase))
                    this.portNumber = int.Parse(argument.Substring(PortNumberSwitch.Length), CultureInfo.CurrentCulture);
            }
            if ((!string.IsNullOrEmpty(dictionaryFileName) && portNumber >= 0))
                this.mode = ServerMode.NormalStart;
        }

        /// <summary>
        /// Создать генератор автодополнений слов.
        /// </summary>
        /// <param name="fileName">Имя файла со словарем автодополнений слов.</param>
        /// <returns>Генератор автодополнений слов.</returns>
        private static IWordCompletionsGenerator CreateGenerator(string fileName)
        {
            using (StreamReader reader = File.OpenText(fileName))
                return WordCompletionsGeneratorFactory.CreateFromTextReader(reader,
                    WordCompletionsGeneratorThreadSafetyMode.ThreadSafe);
        }

        /// <summary>
        /// Запустить сервер.
        /// </summary>
        private void StartServer()
        {
            Logger.WriteInfo(Resources.ServerStart);
            this.server = new Server(CreateGenerator(dictionaryFileName), portNumber);
            this.server.Start();
            Logger.WriteInfo(Resources.ServerStartCompleted);
        }

        /// <summary>
        /// Остановить сервер.
        /// </summary>
        private void StopServer()
        {
            this.server.Stop();
        }

        /// <summary>
        /// Показать справку.
        /// </summary>
        private static void ShowHelp()
        {
            Logger.WriteWarning(Resources.CommandLineHelp);
        }

        /// <summary>
        /// Выполнить сервер загрузку сервера в нужном режиме.
        /// </summary>
        private void Execute()
        {
            switch (this.mode)
            {
                case ServerMode.NormalStart:
                    try
                    {
                        StartServer();
                        Console.In.ReadLine();
                    }
                    finally
                    {
                        StopServer();
                    }
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }

        /// <summary>
        /// Создать экземпляр класса загрузки сервера автодополнений слов.
        /// </summary>
        /// <param name="args">Параметры командной строки.</param>
        private Loader(string[] args)
        {
            ParseCommandLine(args);
        }

        static void Main(string[] args)
        {
            try
            {
                new Loader(args).Execute();
            }
            catch (Exception e)
            {
                Logger.WriteError(e.Message);
                Environment.Exit(-1);
            }
        }
    }
}
