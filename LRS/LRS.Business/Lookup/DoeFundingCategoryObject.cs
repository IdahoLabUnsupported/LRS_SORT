using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class DoeFundingCategoryObject
    {
        #region Properties

        public int DoeFundingCategoryId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public DoeFundingCategoryObject() { }

        #endregion

        #region Repository

        private static IDoeFundingCategoryRepository repo => new DoeFundingCategoryRepository();

        #endregion

        #region Static Methods

        public static List<DoeFundingCategoryObject> GetDoeFundingCategories() => repo.GetDoeFundingCategories();

        internal static DoeFundingCategoryObject GetDoeFundingCategory(int doeFundingCategoryId) => repo.GetDoeFundingCategory(doeFundingCategoryId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveDoeFundingCategory(this);
            MemoryCache.ClearDoeFunding();
        }

        public void Delete()
        {
            repo.DeleteDoeFundingCategory(this);
            MemoryCache.ClearDoeFunding();
        }

        #endregion
    }

    public interface IDoeFundingCategoryRepository
    {
        List<DoeFundingCategoryObject> GetDoeFundingCategories();
        DoeFundingCategoryObject GetDoeFundingCategory(int doeFundingCategoryId);
        DoeFundingCategoryObject SaveDoeFundingCategory(DoeFundingCategoryObject doeFundingCategory);
        bool DeleteDoeFundingCategory(DoeFundingCategoryObject doeFundingCategory);
    }

    public class DoeFundingCategoryRepository : IDoeFundingCategoryRepository
    {
        public List<DoeFundingCategoryObject> GetDoeFundingCategories() => Config.Conn.Query<DoeFundingCategoryObject>("SELECT * FROM lu_DoeFundingCategory").ToList();

        public DoeFundingCategoryObject GetDoeFundingCategory(int doeFundingCategoryId) => Config.Conn.Query<DoeFundingCategoryObject>("SELECT * FROM lu_DoeFundingCategory WHERE DoeFundingCategoryId = @DoeFundingCategoryId", new { DoeFundingCategoryId = doeFundingCategoryId }).FirstOrDefault();

        public DoeFundingCategoryObject SaveDoeFundingCategory(DoeFundingCategoryObject doeFundingCategory)
        {
            if (doeFundingCategory.DoeFundingCategoryId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_DoeFundingCategory
                    SET     Category = @Category,
                            Description = @Description,
                            Active = @Active
                    WHERE   DoeFundingCategoryId = @DoeFundingCategoryId";
                Config.Conn.Execute(sql, doeFundingCategory);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_DoeFundingCategory (
                        Category,
                        Description,
                        Active
                    )
                    VALUES (
                        @Category,
                        @Description,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                doeFundingCategory.DoeFundingCategoryId = Config.Conn.Query<int>(sql, doeFundingCategory).Single();
            }
            return doeFundingCategory;
        }

        public bool DeleteDoeFundingCategory(DoeFundingCategoryObject doeFundingCategory)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_DoeFundingCategory WHERE DoeFundingCategoryId = @DoeFundingCategoryId", doeFundingCategory);
            }
            catch { return false; }
            return true;
        }
    }
}