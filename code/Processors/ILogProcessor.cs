// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogProcessor.cs" company="Namics AG">
//   
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// <summary>
//   The LogProcessor interface. Every Processor needs to implement this interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger.Processors
{
    /// <summary>
    /// The LogProcessor interface. Every Processor needs to implement this interface.
    /// </summary>
    public interface ILogProcessor
    {
        /// <summary>
        /// Processes a log entry.
        /// </summary>
        /// <param name="pEntry">
        /// The log entry object
        /// </param>
        void WriteLogEntry(LogEntry pEntry);
    }
}
