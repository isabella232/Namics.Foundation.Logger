// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Namics AG">
//  Sitecore Shared Source Logger Module  
//  (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    ///   Contains static methods for logging. These will invoke the registered log processors which will generate log output depending
    ///   on the log level.
    /// </summary>
    public class Logger
    {
        #region Consts
        /// <summary>
        /// The WriteLogEntry-Method-Name for the Processors
        /// </summary>
        private const string WRITE_LOG_ENTRY = "WriteLogEntry";
        
        #endregion

        #region Static Fields

        /// <summary>
        ///   Defines which type of entries should be logged.
        /// </summary>
        private static LogLevel _mLogLevel = LogLevel.NOTSET;

        /// <summary>
        /// A list of processors fully qualified names
        /// </summary>
        private static List<string> _mProcessors; 

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value that defines, which type of entries should be logged.
        /// </summary>
        public static LogLevel LogLevel
        {
            get
            {
                // If a log level was programattically set, use it.
                if (_mLogLevel != LogLevel.NOTSET)
                {
                    return _mLogLevel;
                }

                // Read level from config
                return LogConfig.LogLevel;
            }
            
            set
            {
                _mLogLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the log processors. These are defined by the full name (namespace+classname) of the processor class.
        /// </summary>
        public static List<string> Processors
        {
            get
            {
                // If processors were programatically set, use them
                if (_mProcessors != null)
                {
                    return _mProcessors;
                }

                return LogConfig.LogProcessors;
            }

            set
            {
                _mProcessors = value;
            }
        } 

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether AUDIT log level is enabled.
        /// </summary>
        private static bool IsAuditEnabled
        {
            get
            {
                return (int)LogLevel >= (int)LogLevel.AUDIT;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether DEBUG log level is enabled.
        /// </summary>
        private static bool IsDebugEnabled
        {
            get
            {
                return (int)LogLevel >= (int)LogLevel.DEBUG;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether ERROR log level is enabled.
        /// </summary>
        private static bool IsErrorEnabled
        {
            get
            {
                return (int)LogLevel >= (int)LogLevel.ERROR;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether FATAL log level is enabled.
        /// </summary>
        private static bool IsFatalEnabled
        {
            get
            {
                return (int)LogLevel >= (int)LogLevel.FATAL;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether INFO log level is enabled.
        /// </summary>
        private static bool IsInfoEnabled
        {
            get
            {
                return (int)LogLevel >= (int)LogLevel.INFO;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether WARN log level is enabled.
        /// </summary>
        private static bool IsWarnEnabled
        {
            get
            {
                return (int)LogLevel >= (int)LogLevel.WARN;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Log an audit message
        /// </summary>
        /// <param name="pMessage">
        /// The message 
        /// </param>
        public static void Audit(string pMessage)
        {
            if (!IsAuditEnabled)
            {
                return;
            }

            WriteEntry(LogEntryType.AUDIT, pMessage);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="pMessage">
        /// The message 
        /// </param>
        public static void Debug(string pMessage)
        {
            if (!IsDebugEnabled)
            {
                return;
            }

            WriteEntry(LogEntryType.DEBUG, pMessage);
        }

        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="pMessage">
        /// The message 
        /// </param>
        /// <param name="pException">
        /// The p Exception.
        /// </param>
        public static void Error(string pMessage, Exception pException = null)
        {
            if (!IsErrorEnabled)
            {
                return;
            }

            WriteEntry(LogEntryType.ERROR, pMessage, pException);
        }

        /// <summary>
        /// Log a fatal message
        /// </summary>
        /// <param name="pMessage">
        /// The message 
        /// </param>
        /// <param name="pException">
        /// The p Exception.
        /// </param>
        public static void Fatal(string pMessage, Exception pException = null)
        {
            if (!IsFatalEnabled)
            {
                return;
            }

            WriteEntry(LogEntryType.FATAL, pMessage, pException);
        }

        /// <summary>
        /// Log an info message
        /// </summary>
        /// <param name="pMessage">
        /// The message 
        /// </param>
        public static void Info(string pMessage)
        {
            if (!IsInfoEnabled)
            {
                return;
            }

            WriteEntry(LogEntryType.INFO, pMessage);
        }

        /// <summary>
        /// Log a warning
        /// </summary>
        /// <param name="pMessage">
        /// The message 
        /// </param>
        /// <param name="pException">
        /// The p Exception.
        /// </param>
        public static void Warn(string pMessage, Exception pException = null)
        {
            if (!IsWarnEnabled)
            {
                return;
            }

            WriteEntry(LogEntryType.WARN, pMessage, pException);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Returns the caller of the last method from stack trace or an empty string if not found.
        /// </summary>
        /// <returns> Name of the calling method </returns>
        private static string GetCaller()
        {
            StackFrame frame = new StackTrace().GetFrame(3);
            if (frame == null)
            {
                return string.Empty;
            }
            
            MethodBase caller = frame.GetMethod();
            if (caller == null || string.IsNullOrEmpty(caller.Name))
            {
                return string.Empty;
            }

            return caller.ReflectedType.Namespace + "." + caller.Name;
        }

        /// <summary>
        ///   Returns the caller line of the last method from stack trace or 0 if not found.
        /// </summary>
        /// <returns> Name of the calling method </returns>
        private static int GetCallerLine()
        {
            StackFrame frame = new StackTrace(true).GetFrame(3);
            if (frame == null)
            {
                return 0;
            }

            return frame.GetFileLineNumber();
        }

        /// <summary>
        /// Writes a log entry by invoking all registered log processors and letting them write
        ///   the log entry.
        ///   Processors can be registered by adding the assembly name of the respective processor class.
        /// </summary>
        /// <param name="pType">
        /// The type. 
        /// </param>
        /// <param name="pMessage">
        /// The message. 
        /// </param>
        /// <param name="pException">
        /// The p Exception.
        /// </param>
        private static void WriteEntry(LogEntryType pType, string pMessage, Exception pException = null)
        {
            try
            {
                var logEntry = new LogEntry
                {
                    Caller = GetCaller(),
                    LineNumber = GetCallerLine(),
                    Message = pMessage,
                    Type = pType,
                    Exception = pException
                };
                LogLevel logLevel;
                Enum.TryParse(pType.ToString(), true, out logLevel);

                // Now, invoke all processors in the pipeline to let them log the entry their way

                // Invoke the method for each processor
                foreach (var processor in Processors)
                {
                    if ((int)LogConfig.GetLogLevel(processor) >= (int)logLevel)
                    {
                        var calledType = Type.GetType(processor, false, false);

                        if (calledType == null)
                        {
                            return;
                        }

                        // Looks for a method called "WriteLogEntry"
                        MethodInfo method = calledType.GetMethod(WRITE_LOG_ENTRY);

                        if (method != null)
                        {
                            try
                            {
                                object processorInstance = Activator.CreateInstance(calledType, null);
                                method.Invoke(processorInstance, new object[] { logEntry });
                            }
                            catch
                            {
                                // Do nothing on error. Don't try to log something here, you might create a loop!
                            }
                        }
                    }
                }
            }
            catch
            {
                // Do nothing on error. Don't try to log something here, you might create a loop!
            }
        }

        #endregion
    }
}