using System;

namespace WordCompletions
{
    public partial class WordCompletionBuilder
    {
        private struct WordCompletion : IWordCompletion
        {
            private string word;
            private int frequency;
            public WordCompletion(string word, int frequency)
            {
                this.word = word;
                this.frequency = frequency;
            }

            private int CompareByWord(IWordCompletion other)
            {
                return String.Compare(this.Word, other.Word);
            }

            public int Frequency
            {
                get { return this.frequency; }
            }

            public string Word
            {
                get { return this.word; }
            }

            public int CompareTo(IWordCompletion other)
            {
                int Result = this.frequency.CompareTo(other.Frequency);
                if (Result == 0)
                    return CompareByWord(other);
                else
                    return -Result;
            }
        }
    }
}
