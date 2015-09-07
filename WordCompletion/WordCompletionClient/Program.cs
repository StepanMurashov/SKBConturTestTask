using System;
using System.IO;
using WordCompletions;

namespace WordCompletionClient
{
    class Program
    {
        private static void GenerateAnswers(TextReader input, TextWriter output, IWordCompletions dictionary)
        {
            int questionsCount = int.Parse(input.ReadLine());
            for (int i = 0; i < questionsCount; i++)
            {
                string question = input.ReadLine();
                foreach (IWordCompletion completion in dictionary.GetTenBestCompletions(question))
                    output.WriteLine(completion.Word);
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
                GenerateAnswers(Console.In, Console.Out, WordCompletionBuilder.CreateFromStream(Console.In));
            }
            catch (Exception e)
            {
                Logger.WriteError(e.Message);
                Environment.Exit(-1);
            }
        }
    }
}
