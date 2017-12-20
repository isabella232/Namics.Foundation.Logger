// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryLogProcessor.cs" company="Namics AG">
//   (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger.Processors.MemoryLog
{
    using System.Collections.Generic;

    /// <summary>
    ///   Keeps log entries in Memory. The log entries can be accessed by using MemoryLogProcessor.Entries. This can be used for
    ///   Unit Testing and for displaying Log Entries somewhere. Caution: All entries from all threads are logged here. You
    ///   Have to manually call MemoryLogProcessor.Clear() to empty the memory log.
    /// </summary>
    public class MemoryLogProcessor : ILogProcessor
    {
        #region Constants

        /// <summary>
        ///   The max number of log entries in memory.
        /// </summary>
        private const int MaxLogEntriesInMemory = 1000;

        #endregion

        #region Static Fields

        /// <summary>
        ///   The entries. Caution: this is not thread safe: Only use for development.
        /// </summary>
        private static SynchronizedCollection<LogEntry> _mEntries = new SynchronizedCollection<LogEntry>();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the entries. Caution: this needs to be cleared by calling Clear() because entries will live in memory as long as the application does.
        /// </summary>
        public static SynchronizedCollection<LogEntry> Entries
        {
            get
            {
                return _mEntries;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Clears log entries from memory. (Entries written to log file will remain)
        /// </summary>
        public static void Clear()
        {
            _mEntries = new SynchronizedCollection<LogEntry>();
        }

        /// <summary>
        ///   Returns all logged entries and clears the entry collection
        /// </summary>
        /// <returns> List of log entries </returns>
        public static List<LogEntry> FlushEntries()
        {
            var tmpEntries = new List<LogEntry>();

            // The collection needs to be locked before copying the entries
            lock (_mEntries.SyncRoot)
            {
                try
                {
                    tmpEntries.AddRange(Entries);
                } 
                catch
                {
                    // do nothing
                }
            }

            Clear();

            return tmpEntries;
        }

        /// <summary>
        /// Writes log entries to the memory log
        /// </summary>
        /// <param name="pEntry">
        /// The log entry object 
        /// </param>
        public void WriteLogEntry(LogEntry pEntry)
        {
            // Adds a log entry and ensures, memory won't be filled with too many log entries
            if (Entries.Count < MaxLogEntriesInMemory)
            {
                Entries.Add(pEntry);
            }
        }

        #endregion
    }
}