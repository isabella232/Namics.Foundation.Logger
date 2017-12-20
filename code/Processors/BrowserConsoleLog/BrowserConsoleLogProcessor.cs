// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserConsoleLogProcessor.cs" company="Namics AG">
//   (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// <summary>
//   Log handler that writes log entries to the browser console by using http headers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger.Processors.BrowserConsoleLog
{
    using System.Web;

    /// <summary>
    /// Log handler that writes log entries to the browser console by using http headers.
    /// </summary>
    public class BrowserConsoleLogProcessor : ILogProcessor
    {
        /// <summary>
        /// Unique key for the entry in the HttpContext.Current.Items collection
        /// </summary>
        private const string ItemKey = "Namics.Foundation.Logger.Processors.BrowserConsoleLog.BrowserConsoleLogProcessor";

        /// <summary>
        /// Ip Setting indicating console log should be allowed for all IPs
        /// </summary>
        private const string AllIpAllowedIndicator = "ALL";

        /// <summary>
        /// Writes log entries to the sitecore log
        /// </summary>
        /// <param name="pEntry">
        /// The p Entry.
        /// </param>
        public void WriteLogEntry(LogEntry pEntry)
        {
            // Check if we have http context
            var context = HttpContext.Current;
            if (context == null)
            {
                return;
            }

            // Check if the users IP is allowed to receive log entries
            if (!IsIpAllowed(context.Request.UserHostAddress))
            {
                return;
            }

            // Get the fire PHP log utility reference from the request context
            FirePhp firePhp = null;
            if (HttpContext.Current.Items.Contains(ItemKey))
            {
                firePhp = (FirePhp)HttpContext.Current.Items[ItemKey];
            } 

            if (firePhp == null)
            {
                firePhp = new FirePhp();

                HttpContext.Current.Items.Add(ItemKey, firePhp);
            }

            switch (pEntry.Type)
            {
                case LogEntryType.DEBUG:
                    firePhp.Debug(pEntry.Message, pEntry.Caller, pEntry.LineNumber);
                    break;
                case LogEntryType.INFO:
                    firePhp.Info(pEntry.Message, pEntry.Caller, pEntry.LineNumber);
                    break;
                case LogEntryType.WARN:
                    firePhp.Warn(pEntry.Message, pEntry.Caller, pEntry.LineNumber);
                    break;
                case LogEntryType.ERROR:
                    firePhp.Error(pEntry.Message, pEntry.Caller, pEntry.LineNumber);
                    break;
                case LogEntryType.FATAL:
                    firePhp.Error("FATAL:" + pEntry.Message, pEntry.Caller, pEntry.LineNumber);
                    break;
                case LogEntryType.AUDIT:
                    firePhp.Info("AUDIT:" + pEntry.Message, pEntry.Caller, pEntry.LineNumber);
                    break;
            }

            // Set HTTP headers if possible
            // Add base headers once
            foreach (var header in firePhp.BaseHeaders())
            {
                if (header.Key != null && context.Response.Headers.Get(header.Key) == null)
                {
                    context.Response.Headers.Add(header.Key, header.Value);
                }
            }

            foreach (var header in firePhp.LogHeaders())
            {
                if (header.Key != null && context.Response.Headers.Get(header.Key) == null)
                {
                    context.Response.Headers.Add(header.Key, header.Value);
                }
            }
        }

        /// <summary>
        /// Returns true if the ip of the current request is allowed to receive log messages
        /// </summary>
        /// <param name="pUserIp">
        /// The p user ip.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsIpAllowed(string pUserIp)
        {
			var allowedIp = LogConfig.ConsoleAllowedIp;

            if (string.IsNullOrEmpty(pUserIp))
            {
                return false;
            }

            foreach (var ip in allowedIp)
            {
                if (string.IsNullOrEmpty(ip))
                {
                    continue;
                }

                if (pUserIp.StartsWith(ip) || ip.Equals(AllIpAllowedIndicator))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
