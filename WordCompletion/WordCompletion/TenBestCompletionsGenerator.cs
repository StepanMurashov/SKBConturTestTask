﻿using System.Collections.Generic;
using WordCompletions.Properties;

namespace WordCompletions
{
    /// <summary>
    /// Генератор наилучших вариантов автодополнения.
    /// </summary>
    internal class TenBestCompletionsGenerator : IEnumerable<IWordCompletion>
    {
        /// <summary>
        /// Сгенерированные наилучшие варианты автодополнения.
        /// </summary>
        private List<IWordCompletion> bestCompletions;

        /// <summary>
        /// Выбрать наилучшие из вариантов автодополнения.
        /// </summary>
        /// <param name="completions">Все варианты автодополнения.</param>
        /// <returns>Выбранные варианты автодополнения.</returns>
        private List<IWordCompletion> GenerateBestCompletions(IEnumerable<IWordCompletion> completions)
        {
            const int BestCompletionsMaxCount = 10;
            List<IWordCompletion> bestCompletions = new List<IWordCompletion>();
            foreach (IWordCompletion completion in completions)
            {
                if (bestCompletions.Count == BestCompletionsMaxCount)
                {
                    // Оптимизация. Если в списке 10 элементов, и completion должен встать на 11 место,
                    // то completion можно не обрабатывать. Экономим операцию двоичного поиска.
                    if (completion.CompareTo(bestCompletions[bestCompletions.Count - 1]) > 0)
                        continue;
                    else
                        bestCompletions.RemoveAt(BestCompletionsMaxCount - 1);
                }

                bestCompletions.Insert(~bestCompletions.BinarySearch(completion), completion);
            }
            return bestCompletions;
        }

        /// <summary>
        /// Создать генератор наилучших вариантов автодополнения.
        /// </summary>
        /// <param name="dictionary">Словарь вариантов автодополнения.</param>
        /// <param name="wordToComplete">Слово, для которого следует генерировать варианты.</param>
        public TenBestCompletionsGenerator(IWordCompletionsGenerator dictionary, string wordToComplete)
        {
            this.bestCompletions = this.GenerateBestCompletions(dictionary.GetAllCompletions(wordToComplete));
            if (this.bestCompletions.Count == 0)
                Logger.WriteVerbose(string.Format(Resources.ZeroCompletionsFound, wordToComplete));
        }

        /// <summary>
        /// Получить перечислитель лучших вариантов автодополнения.
        /// </summary>
        /// <returns>Перечислитель лучших вариантов автодополнения.</returns>
        public IEnumerator<IWordCompletion> GetEnumerator()
        {
            return this.bestCompletions.GetEnumerator();
        }

        /// <summary>
        /// Получить перечислитель лучших вариантов автодополнения.
        /// </summary>
        /// <returns>Перечислитель лучших вариантов автодополнения.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
