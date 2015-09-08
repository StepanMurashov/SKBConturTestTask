using System.Collections.Generic;
using System.Globalization;
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
        static private List<IWordCompletion> GenerateBestCompletions(IEnumerable<IWordCompletion> completions)
        {
            const int BestCompletionsMaxCount = 10;
            List<IWordCompletion> generatedCompletions = new List<IWordCompletion>();
            foreach (IWordCompletion completion in completions)
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
        /// <param name="dictionary">Словарь вариантов автодополнения.</param>
        /// <param name="wordToComplete">Слово, для которого следует генерировать варианты.</param>
        public TenBestCompletionsGenerator(IWordCompletionsGenerator dictionary, string wordToComplete)
        {
            this.bestCompletions = GenerateBestCompletions(dictionary.GetAllCompletions(wordToComplete));
            if (this.bestCompletions.Count == 0)
                Logger.WriteVerbose(string.Format(CultureInfo.CurrentCulture, Resources.ZeroCompletionsFound, wordToComplete));
        }

        #region Реализация интерфейса IEnumerable<IWordCompletion>.
        
        public IEnumerator<IWordCompletion> GetEnumerator()
        {
            return this.bestCompletions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
