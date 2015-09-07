using System;
using System.Collections.Generic;
using System.IO;

namespace WordCompletions
{
    public partial class WordCompletionBuilder
    {
        private class WordCompletionsEnumerator : IEnumerable<IWordCompletion>, IEnumerator<IWordCompletion>
        {
            private string wordToEnumerate;
            private WordCompletions dictionary;
            private int position = -1;
            private int firstIndex = -1;
            private int lastIndex = -1;

            private int Compare(int index)
            {
                return string.Compare(dictionary[index].Word, 0, wordToEnumerate, 0, wordToEnumerate.Length);
            }

            private bool Find(out int leftIndex, out int rightIndex)
            {
                if (dictionary.Count == 0)
                {
                    leftIndex = -1;
                    rightIndex = -1;
                    return false;
                }
                int left = 0;
                int right = dictionary.Count - 1;
                int testPoint;
                int compareResult;
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
                if (compareResult == 0)
                {
                    leftIndex = left;
                    rightIndex = right;
                    right = testPoint;
                    int testPoint2;
                    while (right - leftIndex > 1)
                    {
                        testPoint2 = (leftIndex + right) / 2;
                        compareResult = Compare(testPoint2);
                        if (compareResult < 0)
                            leftIndex = testPoint2;
                        else
                            right = testPoint2;
                    }
                    if (Compare(leftIndex) != 0)
                        leftIndex = right;
                    left = testPoint;
                    while (rightIndex - left > 1)
                    {
                        testPoint2 = (left + rightIndex) / 2;
                        compareResult = Compare(testPoint2);
                        if (compareResult <= 0)
                            left = testPoint2;
                        else
                            rightIndex = testPoint2;
                    }
                    if (Compare(rightIndex) != 0)
                        rightIndex = left;
                    return true;
                }
                else
                {
                    leftIndex = -2;
                    rightIndex = -2;
                    return false;
                }
            }

            public WordCompletionsEnumerator(WordCompletions dictionary, string wordToEnumerate)
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
                    result = Find(out firstIndex, out lastIndex);
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

        private class TenBestCompletionsGenerator : IEnumerable<IWordCompletion>
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
                        // то completion можно не обрабатывать.
                        // Если же completion попадает в список - из списка точно вылетает последний элемент.
                        if (completion.CompareTo(bestCompletions[bestCompletions.Count - 1]) > 0)
                            continue;
                        else
                            bestCompletions.RemoveAt(BestCompletionsMaxCount - 1);
                    }

                    bestCompletions.Insert(~bestCompletions.BinarySearch(completion), completion);
                }
            }

            public TenBestCompletionsGenerator(IWordCompletions dictionary, string wordToComplete)
            {
                this.GenerateBestCompletions(dictionary.GetAllCompletions(wordToComplete));
                if (this.bestCompletions.Count == 0)
                    Logger.WriteVerbose(string.Format("Zero completions found for word {0}", wordToComplete));
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

        private class WordCompletions : List<IWordCompletion>, IWordCompletions
        {
            public WordCompletions(TextReader input)
            {
                Logger.WriteVerbose("Dictionary loading started.");
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
                            Logger.WriteWarning(string.Format("Frequency parsing failed. Frequency: {0}.", completionParts[completionFrequencyIndex]));
                    else
                        Logger.WriteWarning(string.Format("Dictionary string parsing failed. Input string: {0}.", inputString));
                }
                Logger.WriteVerbose(string.Format("Dictionary loading completed. {0} words loaded.", this.Count));
                Logger.WriteVerbose("Dictionary sorting started.");
                this.Sort(delegate(IWordCompletion left, IWordCompletion right)
                {
                    return left.Word.CompareTo(right.Word);
                });
                Logger.WriteVerbose("Dictionary sorting completed.");
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
}
