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
        /// <summary>
        /// Позиция перечислителя.
        /// </summary>
        private class EnumeratorPosition
        {
            /// <summary>
            /// Индекс первого элемента для перечисления.
            /// </summary>
            public int FirstIndex { get; set; }

            /// <summary>
            /// Индекс последнего элемента для перечисления.
            /// </summary>
            public int LastIndex { get; set; }

            /// <summary>
            /// Текущая позиция перечислителя.
            /// </summary>
            public int CurrentPosition { get; set; }

            /// <summary>
            /// Сдвинуться на следующую позицию.
            /// </summary>
            /// <returns>Признак того, удалось ли выполнить сдвиг.</returns>
            public bool MoveNext()
            {
                bool result =
                    (CurrentPosition >= FirstIndex) &&
                    (CurrentPosition < LastIndex);
                if (result)
                    CurrentPosition++;
                return result;
            }
        }

        /// <summary>
        /// Значение неопределенного индекса.
        /// </summary>
        private const int UndefinedIndex = -1;

        /// <summary>
        /// Слово для автодополнения.
        /// </summary>
        private string wordToComplete;

        /// <summary>
        /// Словарь с вариантами автодополнения.
        /// </summary>
        private WordCompletionsGenerator dictionary;

        /// <summary>
        /// Позиция перечислителя.
        /// </summary>
        private Lazy<EnumeratorPosition> position;

        /// <summary>
        /// Сравнить вариант автозаполнения по заданному индексу в словаре со словом для автодополнения.
        /// </summary>
        /// <param name="index">Индекс в словаре вариантов автодополнения.</param>
        /// <returns>Результат сравнения.</returns>
        private int Compare(int index)
        {
            return string.Compare(this.dictionary[index].Word, 0, this.wordToComplete, 0, this.wordToComplete.Length, StringComparison.Ordinal);
        }

        /// <summary>
        /// Уточнить границу диапазона словаря, внутри которого лежат варианты автодополнения слова.
        /// Используется алгоритм двоичного поиска.
        /// </summary>
        /// <param name="internalPoint">Индекс варианта автодополнения, который точно входит в диапазон (т.е. точно подходит к слову).</param>
        /// <param name="outerPoint">Индекс варианта автодополнения, который скорее всего не входит в диапазон.</param>
        /// <returns>Индекс варианта автодополнения в списке, который является границей диапазона.</returns>
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
        /// Если вариант автодополнения не найден, то position остается заполнен мусором.
        /// </summary>
        /// <param name="completionPosition">Позиция перечислителя. Индекс первого и последнего элемента на выходе указывают на границы диапазона подходящих вариантов.
        /// CurrentPosition на выходе указывает индекс найденного дополнения.</param>
        /// <returns>Признак того, найден ли вариант автодополнения.</returns>
        private bool BinarySearchForFirstCompletion(EnumeratorPosition completionPosition)
        {
            if (this.dictionary.Count == 0)
            {
                completionPosition.FirstIndex = UndefinedIndex;
                completionPosition.LastIndex = UndefinedIndex;
                completionPosition.CurrentPosition = UndefinedIndex;
                return false;
            }

            completionPosition.FirstIndex = 0;
            completionPosition.LastIndex = this.dictionary.Count - 1;
            int compareResult;

            do
            {
                completionPosition.CurrentPosition = (completionPosition.FirstIndex + completionPosition.LastIndex) / 2;
                compareResult = Compare(completionPosition.CurrentPosition);
                if (compareResult == 0)
                    break;
                if (compareResult < 0)
                    completionPosition.FirstIndex = completionPosition.CurrentPosition;
                else
                    completionPosition.LastIndex = completionPosition.CurrentPosition;
            } while (completionPosition.LastIndex - completionPosition.FirstIndex > 1);

            if (compareResult != 0)
            {
                if (completionPosition.FirstIndex == 0)
                {
                    completionPosition.CurrentPosition = completionPosition.FirstIndex;
                    compareResult = Compare(completionPosition.CurrentPosition);
                }
                else
                    if (completionPosition.LastIndex == dictionary.Count - 1)
                    {
                        completionPosition.CurrentPosition = completionPosition.LastIndex;
                        compareResult = Compare(completionPosition.CurrentPosition);
                    }
            }
            return compareResult == 0;
        }

        /// <summary>
        /// Найти все варианты автодополнения слова в словаре.
        /// На основе найденных вариантов инициализировать позицию перечислителя.
        /// </summary>
        /// <returns>Инициализированная позиция перечислителя.</returns>
        private EnumeratorPosition InitializePosition()
        {
            EnumeratorPosition result = new EnumeratorPosition();

            if (BinarySearchForFirstCompletion(result))
            {
                result.FirstIndex = ClarifyBorder(result.CurrentPosition, result.FirstIndex);
                result.LastIndex = ClarifyBorder(result.CurrentPosition, result.LastIndex);
            }
            else
            {
                result.FirstIndex = UndefinedIndex;
                result.LastIndex = UndefinedIndex;
            }
            result.CurrentPosition = result.FirstIndex;
            return result;
        }

        /// <summary>
        /// Создать экземлпяр перечислителя вариантов автодополнения слова.
        /// </summary>
        /// <param name="dictionary">Словарь вариантов автодополенения.</param>
        /// <param name="wordToComplete">Слово для автодополнения.</param>
        public AllWordCompletionsEnumerator(WordCompletionsGenerator dictionary, string wordToComplete)
        {
            this.dictionary = dictionary;
            this.wordToComplete = wordToComplete;
            this.position = new Lazy<EnumeratorPosition>(InitializePosition);
        }

        #region Реализация интерфейса IEnumerator<IWordCompletion>.

        public IWordCompletion Current
        {
            get { return dictionary[position.Value.CurrentPosition]; }
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
            if (!this.position.IsValueCreated)
                return this.position.Value.CurrentPosition != UndefinedIndex;
            else
                return this.position.Value.MoveNext();
        }

        public void Reset()
        {
            this.position = new Lazy<EnumeratorPosition>(InitializePosition);
        }
        #endregion

        #region Реализация интерфейса IEnumerable<IWordCompletion>.

        public IEnumerator<IWordCompletion> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
