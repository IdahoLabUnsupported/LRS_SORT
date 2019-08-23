using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class AuthorObject
    {
        #region Properties

        public int AuthorId { get; set; }
        public int MainId { get; set; }
        [Display(Name = "Author Name")]
        public string Name { get; set; }
        public string Affiliation { get; set; }
        public string Email { get; set; }
        public string OrcidId { get; set; }
        public bool IsPrimary { get; set; }
        public string EmployeeId { get; set; }
        public string AffiliationType { get; set; }
        public string WorkOrg { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }

        #endregion

        #region Extended Properties

        public string CountryState
        {
            get
            {
                string data = string.Empty;
                if (AffiliationEnum != AuthorAffilitionEnum.INL)
                {
                    if (CountryId.HasValue)
                    {
                        if (CountryId.Value == Helper.UnitedStatesCountryId && StateId.HasValue)
                        {
                            data = $"{MemoryCache.GetCountry(CountryId.Value)?.Country} / {MemoryCache.GetState(StateId.Value)?.StateName}";
                        }
                        else
                        {
                            data = MemoryCache.GetCountry(CountryId.Value)?.Country;
                        }
                    }
                }
                return data;
            }
        }

        public AuthorAffilitionEnum AffiliationEnum
        {
            get => AffiliationType.ToEnum<AuthorAffilitionEnum>();
            set => AffiliationType = value.ToString();
        }

        public bool IsValid
        {
            get
            {
                bool valid = true;
                try
                {
                    if (string.IsNullOrWhiteSpace(Affiliation)) valid = false;
                    if (string.IsNullOrWhiteSpace(Name)) valid = false;
                    if (AffiliationEnum == AuthorAffilitionEnum.INL && string.IsNullOrWhiteSpace(EmployeeId)) valid = false;
                }
                catch 
                {
                    valid = false;
                }

                return valid;
            }
        }

        public string MainTitle => MainObject.GetMain(MainId)?.DisplayTitle;
        #endregion

        #region Constructor

        public AuthorObject() { }

        #endregion

        #region Repository

        private static IAuthorRepository repo => new AuthorRepository();

        #endregion

        #region Static Methods

        public static List<AuthorObject> GetAuthors(int mainId, IDbConnection conn = null) => repo.GetAuthors(mainId, conn);

        public static AuthorObject GetAuthor(int authorId) => repo.GetAuthor(authorId);

        public static bool AuthorExists(int mainId, string EmployeeId) => repo.AuthorExists(mainId, EmployeeId);

        public static bool Add(int? authorId, int mainId, string EmployeeId, string name, AuthorAffilitionEnum affiliationType, string affiliation, string orcidId, bool isPrimary, int? countryId, int? stateId)
        {
            if (!authorId.HasValue && AuthorExists(mainId, EmployeeId))
            {
                return false;
            }

            AuthorObject ao = GetAuthor(authorId??0) ?? new AuthorObject();
            ao.MainId = mainId;
            ao.Affiliation = affiliation;
            ao.OrcidId = orcidId;
            ao.EmployeeId = EmployeeId;
            ao.AffiliationEnum = affiliationType;
            ao.SetName(name, affiliationType, EmployeeId);
            if (affiliationType != AuthorAffilitionEnum.INL)
            {
                ao.CountryId = countryId;
                if (countryId.HasValue && countryId.Value == Helper.UnitedStatesCountryId)
                {
                    ao.StateId = stateId;
                }
            }
            ao.Save();
            if (isPrimary)
            {
                ao.SetAsPrimary();
            }
            
            return true;
        }

        public static bool CopyData(int fromMainId, int toMainId) => repo.CopyData(fromMainId, toMainId);
        #endregion

        #region Object Methods

        public void SetName(string name, AuthorAffilitionEnum affiliationType, string employeeId)
        {
            string email = string.Empty;
            if (affiliationType == AuthorAffilitionEnum.INL)
            {
                Affiliation = AuthorAffilitionEnum.INL.GetEnumDisplayName();
                var person = EmployeeCache.GetEmployee(employeeId, true);
                if (person != null)
                {
                    email = person.Email;
                    WorkOrg = person.WorkOrginization;
                }
            }
            Name = name;
            Email = email;
            EmployeeId = employeeId;
            AffiliationEnum = affiliationType;
        }

        public void Save()
        {
            repo.SaveAuthor(this);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public void Delete()
        {
            repo.DeleteAuthor(this);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public void SetAsPrimary()
        {
            repo.ClearPrimary(MainId);
            IsPrimary = true;
            Save();
        }

        #endregion
    }

    public interface IAuthorRepository
    {
        List<AuthorObject> GetAuthors(int mainId, IDbConnection conn = null);
        AuthorObject GetAuthor(int authorId);
        AuthorObject SaveAuthor(AuthorObject author);
        bool DeleteAuthor(AuthorObject author);
        void ClearPrimary(int sortMainId);
        bool AuthorExists(int mainId, string EmployeeId);
        bool CopyData(int fromMainId, int toMainId);
    }

    public class AuthorRepository : IAuthorRepository
    {
        public List<AuthorObject> GetAuthors(int mainId, IDbConnection conn = null) => (conn ?? Config.Conn).Query<AuthorObject>("SELECT * FROM dat_Author WHERE MainId = @MainId", new {MainId = mainId}).ToList();

        public AuthorObject GetAuthor(int authorId) => Config.Conn.Query<AuthorObject>("SELECT * FROM dat_Author WHERE AuthorId = @AuthorId", new { AuthorId = authorId }).FirstOrDefault();

        public bool AuthorExists(int mainId, string EmployeeId) => Config.Conn.QueryFirst<bool>("IF EXISTS(SELECT 1 FROM dat_Author WHERE MainId = @MainId AND EmployeeId = @EmployeeId)SELECT 1 ELSE SELECT 0", new {MainId = mainId, EmployeeId = EmployeeId});

        public AuthorObject SaveAuthor(AuthorObject author)
        {
            if (author.AuthorId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Author
                    SET     MainId = @MainId,
                            [Name] = @Name,
                            Affiliation = @Affiliation,
                            Email = @Email,
                            OrcidId = @OrcidId,
                            IsPrimary = @IsPrimary,
                            EmployeeId = @EmployeeId,
                            AffiliationType = @AffiliationType,
                            WorkOrg = @WorkOrg,
                            CountryId = @CountryId,
                            StateId = @StateId
                    WHERE   AuthorId = @AuthorId";
                Config.Conn.Execute(sql, author);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Author (
                        MainId,
                        [Name],
                        Affiliation,
                        Email,
                        OrcidId,
                        IsPrimary,
                        EmployeeId,
                        AffiliationType,
                        WorkOrg,
                        CountryId,
                        StateId
                    )
                    VALUES (
                        @MainId,
                        @Name,
                        @Affiliation,
                        @Email,
                        @OrcidId,
                        @IsPrimary,
                        @EmployeeId,
                        @AffiliationType,
                        @WorkOrg,
                        @CountryId,
                        @StateId
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                author.AuthorId = Config.Conn.Query<int>(sql, author).Single();
            }
            return author;
        }

        public bool DeleteAuthor(AuthorObject author)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_Author WHERE AuthorId = @AuthorId", author);
            }
            catch { return false; }
            return true;
        }

        public void ClearPrimary(int mainId) => Config.Conn.Execute("UPDATE dat_Author SET IsPrimary = 0 WHERE MainId = @MainId", new { MainId = mainId });

        public bool CopyData(int fromMainId, int toMainId)
        {
            string sql = @" insert into dat_Author (MainId, [Name], Affiliation, Email, OrcidId, IsPrimary, EmployeeId, AffiliationType, WorkOrg, CountryId, StateId)
                            select	@NewMainId, [Name], Affiliation, Email, OrcidId, IsPrimary, EmployeeId, AffiliationType, WorkOrg, CountryId, StateId
                            FROM dat_Author
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