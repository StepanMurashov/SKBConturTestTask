using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using WordCompletions.Properties;

namespace WordCompletions
{
    /// <summary>
    /// Генератор вариантов автодополнения для слов.
    /// </summary>
    internal class WordCompletionsGenerator : List<IWordCompletion>, IWordCompletionsGenerator
    {
        /// <summary>
        /// Кэш лучших вариантов автодополнения.
        /// </summary>
        SortedList<string, IEnumerable<IWordCompletion>> cache = new SortedList<string,IEnumerable<IWordCompletion>>();

        /// <summary>
        /// Создать экземпляр генератора вариантов автодополнения для слов.
        /// </summary>
        /// <param name="input">Входной поток с вариантами автодополнения.</param>
        public WordCompletionsGenerator(TextReader input)
        {
            Logger.WriteVerbose(Resources.DictionaryLoadingStarted);
            int dictionaryCount = int.Parse(input.ReadLine(), CultureInfo.CurrentCulture);
            for (int i = 0; i < dictionaryCount; i++)
            {
                const int completionWordIndex = 0;
                const int completionFrequencyIndex = 1;
                const int completionPartsCount = 2;
                string inputString = input.ReadLine();
                string[] completionParts = inputString.Split(' ');
                int frequency;
                if (completionParts.Length == completionPartsCount)
                    if (int.TryParse(completionParts[completionFrequencyIndex], out frequency))
                        this.Add(new WordCompletion(completionParts[completionWordIndex], frequency));
                    else
                        Logger.WriteWarning(string.Format(CultureInfo.CurrentCulture, Resources.FrequencyParsingFailed, completionParts[completionFrequencyIndex]));
                else
                    Logger.WriteWarning(string.Format(CultureInfo.CurrentCulture, Resources.DictionaryStringParsingFailed, inputString));
            }
            Logger.WriteVerbose(string.Format(CultureInfo.CurrentCulture, Resources.DictionaryLoadingCompleted, this.Count));
            Logger.WriteVerbose(Resources.DictionarySortingStarted);
            this.Sort(delegate(IWordCompletion left, IWordCompletion right)
            {
                return String.Compare(left.Word, right.Word, StringComparison.Ordinal);
            });
            Logger.WriteVerbose(Resources.DictionarySortingCompleted);
        }

        #region Реализация интерфейса IWordCompletionsGenerator.

        public IEnumerable<IWordCompletion> GetAllCompletions(string wordToComplete)
        {
            return new AllWordCompletionsGenerator(this, wordToComplete);
        }

        public IEnumerable<IWordCompletion> GetTenBestCompletions(string wordToComplete)
        {
            IEnumerable<IWordCompletion> result;
            if (!cache.TryGetValue(wordToComplete, out result))
            {
                result = new TenBestCompletionsGenerator(this, wordToComplete);
                cache.Add(wordToComplete, result);
            }
            return result;
        }

        #endregion
    }
}
