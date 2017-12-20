// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseLogProcessor.cs" company="Namics AG">
//  (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger.Processors.DatabaseLog
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web;

    /// <summary>
    /// Creates a LogEntry into a Database in the Solution with the ConnectionString Name = "LogDatabase"
    /// </summary>
    public class DatabaseLogProcessor : ILogProcessor
    {
        /// <summary>
        /// The Name of the ConnectionString
        /// </summary>
        private const string TABLE_NAME = "DatabaseLog";

        /// <summary>
        /// Writes log entries into a database table
        /// </summary>
        /// <param name="pEntry">
        /// The log entry object 
        /// </param>
        public void WriteLogEntry(LogEntry pEntry)
        {
            var conString = System.Configuration.ConfigurationManager.ConnectionStrings[TABLE_NAME];
            if (conString != null && !string.IsNullOrEmpty(conString.ConnectionString))
            {
                using (var sqlConnection = new SqlConnection(conString.ConnectionString))
                {
                    sqlConnection.Open();
                    var sqlCommand = new SqlCommand(DatabaseLogResources.CheckTableExistsCommand, sqlConnection);
                    var adapterLog = new SqlDataAdapter(sqlCommand);
                    var tableLog = new DataTable(TABLE_NAME);
                    
                    adapterLog.Fill(tableLog);

                    if (tableLog.Rows.Count < 1)
                    {
                        var createCommand = new SqlCommand(DatabaseLogResources.CreateTableCommand, sqlConnection);
                        createCommand.ExecuteNonQuery();
                        var createConstraintCommand = new SqlCommand(DatabaseLogResources.AddTableContraintCommand, sqlConnection);
                        createConstraintCommand.ExecuteNonQuery();
                    }

                    var exceptionMessage = string.Empty;
                    var exceptionInnerMessage = string.Empty;
                    var exceptionStackTrace = string.Empty;

                    if (pEntry.Exception != null)
                    {
                        if (!string.IsNullOrEmpty(pEntry.Exception.Message))
                        {
                            exceptionMessage = pEntry.Exception.Message;
                        }

                        if (pEntry.Exception.InnerException != null
                            && !string.IsNullOrEmpty(pEntry.Exception.InnerException.Message))
                        {
                            exceptionInnerMessage = pEntry.Exception.InnerException.Message;
                        }

                        if (!string.IsNullOrEmpty(pEntry.Exception.StackTrace))
                        {
                            exceptionStackTrace = pEntry.Exception.StackTrace;
                        }
                    }

                    // Check if we have http context
                    var context = HttpContext.Current;
                    string url = string.Empty;
                    if (context != null)
                    {
                        url = context.Request.Url.AbsoluteUri;
                    }
                    
                    var query = string.Format(
                        DatabaseLogResources.CreateLogEntryCommand, 
                        pEntry.Type.ToString(), 
                        Environment.MachineName,
                        GetIISPoolName(), 
                        pEntry.Message,
                        exceptionMessage,
                        exceptionInnerMessage, 
                        pEntry.LineNumber, 
                        pEntry.Caller,
                        exceptionStackTrace, 
                        url);

                    var insertCommand = new SqlCommand(query, sqlConnection);
                    var param = new SqlParameter("@Date", SqlDbType.DateTime) { Value = DateTime.Now };
                    insertCommand.Parameters.Add(param);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The get system identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetIISPoolName()
        {
            try
            {
                return System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName();
            }
            catch (Exception)
            {
                return string.Empty;
            }
            
        }
    }
}
