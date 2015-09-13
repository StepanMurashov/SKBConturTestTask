using System;
using System.Net.Sockets;
using System.Text;

namespace Sten.WordCompletions.Server
{
    /// <summary>
    /// Клиент, подключенный к серверу генерации вариантов автодополнений слов.
    /// </summary>
    internal class Client
    {
        /// <summary>
        /// Сокет для обмена командами с клиентом.
        /// </summary>
        private Socket clientSocket;

        /// <summary>
        /// Буфер для команд, получаемых от клиента.
        /// </summary>
        private byte[] buffer = new byte[1024];

        /// <summary>
        /// Генератор автодополнений слов.
        /// </summary>
        private IWordCompletionsGenerator wordCompletionsGenerator;

        /// <summary>
        /// Получить строку с вариантами автодополнения слова.
        /// </summary>
        /// <param name="wordToComplete">Слово для автодополнения.</param>
        /// <returns>Строка с вариантами автодополнения.</returns>
        private string GetCompletionsString(string wordToComplete)
        {
            StringBuilder result = new StringBuilder();
            foreach (IWordCompletion completion in this.wordCompletionsGenerator.GetTenBestCompletions(wordToComplete))
                result.AppendLine(completion.Word);
            result.AppendLine();
            return result.ToString();
        }

        /// <summary>
        /// Завершить отправку данных клиенту.
        /// </summary>
        /// <param name="ar">Нужен для соответствия прототипу AsyncCallback.</param>
        private void EndSend(IAsyncResult ar)
        {
            this.clientSocket.EndSend(ar);
            BeginReceive();
        }

        /// <summary>
        /// Завершить получение данных от клиента.
        /// </summary>
        /// <param name="ar">Нужен для соответствия прототипу AsyncCallback.</param>
        private void EndReceive(IAsyncResult ar)
        {
            int bytesRead = this.clientSocket.EndReceive(ar);
            if (!TCPCommandsBuilder.ShutdownCommand.TryParse(this.buffer, bytesRead))
            {
                string wordToComplete = TCPCommandsBuilder.GetCompletionsCommand.Parse(this.buffer, bytesRead);
                byte[] answer = TCPCommandsBuilder.AnswerCommand.Build(
                    GetCompletionsString(wordToComplete));
                this.clientSocket.BeginSend(answer, 0, answer.Length, 0, EndSend, null);
            }
            else
            {
                this.clientSocket.Shutdown(SocketShutdown.Both);
                this.clientSocket.Close();
            }
        }

        /// <summary>
        /// Начать получение данных от клиента.
        /// </summary>
        private void BeginReceive()
        {
            this.clientSocket.BeginReceive(this.buffer, 0, this.buffer.Length, 0, EndReceive, null);
        }

        /// <summary>
        /// Создать экземпляр класса клиента.
        /// </summary>
        /// <param name="clientSocket">Сокет для обмена командами с клиентом.</param>
        /// <param name="wordCompletionsGenerator">Генератор автодополнений слов.</param>
        public Client(Socket clientSocket, IWordCompletionsGenerator wordCompletionsGenerator)
        {
            this.clientSocket = clientSocket;
            this.wordCompletionsGenerator = wordCompletionsGenerator;
            BeginReceive();
        }

    }
}
