using System;
using System.Collections.Generic;

namespace WordCompletions
{
    /// <summary>
    /// Генератор вариантов автодополнения.
    /// Для заданного слова подбирает все варианты автодополнения из заданного словаря.
    /// </summary>
    internal class AllWordCompletionsGenerator : IEnumerable<IWordCompletion>
    {
        /// <summary>
        /// Триплет, определяющий позицию в словаре.
        /// </summary>
        private class PositionTriplet
        {
            /// <summary>
            /// Индекс первого элемента.
            /// </summary>
            public int FirstIndex { get; set; }

            /// <summary>
            /// Индекс последнего элемента.
            /// </summary>
            public int LastIndex { get; set; }

            /// <summary>
            /// Индекс текущего элемента.
            /// </summary>
            public int CurrentIndex { get; set; }
        }

        /// <summary>
        /// Перечислитель всех вариантов автодополнения.
        /// </summary>
        private class AllWordCompletionsEnumerator: IEnumerator<IWordCompletion>
        {
            /// <summary>
            /// Признак инциализированности текущей позиции перечислителя.
            /// </summary>
            private bool isCurrentPositionInitialized = false;

            /// <summary>
            /// Позиция перечислителя.
            /// </summary>
            private PositionTriplet position;

            /// <summary>
            /// Словарь автодополнений.
            /// </summary>
            private WordCompletionsGenerator dictionary;

            /// <summary>
            /// Создать экземпляр перечислителя вариантов автодополнения.
            /// </summary>
            /// <param name="dictionary">Словарь с вариантами автодополнения.</param>
            /// <param name="enumeratorPosition">Позиция перечислителя. 
            /// FirstIndex и LastIndex должны содержать позиции первого и последнего вариантов автодополнения в словаре.
            /// CurrentIndex не используется.</param>
            public AllWordCompletionsEnumerator(WordCompletionsGenerator dictionary, PositionTriplet enumeratorPosition)
            {
                this.position = enumeratorPosition;
                this.dictionary = dictionary;
            }

            #region Реализация интерфейса IEnumerator<IWordCompletion>.
            
            public bool MoveNext()
            {
                if (!isCurrentPositionInitialized)
                {
                    this.position.CurrentIndex = this.position.FirstIndex;
                    isCurrentPositionInitialized = true;
                    return this.position.CurrentIndex != UndefinedIndex;
                }

                bool result =
                    (position.CurrentIndex >= position.FirstIndex) &&
                    (position.CurrentIndex < position.LastIndex);
                if (result)
                    position.CurrentIndex++;
                return result;
            }

            public IWordCompletion Current
            {
                get { return dictionary[position.CurrentIndex]; }
            }

            public void Dispose()
            {
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public void Reset()
            {
                this.isCurrentPositionInitialized = false;
            }

            #endregion
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
        private Lazy<AllWordCompletionsEnumerator> enumerator;

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
        private bool BinarySearchForFirstCompletion(PositionTriplet completionPosition)
        {
            if (this.dictionary.Count == 0)
            {
                completionPosition.FirstIndex = UndefinedIndex;
                completionPosition.LastIndex = UndefinedIndex;
                completionPosition.CurrentIndex = UndefinedIndex;
                return false;
            }

            completionPosition.FirstIndex = 0;
            completionPosition.LastIndex = this.dictionary.Count - 1;
            int compareResult;

            do
            {
                completionPosition.CurrentIndex = (completionPosition.FirstIndex + completionPosition.LastIndex) / 2;
                compareResult = Compare(completionPosition.CurrentIndex);
                if (compareResult == 0)
                    break;
                if (compareResult < 0)
                    completionPosition.FirstIndex = completionPosition.CurrentIndex;
                else
                    completionPosition.LastIndex = completionPosition.CurrentIndex;
            } while (completionPosition.LastIndex - completionPosition.FirstIndex > 1);

            if (compareResult != 0)
            {
                if (completionPosition.FirstIndex == 0)
                {
                    completionPosition.CurrentIndex = completionPosition.FirstIndex;
                    compareResult = Compare(completionPosition.CurrentIndex);
                }
                else
                    if (completionPosition.LastIndex == dictionary.Count - 1)
                    {
                        completionPosition.CurrentIndex = completionPosition.LastIndex;
                        compareResult = Compare(completionPosition.CurrentIndex);
                    }
            }
            return compareResult == 0;
        }

        /// <summary>
        /// Найти все варианты автодополнения слова в словаре.
        /// На основе найденных вариантов инициализировать позицию перечислителя.
        /// </summary>
        /// <returns>Инициализированная позиция перечислителя.</returns>
        private AllWordCompletionsEnumerator InitializePosition()
        {
            PositionTriplet position = new PositionTriplet();

            if (BinarySearchForFirstCompletion(position))
            {
                position.FirstIndex = ClarifyBorder(position.CurrentIndex, position.FirstIndex);
                position.LastIndex = ClarifyBorder(position.CurrentIndex, position.LastIndex);
            }
            else
            {
                position.FirstIndex = UndefinedIndex;
                position.LastIndex = UndefinedIndex;
            }
            return new AllWordCompletionsEnumerator(dictionary, position);
        }

        /// <summary>
        /// Создать экземлпяр перечислителя вариантов автодополнения слова.
        /// </summary>
        /// <param name="dictionary">Словарь вариантов автодополенения.</param>
        /// <param name="wordToComplete">Слово для автодополнения.</param>
        public AllWordCompletionsGenerator(WordCompletionsGenerator dictionary, string wordToComplete)
        {
            this.dictionary = dictionary;
            this.wordToComplete = wordToComplete;
            this.enumerator = new Lazy<AllWordCompletionsEnumerator>(InitializePosition);
        }

        #region Реализация интерфейса IEnumerable<IWordCompletion>.

        public IEnumerator<IWordCompletion> GetEnumerator()
        {
            return this.enumerator.Value;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
