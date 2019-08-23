using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class GenericReviewUserObject
    {
        #region Properties

        public int? GenericUserId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public string ReviewerType { get; set; }

        #endregion

        #region Extended Properties

        public ReviewerTypeEnum ReviewerTypeEnum
        {
            get => ReviewerType.ToEnum<ReviewerTypeEnum>();
            set => ReviewerType = value.ToString();
        }

        public string Name => UserObject.GetUser(EmployeeId)?.FullName;
        public string Email => UserObject.GetUser(EmployeeId)?.Email;

        #endregion

        #region Constructor

        public GenericReviewUserObject() { }

        #endregion

        #region Repository

        private static IGenericReviewUserRepository repo => new GenericReviewUserRepository();

        #endregion

        #region Static Methods

        internal static List<GenericReviewUserObject> GetGenericReviewUsers() => repo.GetGenericReviewUsers();

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveGenericReviewUser(this);
            MemoryCache.ClearGenericUsers();
        }

        public void Delete()
        {
            repo.DeleteGenericReviewUser(this);
            MemoryCache.ClearGenericUsers();
        }

        #endregion
    }

    public interface IGenericReviewUserRepository
    {
        List<GenericReviewUserObject> GetGenericReviewUsers();
        GenericReviewUserObject GetGenericReviewUser(int genericUserId);
        GenericReviewUserObject SaveGenericReviewUser(GenericReviewUserObject genericReviewUser);
        bool DeleteGenericReviewUser(GenericReviewUserObject genericReviewUser);
    }

    public class GenericReviewUserRepository : IGenericReviewUserRepository
    {
        public List<GenericReviewUserObject> GetGenericReviewUsers() => Config.Conn.Query<GenericReviewUserObject>("SELECT * FROM lu_GenericReviewUser").ToList();

        public GenericReviewUserObject GetGenericReviewUser(int genericUserId) => Config.Conn.Query<GenericReviewUserObject>("SELECT * FROM lu_GenericReviewUser WHERE GenericUserId = @GenericUserId", new { GenericUserId = genericUserId }).FirstOrDefault();

        public GenericReviewUserObject SaveGenericReviewUser(GenericReviewUserObject genericReviewUser)
        {
            if (genericReviewUser.GenericUserId.HasValue) // Update
            {
                string sql = @"
                    UPDATE  lu_GenericReviewUser
                    SET     EmployeeId = @EmployeeId,
                            ReviewerType = @ReviewerType
                    WHERE   GenericUserId = @GenericUserId";
                Config.Conn.Execute(sql, genericReviewUser);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_GenericReviewUser (
                        EmployeeId,
                        ReviewerType
                    )
                    VALUES (
                        @EmployeeId,
                        @ReviewerType
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                genericReviewUser.GenericUserId = Config.Conn.Query<int>(sql, genericReviewUser).Single();
            }
            return genericReviewUser;
        }

        public bool DeleteGenericReviewUser(GenericReviewUserObject genericReviewUser)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_GenericReviewUser WHERE GenericUserId = @GenericUserId", genericReviewUser);
            }
            catch { return false; }
            return true;
        }
    }
}
