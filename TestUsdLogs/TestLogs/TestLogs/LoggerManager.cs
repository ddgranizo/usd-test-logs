using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLogs
{
    public class LoggerManager
    {
        #region Private Fields

        /// <summary>
        /// String Builder Info
        /// </summary>
        private readonly StringBuilder _LastError = new StringBuilder();

        /// <summary>
        /// Last Exception
        /// </summary>
        private Exception _LastException;
        #endregion


        private string _logPath = null;
        public string LogPath
        {
            get
            {
                if (_logPath == null)
                {
                    _logPath = new Microsoft.Xrm.Tooling.PackageDeployment.CrmPackageExtentionBase.TraceLogger("asd").GetLogFilePath();
                }
                return _logPath;
            }
        }

        public TraceSource Source { get; internal set; }

        public string LastError { get { return _LastError.ToString(); } }

        public Exception LastException { get { return _LastException; } }

        public SourceLevels CurrentTraceLevel
        {
            get
            {
                return this.Source.Switch.Level;
            }
        }

        public LoggerManager(string traceSourceName = "")
        {
            Source = new TraceSource(traceSourceName);
        }

        public void ResetLastError()
        {
            _LastError.Remove(0, LastError.Length);
            _LastException = null;
        }


        public void Log(string message, TraceEventType eventType = TraceEventType.Information, int eventId = 0)
        {
            string messageToLog = null;

            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageToLog = FormatMessage(message);
                    Source.TraceEvent(eventType, eventId, messageToLog);
                    if (eventType == TraceEventType.Error)
                    {
                        _LastError.Append(messageToLog);
                    }
                }
            }
            catch
            {
                //No implemented for not break the execution workflow
            }

        }

        public void Log(TraceEventType eventType, int eventId, Exception ex)
        {
            Log(eventType, eventId, ex, null);
        }
        public void Log(TraceEventType eventType, int eventId, Exception ex, string message, params object[] messageParams)
        {
            var sb = new StringBuilder();

            try
            {
                if (message != null)
                {
                    sb.Append(message);
                    sb.AppendLine();
                }
                if (ex != null)
                {
                    sb.Append(ex.ToString());
                }

                Source.TraceEvent(eventType, eventId, FormatMessage(sb.ToString()), messageParams);
            }
            catch
            {
                //No implemented for not break the execution workflow
            }
        }


        public void Log(string messageFormat, int eventId, TraceEventType eventType, params object[] args)
        {
            string message = string.Format(messageFormat, args);
            Log(message, eventType, eventId);
        }

        public void Log(string messageFormat, TraceEventType eventType = TraceEventType.Information, params object[] args)
        {
            string message = string.Format(messageFormat, args);
            Log(message, 0, eventType);
        }


        public void Log(string message, TraceEventType eventType, Exception exception, int eventId = 0)
        {
            StringBuilder sbException = new StringBuilder();

            try
            {
                Source.TraceEvent(eventType, eventId, FormatMessage(sbException.ToString()));
                sbException.AppendLine("Message: " + message);
                LogExceptionToFile(exception, sbException, 0);
                if (eventType == TraceEventType.Error)
                {
                    _LastError.Append(sbException.ToString());
                    _LastException = exception;
                }
            }
            catch
            {
                //No implemented for not break the execution workflow
            }

        }

        public void Log(Exception exception, TraceEventType eventType = TraceEventType.Error, int eventId = 0)
        {
            StringBuilder sbException = new StringBuilder();

            try
            {
                LogExceptionToFile(exception, sbException, eventId);
                if (sbException.Length > 0)
                {
                    Source.TraceEvent(eventType, eventId, FormatMessage(sbException.ToString()));
                }

                _LastError.Append(sbException.ToString());
                _LastException = exception;
            }
            catch
            {
                //No implemented for not break the execution workflow
            }
        }


        private static void LogExceptionToFile(Exception objException, StringBuilder sw, int level)
        {
            if (objException == null)
            {
                return;
            }
            if (level != 0)
            {
                sw.AppendLine(string.Format("Inner Exception Level {0}\t: ", level));
            }


            sw.AppendLine("Source\t: " +
                (objException.Source != null ? objException.Source.ToString().Trim() : "Not Provided"));
            sw.AppendLine("Method\t: " +
                (objException.TargetSite != null ? objException.TargetSite.Name.ToString() : "Not Provided"));
            sw.AppendLine("Date\t: " +
                    DateTime.Now.ToShortDateString());
            sw.AppendLine("Time\t: " +
                    DateTime.Now.ToLongTimeString());
            sw.AppendLine("Error\t: " +
                (string.IsNullOrEmpty(objException.Message) ? "Not Provided" : objException.Message.ToString().Trim()));
            sw.AppendLine("Stack Trace\t: " +
                (string.IsNullOrEmpty(objException.StackTrace) ? "Not Provided" : objException.StackTrace.ToString().Trim()));
            sw.AppendLine("======================================================================================================================");

            level++;
            if (objException.InnerException != null)
            {
                LogExceptionToFile(objException.InnerException, sw, level);
            }
        }


        public void LogVerbose(string message, params object[] messageParams)
        {
            Log(message, 0, TraceEventType.Verbose, messageParams);
        }
        public void LogInfo(string message, params object[] messageParams)
        {
            Log(message, 0, TraceEventType.Information, messageParams);
        }
        public void LogError(string message, params object[] messageParams)
        {
            Log(message, 0, TraceEventType.Error, messageParams);
        }
        public void LogWarning(string message, params object[] messageParams)
        {
            Log(message, 0, TraceEventType.Warning, messageParams);
        }


        private string FormatMessage(string message)
        {
            return string.Format("{0} {1}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
        }
    }
}
