using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class SppCategoryObject
    {
        #region Properties

        public int SppCategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string Category { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public SppCategoryObject() { }

        #endregion

        #region Repository

        private static ISppCategoryRepository repo => new SppCategoryRepository();

        #endregion

        #region Static Methods

        internal static List<SppCategoryObject> GetSppCategories() => repo.GetSppCategories();

        internal static SppCategoryObject GetSppCategory(int sppCategoryId) => repo.GetSppCategory(sppCategoryId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveSppCategory(this);
            MemoryCache.ClearSppFunding();
        }

        public void Delete()
        {
            repo.DeleteSppCategory(this);
            MemoryCache.ClearSppFunding();
        }

        #endregion
    }

    public interface ISppCategoryRepository
    {
        List<SppCategoryObject> GetSppCategories();
        SppCategoryObject GetSppCategory(int sppCategoryId);
        SppCategoryObject SaveSppCategory(SppCategoryObject sppCategory);
        bool DeleteSppCategory(SppCategoryObject sppCategory);
    }

    public class SppCategoryRepository : ISppCategoryRepository
    {
        public List<SppCategoryObject> GetSppCategories() => Config.Conn.Query<SppCategoryObject>("SELECT * FROM lu_SppCategory").ToList();

        public SppCategoryObject GetSppCategory(int sppCategoryId) => Config.Conn.Query<SppCategoryObject>("SELECT * FROM lu_SppCategory WHERE SppCategoryId = @SppCategoryId", new { SppCategoryId = sppCategoryId }).FirstOrDefault();

        public SppCategoryObject SaveSppCategory(SppCategoryObject sppCategory)
        {
            if (sppCategory.SppCategoryId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_SppCategory
                    SET     CategoryCode = @CategoryCode,
                            Category = @Category,
                            Active = @Active
                    WHERE   SppCategoryId = @SppCategoryId";
                Config.Conn.Execute(sql, sppCategory);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_SppCategory (
                        CategoryCode,
                        Category,
                        Active
                    )
                    VALUES (
                        @CategoryCode,
                        @Category,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                sppCategory.SppCategoryId = Config.Conn.Query<int>(sql, sppCategory).Single();
            }
            return sppCategory;
        }

        public bool DeleteSppCategory(SppCategoryObject sppCategory)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_SppCategory WHERE SppCategoryId = @SppCategoryId", sppCategory);
            }
            catch { return false; }
            return true;
        }
    }
}
