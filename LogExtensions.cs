using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Data.SqlClient;

namespace FmSoftlab.Logging
{
    public static class AllExceptionsLogger
    {
        public static void LogSqlException(this ILogger log, SqlException e, string sql)
        {
            if (e == null || log == null)
                return;
            log.LogError($"Sql error, sql:{sql} Server:{e.Server}, Procedure:{e.Procedure}, ErrorCode:{e.ErrorCode}, State:{e.State}, Number:{e.Number}, LineNumber:{e.LineNumber}, Source:{e.Source}, {e.Message}, ClientConnectionId:{e.ClientConnectionId}");
            foreach (SqlError x in e.Errors)
            {
                log.LogError($"Sql error {x.Message}, Server:{x.Server}, Number:{x.Number}, Procedure:{x.Procedure}, LineNumber:{x.LineNumber}, Source:{x.Source}");
            }
        }
        public static void LogException(this ILogger log, Exception ex, string sql)
        {
            if ((log is null) || (ex is null))
                return;

            log.LogError($"{ex.Message}, {ex.GetType()}, TargetSite: {ex.TargetSite?.Name}, " +
                $"Source: {ex.Source}, {ex.HelpLink} HResult: {ex.HResult}{Environment.NewLine}" +
                $"StackTrace: {ex.StackTrace}");
            if (ex is SqlException sqle)
            {
                LogSqlException(log, sqle, sql);
            }
            if (ex.Data?.Count > 0)
            {
                foreach (DictionaryEntry d in ex.Data)
                {
                    if (d.Value != null)
                    {
                        log.LogError($"name:{d.Key}, value:{d.Value}");
                    }
                }
            }
        }
        public static void LogAllErrors(this ILogger log, Exception ex, string sql)
        {
            if ((log is null) || (ex is null))
                return;
            List<Exception> exceptionList = new List<Exception>();
            var currentException = ex;
            while (currentException != null)
            {
                exceptionList.Add(currentException);
                currentException = currentException.InnerException;
            }
            foreach (var e in exceptionList)
            {
                LogException(log, e, sql);
            };
        }
        public static void LogAllErrors(this ILogger log, Exception ex)
        {
            LogAllErrors(log, ex, string.Empty);
        }
    }
}