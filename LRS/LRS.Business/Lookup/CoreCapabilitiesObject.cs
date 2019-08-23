using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class CoreCapabilitiesObject
    {
        #region Properties

        public int CoreCapabilitiesId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public CoreCapabilitiesObject() { }

        #endregion

        #region Repository

        private static ICoreCapabilitiesRepository repo => new CoreCapabilitiesRepository();

        #endregion

        #region Static Methods

        internal static List<CoreCapabilitiesObject> GetCoreCapabilitiess() => repo.GetCoreCapabilitiess();

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveCoreCapabilities(this);
            MemoryCache.ClearCoreCapabilities();
        }

        public void Delete()
        {
            Active = false;
            Save();
        }

        #endregion
    }

    public interface ICoreCapabilitiesRepository
    {
        List<CoreCapabilitiesObject> GetCoreCapabilitiess();
        CoreCapabilitiesObject SaveCoreCapabilities(CoreCapabilitiesObject coreCapabilities);
    }

    public class CoreCapabilitiesRepository : ICoreCapabilitiesRepository
    {
        public List<CoreCapabilitiesObject> GetCoreCapabilitiess() => Config.Conn.Query<CoreCapabilitiesObject>("SELECT * FROM lu_CoreCapabilities").ToList();

        public CoreCapabilitiesObject SaveCoreCapabilities(CoreCapabilitiesObject coreCapabilities)
        {
            if (coreCapabilities.CoreCapabilitiesId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_CoreCapabilities
                    SET     Name = @Name,
                            Active = @Active
                    WHERE   CoreCapabilitiesId = @CoreCapabilitiesId";
                Config.Conn.Execute(sql, coreCapabilities);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_CoreCapabilities (
                        Name,
                        Active
                    )
                    VALUES (
                        @Name,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                coreCapabilities.CoreCapabilitiesId = Config.Conn.Query<int>(sql, coreCapabilities).Single();
            }
            return coreCapabilities;
        }
    }
}
