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
    /// Построитель команд для обмена по TCP/IP с сервером автодополнения слов.
    /// </summary>
    class WordCompletionsServerTCPCommandsBuilder
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
        /// Словарь со всеми возможными командами.
        /// </summary>
        private static Dictionary<Command, WordCompletionsServerTCPCommandsBuilder> commands =
            new Dictionary<Command, WordCompletionsServerTCPCommandsBuilder>()
            {
                {Command.Get, new WordCompletionsServerTCPCommandsBuilder("get")},
                {Command.Answer, new WordCompletionsServerTCPCommandsBuilder("")},
                {Command.Shutdown, new WordCompletionsServerTCPCommandsBuilder("shutdown")}
            };

        /// <summary>
        /// Префикс команды.
        /// </summary>
        private string commandPrefix;

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
            return Encoding.ASCII.GetBytes(
                string.Format(CultureInfo.InvariantCulture, CommandFormat, this.commandPrefix, data));
        }

        /// <summary>
        /// Определить, содержит ли буфер текущую команду.
        /// </summary>
        /// <param name="command">Буфер с байтами команды.</param>
        /// <param name="commandLength">Длина буфера.</param>
        /// <returns>Признак того, что в буфере содержится текущая команда.</returns>
        public bool TryParse(byte[] command, int commandLength)
        {
            return ConvertCommandDataToString(command, commandLength).
                StartsWith(commandPrefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Разобрать команду и извлечь из нее данные.
        /// </summary>
        /// <param name="command">Буфер с байтами команды.</param>
        /// <param name="commandLength">Длина буфера.</param>
        /// <returns>Данные, пришедшие с командой, в виде строки.</returns>
        public string Parse(byte[] command, int commandLength)
        {
            if (!TryParse(command, commandLength))
                throw new ArgumentOutOfRangeException("command");
            return ConvertCommandDataToString(command, commandLength).
                Substring(commandPrefix.Length + CommandDelimiter.Length);
        }

        /// <summary>
        /// Создать экземпляр класса команды.
        /// </summary>
        /// <param name="commandPrefix"></param>
        private WordCompletionsServerTCPCommandsBuilder(string commandPrefix)
        {
            this.commandPrefix = commandPrefix;
        }

        /// <summary>
        /// Получить команду.
        /// </summary>
        /// <param name="command">Тип команды.</param>
        /// <returns>Экземпляр класса для обработки команды.</returns>
        public static WordCompletionsServerTCPCommandsBuilder GetCommand(Command command)
        {
            return commands[command];
        }
    }
}
