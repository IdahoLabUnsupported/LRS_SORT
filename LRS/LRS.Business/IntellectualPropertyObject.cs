using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class IntellectualPropertyObject
    {
        #region Properties

        public int IntellectualPropertyId { get; set; }
        public int MainId { get; set; }
        public string IdrNumber { get; set; }
        public string DocketNumber { get; set; }
        public string Aty { get; set; }
        public string Ae { get; set; }
        public string Title { get; set; }

        #endregion

        #region Extended Properties

        public string MainTitle => MainObject.GetMain(MainId)?.DisplayTitle;

        #endregion

        #region Constructor

        public IntellectualPropertyObject() { }

        #endregion

        #region Repository

        private static IIntellectualPropertyRepository repo => new IntellectualPropertyRepository();

        #endregion

        #region Static Methods

        public static List<IntellectualPropertyObject> GetIntellectualProperties(int mainId) => repo.GetIntellectualProperties(mainId);

        public static IntellectualPropertyObject GetIntellectualProperty(int intellectualPropertyId) => repo.GetIntellectualProperty(intellectualPropertyId);

        public static bool CopyData(int fromMainId, int toMainId) => repo.CopyData(fromMainId, toMainId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveIntellectualProperty(this);
            MainObject.CheckReviewsForChanges(MainId);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public void Delete()
        {
            repo.DeleteIntellectualProperty(this);
            MainObject.CheckReviewsForChanges(MainId);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        #endregion
    }

    public interface IIntellectualPropertyRepository
    {
        List<IntellectualPropertyObject> GetIntellectualProperties(int mainId);
        IntellectualPropertyObject GetIntellectualProperty(int intellectualPropertyId);
        IntellectualPropertyObject SaveIntellectualProperty(IntellectualPropertyObject intellectualProperty);
        bool DeleteIntellectualProperty(IntellectualPropertyObject intellectualProperty);
        bool CopyData(int fromMainId, int toMainId);
    }

    public class IntellectualPropertyRepository : IIntellectualPropertyRepository
    {
        public List<IntellectualPropertyObject> GetIntellectualProperties(int mainId) => Config.Conn.Query<IntellectualPropertyObject>("SELECT * FROM dat_IntellectualProperty WHERE MainId = @MainId", new { MainId = mainId }).ToList();

        public IntellectualPropertyObject GetIntellectualProperty(int intellectualPropertyId) => Config.Conn.Query<IntellectualPropertyObject>("SELECT * FROM dat_IntellectualProperty WHERE IntellectualPropertyId = @IntellectualPropertyId", new { IntellectualPropertyId = intellectualPropertyId }).FirstOrDefault();

        public IntellectualPropertyObject SaveIntellectualProperty(IntellectualPropertyObject intellectualProperty)
        {
            if (intellectualProperty.IntellectualPropertyId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_IntellectualProperty
                    SET     MainId = @MainId,
                            IdrNumber = @IdrNumber,
                            DocketNumber = @DocketNumber,
                            Aty = @Aty,
                            Ae = @Ae,
                            Title = @Title
                    WHERE   IntellectualPropertyId = @IntellectualPropertyId";
                Config.Conn.Execute(sql, intellectualProperty);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_IntellectualProperty (
                        MainId,
                        IdrNumber,
                        DocketNumber,
                        Aty,
                        Ae,
                        Title
                    )
                    VALUES (
                        @MainId,
                        @IdrNumber,
                        @DocketNumber,
                        @Aty,
                        @Ae,
                        @Title
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                intellectualProperty.IntellectualPropertyId = Config.Conn.Query<int>(sql, intellectualProperty).Single();
            }
            return intellectualProperty;
        }

        public bool DeleteIntellectualProperty(IntellectualPropertyObject intellectualProperty)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_IntellectualProperty WHERE IntellectualPropertyId = @IntellectualPropertyId", intellectualProperty);
            }
            catch { return false; }
            return true;
        }

        public bool CopyData(int fromMainId, int toMainId)
        {
            string sql = @" insert into dat_IntellectualProperty (MainId, IdrNumber, DocketNumber, Aty, Ae, Title)
                            select @NewMainId, IdrNumber, DocketNumber, Aty, Ae, Title
                            FROM dat_IntellectualProperty
                            WHERE MainId = @OldMainId";

            try
            {
                Config.Conn.Execute(sql, new { NewMainId = toMainId, OldMainId = fromMainId });
            }
            catch { return false; }
            return true;
        }
    }
}
