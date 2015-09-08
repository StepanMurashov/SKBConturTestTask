using System;
using System.Collections.Generic;
using System.IO;

namespace WordCompletions
{
    /// <summary>
    /// Перечислитель вариантов автодополнения.
    /// Для заданного слова выбирает все варианты автодополнения из заданного словаря.
    /// </summary>
    internal class WordCompletionsEnumerator : IEnumerable<IWordCompletion>, IEnumerator<IWordCompletion>
    {
        private string wordToEnumerate;
        private WordCompletionsGenerator dictionary;
        private int position = -1;
        private int firstIndex = -1;
        private int lastIndex = -1;

        private int Compare(int index)
        {
            return string.Compare(dictionary[index].Word, 0, wordToEnumerate, 0, wordToEnumerate.Length);
        }

        private int FindBorder(int internalPoint, int outerPoint)
        {
            while (Math.Abs(outerPoint - internalPoint) > 1)
            {
                int testPoint = (internalPoint + outerPoint) / 2;
                if (Compare(testPoint) == 0)
                    internalPoint = testPoint;
                else
                    outerPoint = testPoint;
            }
            if (Compare(outerPoint) == 0)
                return outerPoint;
            else
                return internalPoint;
        }

        private bool FindCompletions(out int firstCompletionIndex, out int lastCompletionIndex)
        {
            if (dictionary.Count == 0)
            {
                firstCompletionIndex = -1;
                lastCompletionIndex = -1;
                return false;
            }
            int left = 0;
            int right = dictionary.Count - 1;
            int testPoint;
            int compareResult;

            // Двоичным поиском ищем первое подходящее автодополнение.
            do
            {
                testPoint = (left + right) / 2;
                compareResult = Compare(testPoint);
                if (compareResult == 0)
                    break;
                if (compareResult < 0)
                    left = testPoint;
                else
                    right = testPoint;
            } while (right - left > 1);

            if (compareResult != 0)
            {
                if (left == 0)
                {
                    testPoint = left;
                    compareResult = Compare(testPoint);
                }
                else
                    if (right == dictionary.Count - 1)
                    {
                        testPoint = right;
                        compareResult = Compare(testPoint);
                    }
            }

            if (compareResult == 0)
            {
                firstCompletionIndex = FindBorder(testPoint, left);
                lastCompletionIndex = FindBorder(testPoint, right);
                return true;
            }
            else
            {
                firstCompletionIndex = -2;
                lastCompletionIndex = -2;
                return false;
            }
        }

        public WordCompletionsEnumerator(WordCompletionsGenerator dictionary, string wordToEnumerate)
        {
            this.dictionary = dictionary;
            this.wordToEnumerate = wordToEnumerate;
        }

        public IWordCompletion Current
        {
            get { return dictionary[position]; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            bool result;
            if (this.position == -1)
            {
                result = FindCompletions(out firstIndex, out lastIndex);
                position = firstIndex;
                return result;
            }
            result =
                (this.position >= firstIndex) &&
                (this.position < lastIndex);
            if (result)
                this.position++;
            return result;
        }

        public void Reset()
        {
            this.position = -1;
        }

        public IEnumerator<IWordCompletion> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }
    }

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

    internal class WordCompletionsGenerator : List<IWordCompletion>, IWordCompletionsGenerator
    {
        public WordCompletionsGenerator(TextReader input)
        {
            Logger.WriteVerbose("Dictionary loading started.\n");
            int dictionaryCount = int.Parse(input.ReadLine());
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
                        Logger.WriteWarning(string.Format("Frequency parsing failed. Frequency: {0}.\n", completionParts[completionFrequencyIndex]));
                else
                    Logger.WriteWarning(string.Format("Dictionary string parsing failed. Input string: {0}.\n", inputString));
            }
            Logger.WriteVerbose(string.Format("Dictionary loading completed. {0} words loaded.\n", this.Count));
            Logger.WriteVerbose("Dictionary sorting started.\n");
            this.Sort(delegate(IWordCompletion left, IWordCompletion right)
            {
                return left.Word.CompareTo(right.Word);
            });
            Logger.WriteVerbose("Dictionary sorting completed.\n");
        }

        public IEnumerable<IWordCompletion> GetAllCompletions(string wordToComplete)
        {
            return new WordCompletionsEnumerator(this, wordToComplete);
        }

        public IEnumerable<IWordCompletion> GetTenBestCompletions(string wordToComplete)
        {
            return new TenBestCompletionsGenerator(this, wordToComplete);
        }
    }
}
