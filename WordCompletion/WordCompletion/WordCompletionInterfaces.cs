using System;
using System.Collections.Generic;
using System.IO;

namespace WordCompletions
{
    public interface IWordCompletion : IComparable<IWordCompletion>
    {
        int Frequency { get; }
        string Word { get; }
    }

    public interface IWordCompletions
    {
        IEnumerable<IWordCompletion> GetAllCompletions(string wordToComplete);
        IEnumerable<IWordCompletion> GetTenBestCompletions(string wordToComplete);
    }

    public partial class WordCompletionBuilder
    {
        public static IWordCompletions CreateFromStream(TextReader input)
        {
            return new WordCompletions(input);
        }
    }
}
