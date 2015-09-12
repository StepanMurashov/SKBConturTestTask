using Sten.WordCompletions.LocalClient.Properties;
using System;
using System.Globalization;
using System.IO;

[assembly: CLSCompliant(true)]
namespace Sten.WordCompletions.LocalClient
{
    /// <summary>
    /// Локальный клиент для генерации автодополнений слов.
    /// </summary>
    class WordCompletionsLocalClient
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
                if (i != questionsCount - 1)
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
                GenerateAnswers(Console.In, Console.Out, WordCompletionsGeneratorFactory.CreateFromTextReader(Console.In,
                    WordCompletionsGeneratorThreadSafetyMode.None));
            }
            catch (Exception e)
            {
                Logger.WriteError(e.Message);
                Environment.Exit(-1);
            }
        }
    }
}
