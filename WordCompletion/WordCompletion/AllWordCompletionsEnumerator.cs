using System;
using System.Collections.Generic;

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
            return string.Compare(this.dictionary[index].Word, 0, this.wordToEnumerate, 0, this.wordToEnumerate.Length);
        }

        private int ClarifyBorder(int internalPoint, int outerPoint)
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

        /// <summary>
        /// Найти первый вариант автодополнения в словаре.
        /// Уточнить диапазон, внутри которого лежат остальные варианты автодополнения.
        /// Используется алгоритм двоичного поиска.
        /// Если вариант автодополнения не найден, то выходные значения leftBorder, rightBorder и completionPosition не имеют смысла.
        /// </summary>
        /// <param name="leftBorder">Левая граница диапазона словаря, внутри которого лежат все варианты автодополнения.</param>
        /// <param name="rightBorder">Правая граница диапазона словаря, внутри которого лежат все варианты автодополнения.</param>
        /// <param name="completionPosition">Позиция найденного варианта автодополнения.</param>
        /// <returns>Признак того, найден ли вариант автодополнения.</returns>
        private bool BinarySearchForFirstCompletion(out int leftBorder, out int rightBorder, out int completionPosition)
        {
            if (this.dictionary.Count == 0)
            {
                leftBorder = UndefinedIndex;
                rightBorder = UndefinedIndex;
                completionPosition = UndefinedIndex;
                return false;
            }

            leftBorder = 0;
            rightBorder = this.dictionary.Count - 1;
            int compareResult;

            do
            {
                completionPosition = (leftBorder + rightBorder) / 2;
                compareResult = Compare(completionPosition);
                if (compareResult == 0)
                    break;
                if (compareResult < 0)
                    leftBorder = completionPosition;
                else
                    rightBorder = completionPosition;
            } while (rightBorder - leftBorder > 1);

            if (compareResult != 0)
            {
                if (leftBorder == 0)
                {
                    completionPosition = leftBorder;
                    compareResult = Compare(completionPosition);
                }
                else
                    if (rightBorder == dictionary.Count - 1)
                    {
                        completionPosition = rightBorder;
                        compareResult = Compare(completionPosition);
                    }
            }
            return compareResult == 0;
        }

        private bool FindCompletions(out int firstCompletionIndex, out int lastCompletionIndex)
        {
            int leftBorder;
            int rightBorder;
            int completionPosition;

            if (BinarySearchForFirstCompletion(out leftBorder, out rightBorder, out completionPosition))
            {
                firstCompletionIndex = ClarifyBorder(completionPosition, leftBorder);
                lastCompletionIndex = ClarifyBorder(completionPosition, rightBorder);
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
