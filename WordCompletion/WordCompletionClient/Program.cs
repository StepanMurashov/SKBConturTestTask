using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordCompletionGenerator;

namespace WordCompletionClient
{
    class Program
    {
        private static void GenerateAnswers(TextReader input, TextWriter output, IWordCompletionDictionary dictionary)
        {
            int questionsCount = int.Parse(input.ReadLine());
            for (int i = 0; i < questionsCount; i++)
            {
                string question = input.ReadLine();
                foreach (WordCompletion completion in dictionary.GetTop10Completions(question))
                    output.WriteLine(completion.Value);
                output.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            GenerateAnswers(Console.In, Console.Out, DictionaryBuilder.CreateFromStream(Console.In));
        }
    }
}
