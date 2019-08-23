using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sort.Business
{
    public class ProcessLogObject
    {
        #region Properties

        public int ProcessLogId { get; set; }
        public DateTime EntryDate { get; set; }
        public int? SortMainId { get; set; }
        public string Result { get; set; }
        public string ResultDesc { get; set; }

        #endregion

        #region Constructor

        public ProcessLogObject() { }

        #endregion

        #region Repository

        private static IProcessLogRepository repo => new ProcessLogRepository();

        #endregion

        #region Static Methods

        public static List<ProcessLogObject> Get() => repo.GetProcessLogs();

        public static ProcessLogObject Get(int processLogId) => repo.GetProcessLog(processLogId);

        public static void Add(string log)
        {
            ProcessLogObject.Add(null, log, String.Empty);
        }

        public static void Add(int? sortMainId, string log)
        {
            ProcessLogObject.Add(sortMainId, log, String.Empty);
        }

        public static void Add(string log, string desc)
        {
            ProcessLogObject.Add(null, log, desc);
        }

        public static void Add(int? sortMainId, string log, string desc)
        {
            var o = new ProcessLogObject();
            o.SortMainId = sortMainId;
            o.Result = log;
            o.ResultDesc = desc;
            o.Save();
        }

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveProcessLog(this);
        }

        #endregion
    }

    public interface IProcessLogRepository
    {
        List<ProcessLogObject> GetProcessLogs();
        ProcessLogObject GetProcessLog(int processLogId);
        ProcessLogObject SaveProcessLog(ProcessLogObject processLog);
    }

    public class ProcessLogRepository : IProcessLogRepository
    {
        public List<ProcessLogObject> GetProcessLogs() => Config.Conn.Query<ProcessLogObject>("SELECT * FROM dat_ProcessLog").ToList();

        public ProcessLogObject GetProcessLog(int processLogId) => Config.Conn.Query<ProcessLogObject>("SELECT * FROM dat_ProcessLog WHERE ProcessLogId = @ProcessLogId", new { ProcessLogId = processLogId }).FirstOrDefault();

        public ProcessLogObject SaveProcessLog(ProcessLogObject processLog)
        {
            if (processLog.ProcessLogId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_ProcessLog
                    SET     SortMainId = @SortMainId,
                            Result = @Result,
                            ResultDesc = @ResultDesc
                    WHERE   ProcessLogId = @ProcessLogId";
                Config.Conn.Execute(sql, processLog);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_ProcessLog (
                        SortMainId,
                        Result,
                        ResultDesc
                    )
                    VALUES (
                        @SortMainId,
                        @Result,
                        @ResultDesc
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                processLog.ProcessLogId = Config.Conn.Query<int>(sql, processLog).Single();
            }
            return processLog;
        }
    }
}
