using System;
using System.Diagnostics;

namespace WordCompletions
{
    /// <summary>
    /// Вариант автодополнения.
    /// </summary>
    internal class WordCompletion : IWordCompletion
    {
        /// <summary>
        /// Слово.
        /// </summary>
        private string word;

        /// <summary>
        /// Частота слова в текстах.
        /// </summary>
        private int frequency;

        /// <summary>
        /// Инициализировать вариант автодополнения.
        /// </summary>
        /// <param name="word">Слово.</param>
        /// <param name="frequency">Частота в текстах.</param>
        public WordCompletion(string word, int frequency)
        {
            this.word = word;
            this.frequency = frequency;
        }

        #region Реализация интерфейса IWordCompletion.
        
        public int Frequency
        {
            get { return this.frequency; }
        }

        public string Word
        {
            get { return this.word; }
        }

        public int CompareTo(IWordCompletion other)
        {
            int Result = this.frequency.CompareTo(other.Frequency);
            if (Result == 0)
                return String.Compare(this.Word, other.Word, StringComparison.Ordinal);
            else
                return -Result;
        }

        #endregion
    }
}
