using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Config;

namespace Resxar
{
    public class AppLog
    {
        public enum LogLevel
        {
            DEBUG, INFO, WARNING, ERROR,
        };

        private static LogLevel m_logLevel = LogLevel.INFO;
        public static LogLevel OutputLogLevel
        {
            get
            {
                return m_logLevel;
            }

            set
            {
                m_logLevel = value;
            }
        }

        private static ILog s_log;

        static AppLog()
        {
            Configure();
        }

        public static void Configure()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(AppLog));
            using (MemoryStream configStream = new MemoryStream(Properties.Log.xml_log))
            {
                Configure(configStream);
            }
        }

        public static void Configure(Stream stream)
        {
            XmlConfigurator.Configure(stream);
            s_log = LogManager.GetLogger(typeof(AppLog));
        }

        public static bool IsDebugEnabled
        {
            get
            {
                return m_logLevel <= LogLevel.DEBUG;
            }
        }

        public static void Debug(Type context, string message)
        {
            if (IsDebugEnabled)
            {
                WriteLine(context, message, LogLevel.DEBUG);
            }
        }

        public static void Debug(Type context, string message, params object[] messageParams)
        {
            if (IsDebugEnabled)
            {
                WriteLine(context, string.Format(message, messageParams), LogLevel.DEBUG);
            }
        }

        public static bool IsInfoEnabled
        {
            get
            {
                return m_logLevel <= LogLevel.INFO;
            }
        }

        public static void Info(Type context, string message)
        {
            if (IsInfoEnabled)
            {
                WriteLine(context, message, LogLevel.INFO);
            }
        }

        public static void Info(Type context, string message, params object[] messageParams)
        {
            if (IsInfoEnabled)
            {
                WriteLine(context, string.Format(message, messageParams), LogLevel.INFO);
            }
        }

        public static bool IsWarnEnabled
        {
            get
            {
                return m_logLevel <= LogLevel.WARNING;
            }
        }

        public static void Warn(Type context, string message)
        {
            if (IsWarnEnabled)
            {
                WriteLine(context, message, LogLevel.WARNING);
            }
        }

        public static void Warn(Type context, string message, params object[] messageParams)
        {
            if (IsWarnEnabled)
            {
                WriteLine(context, string.Format(message, messageParams), LogLevel.WARNING);
            }
        }

        public static void Warn(Type context, Exception e)
        {
            if (IsWarnEnabled)
            {
                WriteLine(context, GetExceptionMessage(e), LogLevel.WARNING);
            }
        }

        public static void Warn(Type context, string message, Exception e)
        {
            if (IsWarnEnabled)
            {
                WriteLine(
                    context,
                    message + Environment.NewLine + GetExceptionMessage(e),
                    LogLevel.WARNING);
            }
        }

        public static void Warn(Type context, string message, Exception e, params object[] messageParams)
        {
            if (IsWarnEnabled)
            {
                WriteLine(
                    context,
                    string.Format(message, messageParams) + Environment.NewLine + GetExceptionMessage(e),
                    LogLevel.WARNING);
            }
        }

        public static bool IsErrorEnabled
        {
            get
            {
                return m_logLevel <= LogLevel.ERROR;
            }
        }

        public static void Error(Type context, string message)
        {
            if (IsErrorEnabled)
            {
                WriteLine(context, message, LogLevel.ERROR);
            }
        }

        public static void Error(Type context, string message, params object[] messageParams)
        {
            if (IsErrorEnabled)
            {
                WriteLine(context, string.Format(message, messageParams), LogLevel.ERROR);
            }
        }

        public static void Error(Type context, Exception e)
        {
            if (IsErrorEnabled)
            {
                WriteLine(context, GetExceptionMessage(e), LogLevel.ERROR);
            }
        }

        public static void Error(Type context, string message, Exception e)
        {
            if (IsErrorEnabled)
            {
                WriteLine(
                    context,
                    message + Environment.NewLine + GetExceptionMessage(e),
                    LogLevel.ERROR);
            }
        }

        public static void Error(Type context, string message, Exception e, params object[] messageParams)
        {
            if (IsErrorEnabled)
            {
                WriteLine(
                    context,
                    string.Format(message, messageParams) + Environment.NewLine + GetExceptionMessage(e),
                    LogLevel.ERROR);
            }
        }



        private static string GetExceptionMessage(Exception e)
        {
            List<string> result = new List<string>();
            int count = 0;
            while (e != null && count <= 10)
            {
                ++count;
                result.Add(e.GetType().FullName + ": " + e.Message + Environment.NewLine + e.StackTrace.ToString());
                e = e.InnerException;
            }
            return string.Join(Environment.NewLine + "Caused by ", result);
        }

        private static void WriteLine(Type context, string message, LogLevel level)
        {
            string newMessage = context.FullName + " - " + message;

            switch (level)
            {
                case LogLevel.DEBUG:
                    s_log.Debug(newMessage);
                    break;
                case LogLevel.INFO:
                    s_log.Info(newMessage);
                    break;
                case LogLevel.WARNING:
                    s_log.Warn(newMessage);
                    break;
                case LogLevel.ERROR:
                    s_log.Error(newMessage);
                    break;
            }
        }

        public static string ToString<T>(ICollection<T> collection)
        {
            StringBuilder result = new StringBuilder();
            result.Append("(");
            int i = 0;
            foreach (T item in collection)
            {
                if (0 < i)
                {
                    result.Append(", ");
                }

                result.Append(item.ToString());

                ++i;
            }
            result.Append(")");
            return result.ToString();
        }

        private static string GetLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.DEBUG:
                    return "DEBUG";
                case LogLevel.INFO:
                    return "INFO ";
                case LogLevel.WARNING:
                    return "WARN ";
                case LogLevel.ERROR:
                    return "ERROR";
                default:
                    return "UNKN ";
            }
        }
    }
}
