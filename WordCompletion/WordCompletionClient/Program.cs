using System;
using System.Globalization;
using System.IO;
using Sten.WordCompletions.Client.Properties;
using Sten.WordCompletions.Library;

[assembly: CLSCompliant(true)]
namespace Sten.WordCompletions.Client
{
    class Program
    {
        /// <summary>
        /// Сгенерировать ответы.
        /// </summary>
        /// <param name="input">Входной поток с вопросами.</param>
        /// <param name="output">Выходной поток для ответов.</param>
        /// <param name="answersGenerator">Генератор ответов.</param>
        private static void GenerateAnswers(TextReader input, TextWriter output, IWordCompletionsGenerator answersGenerator)
        {
            int questionsCount = int.Parse(input.ReadLine(), CultureInfo.CurrentCulture);
            for (int i = 0; i < questionsCount; i++)
            {
                string question = input.ReadLine();
                foreach (IWordCompletion completion in answersGenerator.GetTenBestCompletions(question))
                    output.WriteLine(completion.Word);
                output.WriteLine();
                if (i % 1000 == 0)
                    Logger.WriteVerbose(string.Format(CultureInfo.CurrentCulture, Resources.AnsweredQuestionsNumber, i));
            }
            Logger.WriteVerbose(string.Format(CultureInfo.CurrentCulture, Resources.AllQuestionsAnswered));
        }

        static void Main()
        {
            try
            {
                GenerateAnswers(Console.In, Console.Out, WordCompletionsGeneratorFactory.CreateFromTextReader(Console.In));
            }
            catch (Exception e)
            {
                Logger.WriteError(e.Message);
                Environment.Exit(-1);
            }
        }
    }
}
