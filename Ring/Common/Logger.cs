using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace OTP.Ring.Common
{
    public static class Logger
    {
        public enum LogType
        {
            ERROR,
            INFORMATION,
            DIAGNOSTIC
        }

        public static void LogError(string message)
        {
            Log(LogType.ERROR, message);
        }

        public static void LogMessage(string message)
        {
            Log(LogType.INFORMATION, message);
        }

        public static void LogDiagnostic(string message)
        {
            Log(LogType.DIAGNOSTIC, message);
        }

        private static void Log(LogType logType, string message)
        {
            if (RoleEnvironment.IsAvailable)
            {
                LogAzure(logType, message);
            }
            else
            {
                LogWindows(logType, message);
            }
        }

        private static void LogAzure(LogType logType, string message)
        {
            StackTrace stackTrace = new StackTrace();

            string logMessage = string.Format("{0}{1}In Method: {2}{1}{1}",
                message, Environment.NewLine, stackTrace.GetFrame(1).GetMethod());

            switch (logType)
            {
                //If you get an exception here comment out the Azure Listener in the Web.config
                case LogType.ERROR:
                    Trace.TraceError(logMessage);
                    break;
                case LogType.INFORMATION:
                    Trace.TraceInformation(logMessage);
                    break;
                case LogType.DIAGNOSTIC:
                    Trace.TraceWarning(logMessage);
                    break;
            }
        }

        /// <summary>
        /// Debug Development logger to Windows Event log
        /// </summary>
        private static void LogWindows(LogType logType, string message)
        {
            string source = "OTP";
            string log = "Application";

            StackTrace stackTrace = new StackTrace();
            string eventMessage = string.Format("{0}{1}In Method: {2}{1}{1}",
                message, Environment.NewLine, stackTrace.GetFrame(3).GetMethod());

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, log);
            }

            switch (logType)
            {
                //If you get an exception here comment out the Azure Listener in the Web.config
                case LogType.ERROR:
                    EventLog.WriteEntry(source, eventMessage, EventLogEntryType.Error);
                    break;
                case LogType.INFORMATION:
                    EventLog.WriteEntry(source, eventMessage, EventLogEntryType.Information);
                    break;
                case LogType.DIAGNOSTIC:
                    EventLog.WriteEntry(source, eventMessage, EventLogEntryType.Warning);
                    break;
            }

        }
    }
}