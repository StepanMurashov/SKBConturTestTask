using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCompletionGenerator
{
    // TODO: Области видимости и namespace.
    public struct WordCompletion
    {
        // TODO: С большой или маленькой?
        private string value;
        public string Value 
        { 
            get { return value; } 
        }
        private int frequency;
        public WordCompletion(string value, int frequency)
        {
            this.value = value;
            this.frequency = frequency;
        }
        public static int CompareByWord(WordCompletion leftItem, WordCompletion rightItem)
        {
            return String.Compare(leftItem.Value, rightItem.Value);
        }
        public static int CompareForTop10(WordCompletion leftItem, WordCompletion rightItem)
        {
            int Result = leftItem.frequency.CompareTo(rightItem.frequency);
            if (Result == 0)
                return CompareByWord(leftItem, rightItem);
            else
                return -Result;
        }
    }

    public interface IWordCompletionDictionary
    {
        IEnumerable<WordCompletion> GetAllCompletions(string WordToComplete);
        IEnumerable<WordCompletion> GetTop10Completions(string WordToComplete);
    }

    internal class WordCompletionDictionary : List<WordCompletion>, IWordCompletionDictionary
    {
        private class WordCompletionsEnumerator : IEnumerator<WordCompletion>, IEnumerable<WordCompletion>
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

            public WordCompletion Current
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
                Predicate<WordCompletion> BeginsWith = delegate(WordCompletion item)
                {
                    return (String.Compare(item.Value, 0, WordToEnumerate, 0, WordToEnumerate.Length) == 0);
                };

                this.Position = -1;
                this.FirstIndex = this.Dictionary.FindIndex(BeginsWith);
                if (this.FirstIndex < 0)
                    this.LastIndex = this.FirstIndex;
                else
                    this.LastIndex = this.Dictionary.FindLastIndex(BeginsWith);
            }

            public IEnumerator<WordCompletion> GetEnumerator()
            {
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this;
            }
        }
        private class Top10WordCompletionsEnumerator : IEnumerable<WordCompletion>
        {
            private IWordCompletionDictionary Dictionary;
            private string WordToEnumerate;
            private Lazy<List<WordCompletion>> Top10Completions;
            private List<WordCompletion> InitializeTop10List()
            {
                List<WordCompletion> Top10 = new List<WordCompletion>(this.Dictionary.GetAllCompletions(this.WordToEnumerate));
                Top10.Sort(WordCompletion.CompareForTop10);
                if (Top10.Count > 10)
                    Top10.RemoveRange(10, Top10.Count - 10);
                return Top10;
            }
            public Top10WordCompletionsEnumerator(IWordCompletionDictionary dictionary, string wordToEnumerate)
            {
                this.Dictionary = dictionary;
                this.WordToEnumerate = wordToEnumerate;
                this.Top10Completions = new Lazy<List<WordCompletion>>(this.InitializeTop10List);
            }

            public IEnumerator<WordCompletion> GetEnumerator()
            {
                return this.Top10Completions.Value.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public IEnumerable<WordCompletion> GetAllCompletions(string wordToComplete)
        {
            return new WordCompletionsEnumerator(this, wordToComplete);
        }
        public IEnumerable<WordCompletion> GetTop10Completions(string wordToComplete)
        {
            return new Top10WordCompletionsEnumerator(this, wordToComplete);
        }
    }


    public class DictionaryBuilder
    {
        public static IWordCompletionDictionary CreateFromStream(TextReader input)
        {
            WordCompletionDictionary dictionary = new WordCompletionDictionary();
            int DictionaryCount = int.Parse(input.ReadLine());
            for (int i = 0; i < DictionaryCount; i++)
            {
                const int completionWordIndex = 0;
                const int completionFrequencyIndex = 1;
                const int completionPartsCount = 2;
                string[] completionParts = input.ReadLine().Split(' ');
                int frequency;
                if (completionParts.Length == completionPartsCount)
                    if (int.TryParse(completionParts[completionFrequencyIndex], out frequency))
                        dictionary.Add(new WordCompletion(completionParts[completionWordIndex], frequency));
                // TODO: Писать в лог не считавшиеся строки.
            }
            dictionary.Sort(WordCompletion.CompareByWord);
            return dictionary;
        }
    }
}
