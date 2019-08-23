using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using Dapper;

namespace Sort.Business
{
    public class AuthorObject
    {
        #region Properties
        public int? AuthorId { get; set; }
        public int SharePointId { get; set; }
        public int SortMainId { get; set; }
        [Display(Name = "First Name", ShortName = "first_name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name", ShortName = "middle_name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name", ShortName = "last_name")]
        public string LastName { get; set; }
        [Display(Name = "Affiliation", ShortName = "affiliation")]
        public string Affiliation { get; set; }
        [Display(Name = "Email", ShortName = "private_email")]
        public string Email { get; set; }
        [Display(Name = "ORCID Id", ShortName = "orcid_id")]
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
                if (AffiliationEnum != AffiliationEnum.INL)
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

        [Display(Name="Author Name")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        public AffiliationEnum AffiliationEnum
        {
            get { return AffiliationType.ToEnum<AffiliationEnum>(); }
            set { AffiliationType = value.ToString(); }
        }

        public bool IsValid
        {
            get
            {
                bool valid = true;
                try
                {
                    if (string.IsNullOrWhiteSpace(Affiliation)) valid = false;
                    if (string.IsNullOrWhiteSpace(FullName)) valid = false;
                    if (AffiliationEnum == AffiliationEnum.INL && string.IsNullOrWhiteSpace(EmployeeId)) valid = false;
                }
                catch (Exception ex)
                {
                    ErrorLogObject.LogError("AuthorObject::IsValid", ex);
                    valid = false;
                }

                return valid;
            }
        }

        public string SortTitle { get; set; }
        #endregion

        #region Constructor

        public AuthorObject() { }

        #endregion

        #region Repository

        private static IAuthorRepository repo => new AuthorRepository();

        #endregion
        
        #region Static Methods

        public static List<AuthorObject> GetAuthors(int sortMainId, IDbConnection conn = null) => repo.GetAuthors(sortMainId, conn);

        public static AuthorObject GetAuthor(int authorId) => repo.GetAuthor(authorId);

        public static void Add(int? authorId, int sortMainId, string employeeId, string name, AffiliationEnum affiliationType, string affiliation, string orcidId, bool isPrimary, int? countryId, int? stateId)
        {
            AuthorObject ao = new AuthorObject();
            if (authorId.HasValue)
                ao = AuthorObject.GetAuthor(authorId.Value);
            ao.SortMainId = sortMainId;
            ao.Affiliation = affiliation;
            ao.OrcidId = orcidId;
            ao.EmployeeId = employeeId;
            ao.AffiliationEnum = affiliationType;
            ao.SetName(name, affiliationType, employeeId);
            if (affiliationType != AffiliationEnum.INL)
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
        }
        #endregion

        #region Object Methods

        public void SetName(string name, AffiliationEnum affiliationType, string employeeId)
        {
            string fName = name.FirstName();
            string mName = name.MiddelName();
            string lName = name.LastName();
            string email = string.Empty;
            if (affiliationType == AffiliationEnum.INL)
            {
                Affiliation = "Idaho National Laboratory";
                var person = EmployeeCache.GetEmployee(employeeId, true);
                if (person != null)
                {
                    email = person.Email;
                    WorkOrg = person.WorkOrginization;
                }
            }
            FirstName = fName;
            MiddleName = mName;
            LastName = lName;
            Email = email;
            EmployeeId = employeeId;
            AffiliationEnum = affiliationType;
        }

        public void Save()
        {
            if (AffiliationEnum == AffiliationEnum.INL && string.IsNullOrWhiteSpace(EmployeeId) && !string.IsNullOrWhiteSpace(FullName))
            {
                var owner = Config.EmployeeManager.GetEmployeeByName(FullName, true);
                if (owner != null)
                {
                    EmployeeId = owner.EmployeeId;
                    Email = owner.Email;
                    WorkOrg = owner.WorkOrginization;
                }
            }

            if (!string.IsNullOrWhiteSpace(EmployeeId) && string.IsNullOrWhiteSpace(WorkOrg))
            {
                WorkOrg = EmployeeCache.GetEmployee(EmployeeId, true)?.WorkOrginization;
            }

            repo.SaveAuthor(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        public void Delete()
        {
            repo.DeleteAuthor(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        public void SetAsPrimary()
        {
            repo.ClearPrimary(SortMainId);
            IsPrimary = true;
            Save();
        }

        #endregion

    }

    public interface IAuthorRepository
    {
        List<AuthorObject> GetAuthors(int sortMainId, IDbConnection conn = null);
        AuthorObject GetAuthor(int authorId);
        AuthorObject SaveAuthor(AuthorObject author);
        bool DeleteAuthor(AuthorObject author);
        void ClearPrimary(int sortMainId);
    }

    public class AuthorRepository : IAuthorRepository
    {
        public List<AuthorObject> GetAuthors(int sortMainId, IDbConnection conn = null)
        {
            string sql = @" SELECT a.*, s.Title as 'SortTitle'
                            FROM dat_Author a 
                            inner join dat_SortMain s on s.SortMainId = a.SortMainId
                            WHERE a.SortMainId = @SortMainId 
                            order by a.IsPrimary desc";

            return (conn ?? Config.Conn).Query<AuthorObject>(sql, new { SortMainId = sortMainId }).ToList();
        }

        public AuthorObject GetAuthor(int authorId)
        {
            string sql = @" SELECT a.*, s.Title as 'SortTitle'
                            FROM dat_Author a 
                            inner join dat_SortMain s on s.SortMainId = a.SortMainId
                            WHERE a.AuthorId = @AuthorId";

            return Config.Conn.Query<AuthorObject>(sql, new { AuthorId = authorId }).FirstOrDefault();
        } 

        public AuthorObject SaveAuthor(AuthorObject author)
        {
            if (author.AuthorId.HasValue) // Update
            {
                string sql = @"
                    UPDATE  dat_Author
                    SET     SortMainId = @SortMainId,
                            SharePointId = @SharePointId,
                            FirstName = @FirstName,
                            MiddleName = @MiddleName,
                            LastName = @LastName,
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
                        SortMainId,
                        SharePointId,
                        FirstName,
                        MiddleName,
                        LastName,
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
                        @SortMainId,
                        @SharePointId,
                        @FirstName,
                        @MiddleName,
                        @LastName,
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
            catch (Exception ex)
            {
                ErrorLogObject.LogError("AuthorObject::DeleteAuthor", ex);
                return false;
            }
            return true;
        }

        public void ClearPrimary(int sortMainId) => Config.Conn.Execute("UPDATE dat_Author SET IsPrimary = 0 WHERE SortMainId = @SortMainId", new { SortMainId = sortMainId });
    }
}
