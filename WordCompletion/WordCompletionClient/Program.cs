using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordCompletion;

namespace WordCompletionClient
{
    class Program
    {
        private static void GenerateAnswers(TextReader input, TextWriter output, IWordCompletionDictionary dictionary)
        {
            int questuionsCount = int.Parse(input.ReadLine());
            for (int i = 0; i < questuionsCount; i++)
            {
                foreach (WordCompletionDictionaryItem completion in dictionary.GetTop10Completions(input.ReadLine()))
                {
                    output.WriteLine(completion.Word);
                }
                output.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            GenerateAnswers(Console.In, Console.Out, DictionaryFactory.CreateFromStream(Console.In));
        }
    }
}
