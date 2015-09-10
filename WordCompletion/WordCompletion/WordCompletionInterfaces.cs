using System;
using System.Collections.Generic;
using System.IO;

[assembly: CLSCompliant(true)]
namespace Sten.WordCompletions.Library
{
    /// <summary>
    /// Режимы потокобезопасности генератора вариантов автодополнения слов.
    /// </summary>
    public enum WordCompletionsGeneratorThreadSafetyMode {None, ThreadSafe};

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
        /// Получить не более 10 лучших вариантов автодополнения для слова.
        /// </summary>
        /// <param name="wordToComplete">Слово для автодополнения.</param>
        /// <returns>10 лучших вариантов автодополнения для слова.</returns>
        IEnumerable<IWordCompletion> GetTenBestCompletions(string wordToComplete);
    }

    /// <summary>
    /// Фабрика генераторов автодополнений слов.
    /// </summary>
    static public class WordCompletionsGeneratorFactory
    {
        /// <summary>
        /// Создать генератор автодополнений из потока ввода.
        /// </summary>
        /// <param name="input">Поток ввода.</param>
        /// <param name="threadSafetyMode">Режим потокобезопасности генератора.</param>
        /// <returns>Генератор автодополнений для слов.</returns>
        public static IWordCompletionsGenerator CreateFromTextReader(TextReader input, WordCompletionsGeneratorThreadSafetyMode threadSafetyMode)
        {
            switch (threadSafetyMode)
            {
                case WordCompletionsGeneratorThreadSafetyMode.None:
                    return new WordCompletionsGenerator(input);
                case WordCompletionsGeneratorThreadSafetyMode.ThreadSafe:
                    return new ThreadSafeWordCompletionsGenerator(input);
                default:
                    throw new ArgumentOutOfRangeException("threadSafetyMode");
            }
            
        }
    }
}
