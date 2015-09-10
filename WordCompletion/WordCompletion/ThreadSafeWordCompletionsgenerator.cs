using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sten.WordCompletions.Library
{
    /// <summary>
    /// Потокобезопасный генератор вариантов автодополнения для слов.
    /// </summary>
    class ThreadSafeWordCompletionsGenerator : WordCompletionsGenerator
    {
        /// <summary>
        /// Сгенерировать все возможные слова и автодополнения для них.
        /// </summary>
        private void GenerateAllPosibleWordsCompletions()
        {
            Logger.WriteVerbose("All possible words completions generation started.");
            foreach (WordCompletion completion in this)
                for (int i = 1; i <= completion.Word.Length; i++)
                    base.GetTenBestCompletions(completion.Word.Substring(0, i));
            Logger.WriteVerbose(string.Format(CultureInfo.CurrentCulture,
                "All possible words completions generation completed."));
        }

        /// <summary>
        /// Создать экземпляр генератора вариантов автодополнения для слов.
        /// </summary>
        /// <param name="input">Входной поток с вариантами автодополнения.</param>
        public ThreadSafeWordCompletionsGenerator(TextReader input)
            : base(input)
        {
            // Единственным изменяемым по ходу работы полем класса является кэш cache.
            // Если мы заранее сгенерируем все возможные варианты слов и занесем их в кэш, то по ходу работы 
            // кэш уже не будет модифицироваться, а только читаться.
            // Благодаря этому можно будет не синхронизировать доступ к нему.
            GenerateAllPosibleWordsCompletions();
        }

        #region Переопределенные методы WordCompletionsGenerator.

        public override IEnumerable<IWordCompletion> GetTenBestCompletions(string wordToComplete)
        {
            IEnumerable<IWordCompletion> result;
            if (!TryGetCachedBestCompletions(wordToComplete, out result))
                return new List<IWordCompletion>();
            return result;
        }

        #endregion
    }
}
