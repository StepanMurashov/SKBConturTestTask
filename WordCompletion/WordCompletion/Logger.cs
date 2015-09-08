using System.Diagnostics;

namespace WordCompletions
{
    /// <summary>
    /// Класс ведения протокола работы.
    /// </summary>
    static public class Logger
    {
        /// <summary>
        /// Переключатель уровня протоколирования.
        /// </summary>
        private static TraceSwitch traceSwitch = new TraceSwitch("General", "Entire application");

        /// <summary>
        /// Записать отладочное сообщение в протокол.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void WriteVerbose(string message)
        {
            Trace.WriteLineIf(traceSwitch.TraceVerbose, message, "Verbose");
        }
        /// <summary>
        /// Записать информационное сообщение в протокол.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void WriteInfo(string message)
        {
            Trace.WriteLineIf(traceSwitch.TraceInfo, message, "Information");
        }

        /// <summary>
        /// Записать предупреждение в протокол.
        /// </summary>
        /// <param name="message">Текст предупреждения.</param>
        public static void WriteWarning(string message)
        {
            if (traceSwitch.TraceWarning)
              Trace.TraceWarning(message);
        }

        /// <summary>
        /// Записать ошибку в протокол.
        /// </summary>
        /// <param name="message">Текст ошибки.</param>
        public static void WriteError(string message)
        {
            // TODO: Реализовать запись стека вызова и других подробностей.
            if (traceSwitch.TraceError)
                Trace.TraceError(message);
        }
    }
}
