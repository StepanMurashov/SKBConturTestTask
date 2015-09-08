using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCompletions
{
    /// <summary>
    /// Перечислитель вариантов автодополнения.
    /// Для заданного слова подбирает все варианты автодополнения из заданного словаря.
    /// </summary>
    internal class AllWordCompletionsEnumerator : IEnumerable<IWordCompletion>, IEnumerator<IWordCompletion>
    {
        private const int UndefinedIndex = -1;
        private string wordToEnumerate;
        private WordCompletionsGenerator dictionary;
        private bool isInitialized = false;
        private int position = UndefinedIndex;
        private int firstIndex = UndefinedIndex;
        private int lastIndex = UndefinedIndex;

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
                firstCompletionIndex = UndefinedIndex;
                lastCompletionIndex = UndefinedIndex;
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
                firstCompletionIndex = UndefinedIndex;
                lastCompletionIndex = UndefinedIndex;
                return false;
            }
        }

        public AllWordCompletionsEnumerator(WordCompletionsGenerator dictionary, string wordToEnumerate)
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

            if (!isInitialized)
            {
                result = FindCompletions(out this.firstIndex, out this.lastIndex);
                this.position = this.firstIndex;
                this.isInitialized = true;
                return result;
            }

            result =
                (this.position >= this.firstIndex) &&
                (this.position < this.lastIndex);
            if (result)
                this.position++;
            return result;
        }

        public void Reset()
        {
            this.isInitialized = false;
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
}
