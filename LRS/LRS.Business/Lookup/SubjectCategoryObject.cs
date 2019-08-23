using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class SubjectCategoryObject
    {
        #region Properties

        public int SubjectCategoryId { get; set; }
        public string CategoryId { get; set; }
        public string Subject { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Extended Properties

        public string FullSubject => $"{CategoryId} - {Subject}";
        #endregion

        #region Constructor

        public SubjectCategoryObject() { }

        #endregion

        #region Repository

        private static ISubjectCategoryRepository repo => new SubjectCategoryRepository();

        #endregion

        #region Static Methods

        public static List<SubjectCategoryObject> GetSubjectCategories() => repo.GetSubjectCategories();

        internal static SubjectCategoryObject GetSubjectCategory(int subjectCategoryId) => repo.GetSubjectCategory(subjectCategoryId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveSubjectCategory(this);
            MemoryCache.ClearSubjectCategories();
        }

        public void Delete()
        {
            repo.DeleteSubjectCategory(this);
            MemoryCache.ClearSubjectCategories();
        }

        #endregion
    }

    public interface ISubjectCategoryRepository
    {
        List<SubjectCategoryObject> GetSubjectCategories();
        SubjectCategoryObject GetSubjectCategory(int subjectCategoryId);
        SubjectCategoryObject SaveSubjectCategory(SubjectCategoryObject subjectCategory);
        bool DeleteSubjectCategory(SubjectCategoryObject subjectCategory);
    }

    public class SubjectCategoryRepository : ISubjectCategoryRepository
    {
        public List<SubjectCategoryObject> GetSubjectCategories() => Config.Conn.Query<SubjectCategoryObject>("SELECT * FROM lu_SubjectCategory").ToList();

        public SubjectCategoryObject GetSubjectCategory(int subjectCategoryId) => Config.Conn.Query<SubjectCategoryObject>("SELECT * FROM lu_SubjectCategory WHERE SubjectCategoryId = @SubjectCategoryId", new { SubjectCategoryId = subjectCategoryId }).FirstOrDefault();

        public SubjectCategoryObject SaveSubjectCategory(SubjectCategoryObject subjectCategory)
        {
            if (subjectCategory.SubjectCategoryId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_SubjectCategory
                    SET     CategoryId = @CategoryId,
                            Subject = @Subject,
                            Active = @Active
                    WHERE   SubjectCategoryId = @SubjectCategoryId";
                Config.Conn.Execute(sql, subjectCategory);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_SubjectCategory (
                        CategoryId,
                        Subject,
                        Active
                    )
                    VALUES (
                        @CategoryId,
                        @Subject,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                subjectCategory.SubjectCategoryId = Config.Conn.Query<int>(sql, subjectCategory).Single();
            }
            return subjectCategory;
        }

        public bool DeleteSubjectCategory(SubjectCategoryObject subjectCategory)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_SubjectCategory WHERE SubjectCategoryId = @SubjectCategoryId", subjectCategory);
            }
            catch { return false; }
            return true;
        }
    }
}
