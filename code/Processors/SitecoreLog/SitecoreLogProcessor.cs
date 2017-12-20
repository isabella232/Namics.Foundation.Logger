// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitecoreLogHandler.cs" company="Namics AG">
//   (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// <summary>
//   The sitecore log processor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger.Processors.SitecoreLog
{
    using Namics.Foundation.Logger;

    using Sitecore.Diagnostics;

    /// <summary>
    /// Log processor that writes log entries to the Sitecore Log.
    /// </summary>
    public class SitecoreLogProcessor : ILogProcessor
    {
        /// <summary>
        /// Writes log entries to the sitecore log
        /// </summary>
        /// <param name="pEntry">
        /// The log entry object
        /// </param>
        public void WriteLogEntry(LogEntry pEntry)
        {
            var message = CreateMessage(pEntry);

            switch (pEntry.Type)
            {
                case LogEntryType.DEBUG:
                    Log.Debug(message);
                    break;
                case LogEntryType.INFO:
                    Log.Info(message, typeof(SitecoreLogProcessor));
                    break;
                case LogEntryType.WARN:
                    Log.Warn(message, pEntry.Exception, typeof(SitecoreLogProcessor));
                    break;
                case LogEntryType.ERROR:
                    Log.Error(message, pEntry.Exception, typeof(SitecoreLogProcessor));
                    break;
                case LogEntryType.FATAL:
                    Log.Fatal(message, pEntry.Exception, typeof(SitecoreLogProcessor));
                    break;
                case LogEntryType.AUDIT:
                    Log.Audit(message, typeof(SitecoreLogProcessor));
                    break;
            }
        }

        /// <summary>
        /// Creates a string message from caller and message
        /// </summary>
        /// <param name="pEntry">
        /// The entry object
        /// </param>
        /// <returns>
        /// The <see cref="string"/> . 
        /// </returns>
        private static string CreateMessage(LogEntry pEntry)
        {
            return pEntry.Caller + ": " + pEntry.Message;
        }
    }
}
