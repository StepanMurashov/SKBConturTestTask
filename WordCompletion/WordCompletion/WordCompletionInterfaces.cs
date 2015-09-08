using System;
using System.Collections.Generic;
using System.IO;

namespace WordCompletions
{
    /// <summary>
    /// Вариант автодополнения.
    /// </summary>
    public interface IWordCompletion : IComparable<IWordCompletion>
    {
        // Частота слова в текстах.
        int Frequency { get; }
        // Слово.
        string Word { get; }
    }

    /// <summary>
    /// Генератор автодополнений для слов.
    /// </summary>
    public interface IWordCompletionsGenerator
    {
        /// <summary>
        /// Получить все варианты автодополнения для слова.
        /// </summary>
        /// <param name="wordToComplete">Слово для автодополнения.</param>
        /// <returns></returns>
        IEnumerable<IWordCompletion> GetAllCompletions(string wordToComplete);

        /// <summary>
        /// Получить не более 10 лучших вариантов автодополнения для слова.
        /// </summary>
        /// <param name="wordToComplete">Слово для автодополнения.</param>
        /// <returns></returns>
        IEnumerable<IWordCompletion> GetTenBestCompletions(string wordToComplete);
    }

    /// <summary>
    /// Фабрика генераторов автодополнений слов.
    /// </summary>
    public class WordCompletionsGeneratorFactory
    {
        /// <summary>
        /// Создать генератор автодополнений из потока ввода.
        /// </summary>
        /// <param name="input">Поток ввода.</param>
        /// <returns></returns>
        public static IWordCompletionsGenerator CreateFromTextReader(TextReader input)
        {
            return new WordCompletionsGenerator(input);
        }
    }
}
