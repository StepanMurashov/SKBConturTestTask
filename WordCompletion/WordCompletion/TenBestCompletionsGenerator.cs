using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCompletions
{
    internal class TenBestCompletionsGenerator : IEnumerable<IWordCompletion>
    {
        private List<IWordCompletion> bestCompletions;

        private void GenerateBestCompletions(IEnumerable<IWordCompletion> completions)
        {
            const int BestCompletionsMaxCount = 10;
            bestCompletions = new List<IWordCompletion>();
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
        }

        public TenBestCompletionsGenerator(IWordCompletionsGenerator dictionary, string wordToComplete)
        {
            this.GenerateBestCompletions(dictionary.GetAllCompletions(wordToComplete));
            if (this.bestCompletions.Count == 0)
                Logger.WriteVerbose(string.Format("Zero completions found for word {0}.\n", wordToComplete));
        }

        public IEnumerator<IWordCompletion> GetEnumerator()
        {
            return this.bestCompletions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
