// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirePHP.cs" company="Namics AG">
//   (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger.Processors.BrowserConsoleLog
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Script.Serialization;

    /// <summary>
    ///   Helper class used for creating FirePHP (Wildfire) Headers which can be read by browser Console in Firefox and Chrome.
    /// </summary>
    public class FirePhp
    {
        #region Fields

        /// <summary>
        ///   Contains the log entries.
        /// </summary>
        private readonly List<FirePhpLog> _mLogs;

        /// <summary>
        ///   The _ base headers.
        /// </summary>
        private readonly Dictionary<string, string> _mBaseHeaders;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FirePhp"/> class.
        /// </summary>
        public FirePhp()
        {
            _mBaseHeaders = new Dictionary<string, string>
                {
                    { "X-Wf-Protocol-1", "http://meta.wildfirehq.org/Protocol/JsonStream/0.2" },
                    { "X-Wf-1-Plugin-1", "http://meta.firephp.org/Wildfire/Plugin/FirePHP/Library-FirePHPCore/0.3" },
                    { "X-Wf-1-Structure-1", "http://meta.firephp.org/Wildfire/Structure/FirePHP/FirebugConsole/0.1" }
                };

            _mLogs = new List<FirePhpLog>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Returns a dictionary containing all the base headers
        /// </summary>
        /// <returns> Key Value Dictionary </returns>
        public Dictionary<string, string> BaseHeaders()
        {
            return _mBaseHeaders;
        }

        /// <summary>
        /// Creates a CRITICAL entry
        /// </summary>
        /// <param name="pMsg">
        /// The p msg.
        /// </param>
        /// <param name="pFile">
        /// The p file.
        /// </param>
        /// <param name="pLine">
        /// The p line.
        /// </param>
        public void Critical(string pMsg, string pFile = "", int pLine = 0)
        {
            Log("CRITICAL", pMsg, pFile, pLine);
        }

        /// <summary>
        /// Creates a DEBUG Entry
        /// </summary>
        /// <param name="pMsg">
        /// The msg. 
        /// </param>
        /// <param name="pFile">
        /// The file. 
        /// </param>
        /// <param name="pLine">
        /// The line. 
        /// </param>
        public void Debug(string pMsg, string pFile = "", int pLine = 0)
        {
            Log("LOG", pMsg, pFile, pLine);
        }

        /// <summary>
        /// Creates an ERROR Entry
        /// </summary>
        /// <param name="pMsg">
        /// The p msg.
        /// </param>
        /// <param name="pFile">
        /// The p file.
        /// </param>
        /// <param name="pLine">
        /// The p line.
        /// </param>
        public void Error(string pMsg, string pFile = "", int pLine = 0)
        {
            Log("ERROR", pMsg, pFile, pLine);
        }

        /// <summary>
        /// Creates an INFO entry
        /// </summary>
        /// <param name="pMsg">
        /// The p msg. 
        /// </param>
        /// <param name="pFile">
        /// The p file. 
        /// </param>
        /// <param name="pLine">
        /// The p line. 
        /// </param>
        public void Info(string pMsg, string pFile = "", int pLine = 0)
        {
            Log("INFO", pMsg, pFile, pLine);
        }

        /// <summary>
        ///   Returns a dictionary containing all the log headers (log entries)
        /// </summary>
        /// <returns> Key Value Dictionary</returns>
        public Dictionary<string, string> LogHeaders()
        {
            var ret = new Dictionary<string, string>();
            var serializer = new JavaScriptSerializer();

            for (int i = 0; i < _mLogs.Count; i++)
            {
                string json = string.Format("[{0}, {1}]", serializer.Serialize(_mLogs[i].Header), serializer.Serialize(_mLogs[i].Msg));

                ret.Add(string.Format("X-Wf-1-1-1-{0}", i + 1), string.Format("{0}|{1}|", json.Length, json));
            }

            ret.Add("X-Wf-1-Index", _mLogs.Count.ToString(CultureInfo.InvariantCulture));

            return ret;
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="pMsg">
        /// The p msg.
        /// </param>
        /// <param name="pFile">
        /// The p file.
        /// </param>
        /// <param name="pLine">
        /// The p line.
        /// </param>
        public void Warn(string pMsg, string pFile = "", int pLine = 0)
        {
            Log("WARN", pMsg, pFile, pLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes a log entry
        /// </summary>
        /// <param name="pLogType">
        /// The log type.
        /// </param>
        /// <param name="pMsg">
        /// The msg.
        /// </param>
        /// <param name="pFile">
        /// The file.
        /// </param>
        /// <param name="pLine">
        /// The line.
        /// </param>
        private void Log(string pLogType, string pMsg, string pFile = "", int pLine = 0)
        {
            var log = new FirePhpLog
                          {LogType = pLogType, Header = new {Type = pLogType, File = pFile, Line = pLine}, Msg = pMsg};


            _mLogs.Add(log);
        }

        #endregion
    }
}