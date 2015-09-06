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
                if (i % 1000 == 0)
                    Logger.WriteVerbose(string.Format("{0} questuions answered.", i));
            }
            Logger.WriteVerbose(string.Format("All questuions answered."));
        }

        static void Main(string[] args)
        {
            try
            {
                GenerateAnswers(Console.In, Console.Out, DictionaryBuilder.CreateFromStream(Console.In));
            }
            catch (Exception e)
            {
                Logger.WriteError(e.Message);
                Environment.Exit(-1);
            }
        }
    }
}
