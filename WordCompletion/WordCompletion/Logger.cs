﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCompletionGenerator
{
    public class Logger
    {
        private static TraceSwitch traceSwitch = new TraceSwitch("General", "Entire application");
        private Logger()
        {
            traceSwitch = new TraceSwitch("General", "Entire application");
        }
        public static void WriteVerbose(string message)
        {
            Trace.WriteIf(traceSwitch.TraceVerbose, message, "Verbose");
        }
        public static void WriteInfo(string message)
        {
            Trace.WriteIf(traceSwitch.TraceInfo, message, "Information");
        }
        public static void WriteWarning(string message)
        {
            if (traceSwitch.TraceWarning)
              Trace.TraceWarning(message);
        }
        public static void WriteError(string message)
        {
            // TODO: Реализовать запись стека вызова и других подробностей.
            if (traceSwitch.TraceError)
                Trace.TraceError(message);
        }
    }
}