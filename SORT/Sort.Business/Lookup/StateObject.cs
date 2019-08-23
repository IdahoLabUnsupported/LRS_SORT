using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Sort.Business
{
    public class StateObject
    {
        #region Properties

        public int StateId { get; set; }
        public string StateName { get; set; }
        public string ShortName { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public StateObject() { }

        #endregion

        #region Repository

        private static IStateRepository repo => new StateRepository();

        #endregion

        #region Static Methods

        internal static List<StateObject> GetStates() => repo.GetStates();

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveState(this);
            MemoryCache.ClearStates();
        }

        public void Delete()
        {
            Active = false;
            Save();
        }

        #endregion
    }

    public interface IStateRepository
    {
        List<StateObject> GetStates();
        StateObject SaveState(StateObject state);
    }

    public class StateRepository : IStateRepository
    {
        public List<StateObject> GetStates() => Config.Conn.Query<StateObject>("SELECT * FROM lu_State").ToList();

        public StateObject SaveState(StateObject state)
        {
            if (state.StateId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_State
                    SET     StateName = @StateName,
                            ShortName = @ShortName,
                            Active = @Active
                    WHERE   StateId = @StateId";
                Config.Conn.Execute(sql, state);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_State (
                        StateName,
                        ShortName,
                        Active
                    )
                    VALUES (
                        @StateName,
                        @ShortName,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                state.StateId = Config.Conn.Query<int>(sql, state).Single();
            }
            return state;
        }
    }
}
