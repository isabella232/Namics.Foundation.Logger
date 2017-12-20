// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntry.cs" company="Namics AG">
//   (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// <summary>
//   Defines the LogEntry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Namics.Foundation.Logger
{
    using System;

    /// <summary>
    /// The log entry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the type. i.E. WARN, ERROR, INFO
        /// </summary>
        public LogEntryType Type { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the method name that created the log entry.
        /// </summary>
        public string Caller { get; set; }

        /// <summary>
        /// Gets or sets the source code line number that created the log entry.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
