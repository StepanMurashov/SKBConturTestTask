using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCompletion
{
    // TODO: Области видимости и namespace.
    public struct WordCompletionDictionaryItem
    {
        // TODO: С большой или маленькой?
        public string Word;
        public int Frequency;
        public WordCompletionDictionaryItem(String word, int frequency)
        {
            this.Word = word;
            this.Frequency = frequency;
        }
        public static int CompareByWord(WordCompletionDictionaryItem leftItem, WordCompletionDictionaryItem rightItem)
        {
            return String.Compare(leftItem.Word, rightItem.Word);
        }
        public static int CompareByFrequencyAndWord(WordCompletionDictionaryItem leftItem, WordCompletionDictionaryItem rightItem)
        {
            int Result = leftItem.Frequency.CompareTo(rightItem.Frequency);
            if (Result == 0)
                return CompareByWord(leftItem, rightItem);
            else
                return Result;
        }
    }

    public interface IWordCompletionDictionary
    {
        IEnumerable<WordCompletionDictionaryItem> GetAllCompletions(string WordToComplete);
        IEnumerable<WordCompletionDictionaryItem> GetTop10Completions(string WordToComplete);
    }

    internal class WordCompletionDictionary : List<WordCompletionDictionaryItem>, IWordCompletionDictionary
    {
        private class WordCompletionsEnumerator : IEnumerator<WordCompletionDictionaryItem>, IEnumerable<WordCompletionDictionaryItem>
        {
            private string WordToEnumerate;
            private WordCompletionDictionary Dictionary;
            private int Position = -1;
            private int FirstIndex = -1;
            private int LastIndex = -1;

            public WordCompletionsEnumerator(WordCompletionDictionary dictionary, string wordToEnumerate)
            {
                this.Dictionary = dictionary;
                this.WordToEnumerate = wordToEnumerate;
                this.Reset();
            }

            public WordCompletionDictionaryItem Current
            {
                get { return Dictionary[Position]; }
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
                bool Result = this.Position < this.FirstIndex;
                if (Result)
                    this.Position = this.FirstIndex;
                else
                {
                    Result = this.Position < this.LastIndex;
                    if (Result)
                        this.Position++;
                }
                return Result;
            }

            public void Reset()
            {
                Predicate<WordCompletionDictionaryItem> BeginsWith = delegate(WordCompletionDictionaryItem item)
                {
                    return (String.Compare(item.Word, 0, WordToEnumerate, 0, WordToEnumerate.Length) == 0);
                };

                this.Position = -1;
                this.FirstIndex = this.Dictionary.FindIndex(BeginsWith);
                if (this.FirstIndex < 0)
                    this.LastIndex = this.FirstIndex;
                else
                    this.LastIndex = this.Dictionary.FindLastIndex(BeginsWith);
            }

            public IEnumerator<WordCompletionDictionaryItem> GetEnumerator()
            {
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this;
            }
        }
        private class Top10WordCompletionsEnumerator : IEnumerable<WordCompletionDictionaryItem>
        {
            private IWordCompletionDictionary Dictionary;
            private string WordToEnumerate;
            private Lazy<List<WordCompletionDictionaryItem>> Top10Completions;
            private List<WordCompletionDictionaryItem> InitializeTop10List()
            {
                List<WordCompletionDictionaryItem> Top10 = new List<WordCompletionDictionaryItem>(this.Dictionary.GetAllCompletions(this.WordToEnumerate));
                Top10.Sort(WordCompletionDictionaryItem.CompareByFrequencyAndWord);
                if (Top10.Count > 10)
                    Top10.RemoveRange(10, Top10.Count - 10);
                return Top10;
            }
            public Top10WordCompletionsEnumerator(IWordCompletionDictionary dictionary, string wordToEnumerate)
            {
                this.Dictionary = dictionary;
                this.WordToEnumerate = wordToEnumerate;
                this.Top10Completions = new Lazy<List<WordCompletionDictionaryItem>>(this.InitializeTop10List);
            }

            public IEnumerator<WordCompletionDictionaryItem> GetEnumerator()
            {
                return this.Top10Completions.Value.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<WordCompletionDictionaryItem> GetAllCompletions(string wordToComplete)
        {
            return new WordCompletionsEnumerator(this, wordToComplete);
        }
        public IEnumerable<WordCompletionDictionaryItem> GetTop10Completions(string wordToComplete)
        {
            return new Top10WordCompletionsEnumerator(this, wordToComplete);
        }
    }


    public class DictionaryFactory
    {
        public static IWordCompletionDictionary CreateFromStream(TextReader dictionaryStream)
        {
            WordCompletionDictionary dictionary = new WordCompletionDictionary();
            // TODO: Реализовать загрузку.
            dictionary.Add(new WordCompletionDictionaryItem("aaa", 5));
            dictionary.Add(new WordCompletionDictionaryItem("abb", 3));
            dictionary.Add(new WordCompletionDictionaryItem("bb", 10));
            if (dictionary.Count <= 0)
                // TODO: Сделать свой класс исключения.
                // TODO: Подключить локализацию.
                throw new Exception("Dictionary is empty.");
            dictionary.Sort(WordCompletionDictionaryItem.CompareByWord);
            return dictionary;
        }
    }
}
