using System;

namespace WordCompletions
{
    /// <summary>
    /// Слово, которое можно использовать для автодополнения.
    /// </summary>
    internal struct WordCompletion : IWordCompletion
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
        /// Инициализировать новое слово для автодополнения.
        /// </summary>
        /// <param name="word">Слово.</param>
        /// <param name="frequency">Частота в текстах.</param>
        public WordCompletion(string word, int frequency)
        {
            this.word = word;
            this.frequency = frequency;
        }

        /// <summary>
        /// Сравнить варианты автодополнения по алфавитному порядку слов.
        /// </summary>
        /// <param name="other">Вариант автодополнения для сравнения.</param>
        /// <returns></returns>
        private int CompareByWord(IWordCompletion other)
        {
            return String.Compare(this.Word, other.Word);
        }

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
                return CompareByWord(other);
            else
                return -Result;
        }
    }
}
