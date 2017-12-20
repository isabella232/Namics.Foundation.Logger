// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Namics AG">
//   (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// <summary>
//   Defines the LogLevel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger
{
    /// <summary>
    /// The log level.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Not set. No entries will be written.
        /// </summary>
        NOTSET,

        /// <summary>
        /// Nothing will be logged.
        /// </summary>
        NONE,

        /// <summary>
        /// Only AUDIT messages will be logged.
        /// </summary>
        AUDIT,

        /// <summary>
        /// Sets log level to FATAL
        /// </summary>
        FATAL,

        /// <summary>
        /// Sets log level ERROR
        /// </summary>
        ERROR,

        /// <summary>
        /// Sets log level to WARN
        /// </summary>
        WARN,

        /// <summary>
        /// Sets log level to INFO
        /// </summary>
        INFO,

        /// <summary>
        /// Sets log level to DEBUG
        /// </summary>
        DEBUG
    }
}
