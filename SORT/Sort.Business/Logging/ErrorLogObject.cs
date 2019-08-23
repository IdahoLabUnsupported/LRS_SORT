using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sort.Business
{
    public class ErrorLogObject
    {
        #region Properties

        public int ErrorLogId { get; set; }
        public DateTime ErrorDate { get; set; }
        public int? SortmainId { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string StackTrack { get; set; }

        #endregion

        #region Constructor

        public ErrorLogObject() { }

        #endregion

        #region Repository

        private static IErrorLogRepository repo => new ErrorLogRepository();

        #endregion

        #region Static Methods

        public static List<ErrorLogObject> GetErrorLogs() => repo.GetErrorLogs();

        public static ErrorLogObject GetErrorLog(int errorLogId) => repo.GetErrorLog(errorLogId);

        public static void LogError(string name, Exception ex)
        {
            try
            {
                var error = new ErrorLogObject();
                error.Name = name;
                error.ErrorDate = DateTime.Now;
                error.Message = ex.Message;
                error.StackTrack = ex.StackTrace;
                error.Save();
            }
            catch { }
        }

        public static void LogError(string name, string message, string stackTrace)
        {
            try
            {
                var error = new ErrorLogObject();
                error.Name = name;
                error.ErrorDate = DateTime.Now;
                error.Message = message;
                error.StackTrack = stackTrace;
                error.Save();
            }
            catch { }
        }

        #endregion

        #region Object Methods

        public void Save()
        {
            try
            { repo.SaveErrorLog(this);}
            catch { }
        }

        public bool Delete()
        {
            return repo.DeleteErrorLog(this);
        }

        #endregion
    }

    public interface IErrorLogRepository
    {
        List<ErrorLogObject> GetErrorLogs();
        ErrorLogObject GetErrorLog(int errorLogId);
        ErrorLogObject SaveErrorLog(ErrorLogObject errorLog);
        bool DeleteErrorLog(ErrorLogObject errorLog);
    }

    public class ErrorLogRepository : IErrorLogRepository
    {
        public List<ErrorLogObject> GetErrorLogs() => Config.Conn.Query<ErrorLogObject>("SELECT * FROM dat_ErrorLog").ToList();

        public ErrorLogObject GetErrorLog(int errorLogId) => Config.Conn.Query<ErrorLogObject>("SELECT * FROM dat_ErrorLog WHERE ErrorLogId = @ErrorLogId", new { ErrorLogId = errorLogId }).FirstOrDefault();

        public ErrorLogObject SaveErrorLog(ErrorLogObject errorLog)
        {
            if (errorLog.ErrorLogId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_ErrorLog
                    SET     ErrorDate = @ErrorDate,
                            SortmainId = @SortmainId,
                            Name = @Name,
                            Message = @Message,
                            StackTrack = @StackTrack
                    WHERE   ErrorLogId = @ErrorLogId";
                Config.Conn.Execute(sql, errorLog);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_ErrorLog (
                        ErrorDate,
                        SortmainId,
                        Name,
                        Message,
                        StackTrack
                    )
                    VALUES (
                        @ErrorDate,
                        @SortmainId,
                        @Name,
                        @Message,
                        @StackTrack
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                errorLog.ErrorLogId = Config.Conn.Query<int>(sql, errorLog).Single();
            }
            return errorLog;
        }

        public bool DeleteErrorLog(ErrorLogObject errorLog)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_ErrorLog WHERE ErrorLogId = @ErrorLogId", errorLog);
            }
            catch { return false; }
            return true;
        }
    }
}