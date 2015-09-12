using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sten.WordCompletions.Server
{
    /// <summary>
    /// Допустимые команды.
    /// </summary>
    public enum Command { Get, Answer, Shutdown };

    /// <summary>
    /// Команда для обмена по TCP/IP с сервером автодополнения слов.
    /// </summary>
    class WordCompletionsTCPCommand
    {
        /// <summary>
        /// Разделитель частей команды.
        /// </summary>
        private const string CommandDelimiter = " ";

        /// <summary>
        /// Формат команды.
        /// </summary>
        private const string CommandFormat = "{0}" + CommandDelimiter + "{1}";

        /// <summary>
        /// Префикс команды.
        /// </summary>
        private string commandPrefix;

        /// <summary>
        /// Словарь со всеми возможными командами.
        /// </summary>
        private static Dictionary<Command, WordCompletionsTCPCommand> commands =
            new Dictionary<Command, WordCompletionsTCPCommand>();

        /// <summary>
        /// Проверить, начинается ли строка команды с правильного префикса команды.
        /// </summary>
        /// <param name="command">Команда.</param>
        /// <returns>Признак того, начинается ли строка команды с правильного префикса команды.</returns>
        private bool CheckCommandPrefix(string command)
        {
            return command.StartsWith(commandPrefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Сконвертировать данные команды из массива байт в строку.
        /// </summary>
        /// <param name="command">Буфер с байтами команды.</param>
        /// <param name="commandLength">Длина буфера.</param>
        /// <returns>Команда в виде строки.</returns>
        private static string ConvertCommandDataToString(byte[] command, int commandLength)
        {
            return Encoding.ASCII.GetString(command, 0, commandLength);
        }

        /// <summary>
        /// Построить команду.
        /// </summary>
        /// <param name="data">Данные для отправки в команде.</param>
        /// <returns>Готовая к отправке команда.</returns>
        public byte[] Build(string data)
        {
            return Encoding.ASCII.GetBytes(string.Format(CultureInfo.InvariantCulture, CommandFormat, this.commandPrefix, data));
        }

        /// <summary>
        /// Разобрать команду и извлечь из нее данные.
        /// </summary>
        /// <param name="command">Буфер с байтами команды.</param>
        /// <param name="commandLength">Длина буфера.</param>
        /// <returns>Данные, пришедшие с командой, в виде строки.</returns>
        public string Parse(byte[] command, int commandLength)
        {
            string result = ConvertCommandDataToString(command, commandLength);
            if (!CheckCommandPrefix(result))
                throw new ArgumentOutOfRangeException("command");
            return result.Substring(commandPrefix.Length + CommandDelimiter.Length);
        }

        /// <summary>
        /// Определить, содержит ли буфер текущую команду.
        /// </summary>
        /// <param name="command">Буфер с байтами команды.</param>
        /// <param name="commandLength">Длина буфера.</param>
        /// <returns>Признак того, что в буфере содержится текущая команда.</returns>
        public bool TryParse(byte[] command, int commandLength)
        {
            string commandData = ConvertCommandDataToString(command, commandLength);
            return CheckCommandPrefix(commandData);
        }

        /// <summary>
        /// Создать экземпляр класса команды.
        /// </summary>
        /// <param name="commandPrefix"></param>
        private WordCompletionsTCPCommand(string commandPrefix)
        {
            this.commandPrefix = commandPrefix;
        }

        /// <summary>
        /// Инициализировать словарь всех возможных команд.
        /// </summary>
        static WordCompletionsTCPCommand()
        {
            commands.Add(Command.Get, new WordCompletionsTCPCommand("get"));
            commands.Add(Command.Answer, new WordCompletionsTCPCommand(""));
            commands.Add(Command.Shutdown, new WordCompletionsTCPCommand("shutdown"));
        }

        /// <summary>
        /// Получить команду.
        /// </summary>
        /// <param name="command">Тип команды.</param>
        /// <returns>Экземпляр класса для обработки команды.</returns>
        public static WordCompletionsTCPCommand GetCommand(Command command)
        {
            return commands[command];
        }
    }
}
