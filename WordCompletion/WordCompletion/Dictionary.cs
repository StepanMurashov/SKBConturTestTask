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
        private string value;
        private int frequency;
        public string Value
        {
            get { return value; }
        }
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
            private string wordToEnumerate;
            private WordCompletionDictionary dictionary;
            private int position = -1;
            private int firstIndex = -1;
            private int lastIndex = -1;

            int Compare(int index)
            {
                return string.Compare(dictionary[index].Value, 0, wordToEnumerate, 0, wordToEnumerate.Length);
            }

            private bool FindFirst(out int index)
            {
                int left = 0;
                int right = dictionary.Count - 1;
                while (right - left > 1)
                {
                    int testPoint = (left + right) / 2;
                    if (Compare(testPoint) < 0)
                        left = testPoint;
                    else
                        right = testPoint;
                }
                index = -2;
                if (Compare(left) == 0)
                    index = left;
                else
                    if (Compare(right) == 0)
                        index = right;
                return index >= 0;
            }

            private bool FindLast(out int index)
            {
                int left = 0;
                int right = dictionary.Count - 1;
                while (right - left > 1)
                {
                    int testPoint = (left + right) / 2;
                    if (Compare(testPoint) <= 0)
                        left = testPoint;
                    else
                        right = testPoint;
                }
                index = -2;
                if (Compare(right) == 0)
                    index = right;
                else
                    if (Compare(left) == 0)
                        index = left;
                return index >= 0;
            }

            public WordCompletionsEnumerator(WordCompletionDictionary dictionary, string wordToEnumerate)
            {
                this.dictionary = dictionary;
                this.wordToEnumerate = wordToEnumerate;
            }

            public WordCompletion Current
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
                bool Result;
                if (this.position == -1)
                {
                    Result = FindFirst(out firstIndex);
                    if (Result)
                        FindLast(out lastIndex);
                    position = firstIndex;
                    return Result;
                }
                Result = 
                    (this.position  >= firstIndex) &&
                    (this.position < lastIndex);
                if (Result)
                    this.position++;
                return Result;
            }

            public void Reset()
            {
                this.position = -1;
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
            Logger.WriteVerbose("Dictionary loading started.");
            WordCompletionDictionary dictionary = new WordCompletionDictionary();
            int DictionaryCount = int.Parse(input.ReadLine());
            for (int i = 0; i < DictionaryCount; i++)
            {
                const int completionWordIndex = 0;
                const int completionFrequencyIndex = 1;
                const int completionPartsCount = 2;
                string inputString = input.ReadLine();
                string[] completionParts = inputString.Split(' ');
                int frequency;
                if (completionParts.Length == completionPartsCount)
                    if (int.TryParse(completionParts[completionFrequencyIndex], out frequency))
                        dictionary.Add(new WordCompletion(completionParts[completionWordIndex], frequency));
                    else
                        Logger.WriteWarning(string.Format("Frequency parsing failed. Frequency: {0}.", completionParts[completionFrequencyIndex]));
                else
                    Logger.WriteWarning(string.Format("Dictionary string parsing failed. Input string: {0}.", inputString));
            }
            Logger.WriteVerbose(string.Format("Dictionary loading completed. {0} words loaded.", dictionary.Count));
            Logger.WriteVerbose("Dictionary sorting started.");
            dictionary.Sort(WordCompletion.CompareByWord);
            Logger.WriteVerbose("Dictionary sorting completed.");
            return dictionary;
        }
    }
}
