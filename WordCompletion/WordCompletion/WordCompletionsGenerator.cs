using System.Collections.Generic;
using System.IO;
using WordCompletions.Properties;

namespace WordCompletions
{
    internal class WordCompletionsGenerator : List<IWordCompletion>, IWordCompletionsGenerator
    {
        SortedList<string, IEnumerable<IWordCompletion>> cache = new SortedList<string,IEnumerable<IWordCompletion>>();
        public WordCompletionsGenerator(TextReader input)
        {
            Logger.WriteVerbose(Resources.DictionaryLoadingStarted);
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
                        Logger.WriteWarning(string.Format(Resources.FrequencyParsingFailed, completionParts[completionFrequencyIndex]));
                else
                    Logger.WriteWarning(string.Format(Resources.DictionaryStringParsingFailed, inputString));
            }
            Logger.WriteVerbose(string.Format(Resources.DictionaryLoadingCompleted, this.Count));
            Logger.WriteVerbose(Resources.DictionarySortingStarted);
            this.Sort(delegate(IWordCompletion left, IWordCompletion right)
            {
                return left.Word.CompareTo(right.Word);
            });
            Logger.WriteVerbose(Resources.DictionarySortingCompleted);
        }

        public IEnumerable<IWordCompletion> GetAllCompletions(string wordToComplete)
        {
            return new AllWordCompletionsEnumerator(this, wordToComplete);
        }

        public IEnumerable<IWordCompletion> GetTenBestCompletions(string wordToComplete)
        {
            IEnumerable<IWordCompletion> result;
            if (!cache.TryGetValue(wordToComplete, out result))
            {
                result = new TenBestCompletionsGenerator(this, wordToComplete);
                cache.Add(wordToComplete, result);
            }
            return result;
        }
    }
}
