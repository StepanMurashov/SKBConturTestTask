﻿using System;
using System.Collections.Generic;

namespace Sten.WordCompletions
{
    /// <summary>
    /// Генератор наилучших вариантов автодополнения.
    /// </summary>
    internal class TenBestCompletionsGenerator : IEnumerable<IWordCompletion>
    {
        /// <summary>
        /// Сгенерированные наилучшие варианты автодополнения.
        /// </summary>
        private Lazy<List<IWordCompletion>> bestCompletions;

        /// <summary>
        /// Все возможные варианты автодополнения, из которых будут выбираться лучшие.
        /// </summary>
        private IEnumerable<IWordCompletion> allCompletions;

        /// <summary>
        /// Выбрать наилучшие из вариантов автодополнения.
        /// </summary>
        /// <param name="completions">Все варианты автодополнения.</param>
        /// <returns>Выбранные варианты автодополнения.</returns>
        private List<IWordCompletion> GenerateBestCompletions()
        {
            const int BestCompletionsMaxCount = 10;
            List<IWordCompletion> generatedCompletions = new List<IWordCompletion>();
            foreach (IWordCompletion completion in this.allCompletions)
            {
                if (generatedCompletions.Count == BestCompletionsMaxCount)
                {
                    // Оптимизация. Если в списке 10 элементов, и completion должен встать на 11 место,
                    // то completion можно не обрабатывать. Экономим операцию двоичного поиска.
                    if (completion.CompareTo(generatedCompletions[generatedCompletions.Count - 1]) > 0)
                        continue;
                    else
                        generatedCompletions.RemoveAt(BestCompletionsMaxCount - 1);
                }

                generatedCompletions.Insert(~generatedCompletions.BinarySearch(completion), completion);
            }
            return generatedCompletions;
        }

        /// <summary>
        /// Создать генератор наилучших вариантов автодополнения.
        /// </summary>
        /// <param name="allCompletions">Все возможные варианты автодополнения.</param>
        public TenBestCompletionsGenerator(IEnumerable<IWordCompletion> allCompletions)
        {
            this.allCompletions = allCompletions;
            this.bestCompletions = new Lazy<List<IWordCompletion>>(GenerateBestCompletions);
        }

        #region Реализация интерфейса IEnumerable<IWordCompletion>.
        
        public IEnumerator<IWordCompletion> GetEnumerator()
        {
            return this.bestCompletions.Value.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
