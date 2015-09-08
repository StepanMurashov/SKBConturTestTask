using System;
using System.Collections.Generic;
using System.IO;

namespace WordCompletions
{
    internal class WordCompletionsGenerator : List<IWordCompletion>, IWordCompletionsGenerator
    {
        public WordCompletionsGenerator(TextReader input)
        {
            Logger.WriteVerbose("Dictionary loading started.\n");
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
                        Logger.WriteWarning(string.Format("Frequency parsing failed. Frequency: {0}.\n", completionParts[completionFrequencyIndex]));
                else
                    Logger.WriteWarning(string.Format("Dictionary string parsing failed. Input string: {0}.\n", inputString));
            }
            Logger.WriteVerbose(string.Format("Dictionary loading completed. {0} words loaded.\n", this.Count));
            Logger.WriteVerbose("Dictionary sorting started.\n");
            this.Sort(delegate(IWordCompletion left, IWordCompletion right)
            {
                return left.Word.CompareTo(right.Word);
            });
            Logger.WriteVerbose("Dictionary sorting completed.\n");
        }

        public IEnumerable<IWordCompletion> GetAllCompletions(string wordToComplete)
        {
            return new AllWordCompletionsEnumerator(this, wordToComplete);
        }

        public IEnumerable<IWordCompletion> GetTenBestCompletions(string wordToComplete)
        {
            return new TenBestCompletionsGenerator(this, wordToComplete);
        }
    }
}
