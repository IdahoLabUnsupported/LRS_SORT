using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace Sort.Business
{
    public class ContactObject
    {
        #region Properties

        public int ContactId { get; set; }
        public int SortMainId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ContactType { get; set; }
        public string Phone { get; set; }
        public string EmployeeId { get; set; }
        public string WorkOrg { get; set; }
        #endregion

        #region Extended Properties

        [Display(Name = "Contact Name")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        public ContactTypeEnum ContactTypeEnum
        {
            get { return ContactType.ToEnum<ContactTypeEnum>(); }
            set { ContactType = value.ToString(); }
        }

        public string ContactTypeDisplayName => ContactTypeEnum.GetEnumDisplayName();

        public bool IsValid
        {
            get
            {
                bool valid = true;
                try
                {
                    if (string.IsNullOrWhiteSpace(ContactType)) valid = false;
                    if (string.IsNullOrWhiteSpace(FullName)) valid = false;
                    if (string.IsNullOrWhiteSpace(EmployeeId)) valid = false;
                    ContactTypeEnum ct;
                    if (!Enum.TryParse(ContactType, out ct)) valid = false;
                }
                catch(Exception ex)
                {
                    ErrorLogObject.LogError("ContactObject::IsValid", ex);
                    valid = false;
                }

                return valid;
            }
        }

        public string SortTitle { get; set; }
        #endregion

        #region Constructor

        public ContactObject() { }

        #endregion

        #region Repository

        private static IContactRepository repo => new ContactRepository();

        #endregion

        #region Static Methods

        public static List<ContactObject> GetContacts(int sortMainId, IDbConnection conn = null) => repo.GetContacts(sortMainId, conn);

        public static ContactObject GetContact(int contactId) => repo.GetContact(contactId);

        public static void Add(int sortMainId, string employeeId, string contactType)
        {
            ContactObject co = new ContactObject();
            co.SortMainId = sortMainId;
            co.ContactType = contactType;
            if (co.SetNames(employeeId))
            {
                co.Save();
            }
        }

        #endregion

        #region Object Methods

        public bool SetNames(string employeeId)
        {
            var person = EmployeeCache.GetEmployee(employeeId, true);
            if (person != null)
            {
                FirstName = person.FirstName;
                MiddleName = person.MiddleName;
                LastName = person.LastName;
                Phone = person.PhoneNumber;
                EmployeeId = person.EmployeeId;
                WorkOrg = person.WorkOrginization;

                return true;
            }

            return false;
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(EmployeeId) && !string.IsNullOrWhiteSpace(FullName))
            {
                var owner = Config.EmployeeManager.GetEmployeeByName(FullName, true);
                if (owner != null)
                {
                    EmployeeId = owner.EmployeeId;
                    WorkOrg = owner.WorkOrginization;
                }
            }

            if (!string.IsNullOrWhiteSpace(EmployeeId) && string.IsNullOrWhiteSpace(WorkOrg))
            {
                WorkOrg = EmployeeCache.GetEmployee(EmployeeId, true)?.WorkOrginization;
            }

            repo.SaveContact(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        public void Delete()
        {
            repo.DeleteContact(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        #endregion
    }

    public interface IContactRepository
    {
        List<ContactObject> GetContacts(int sortMainId, IDbConnection conn = null);
        ContactObject GetContact(int contactId);
        ContactObject SaveContact(ContactObject contact);
        bool DeleteContact(ContactObject contact);
    }

    public class ContactRepository : IContactRepository
    {
        public List<ContactObject> GetContacts(int sortMainId, IDbConnection conn = null)
        {
            string sql = @" SELECT c.*, s.Title as 'SortTitle'
                            FROM dat_Contact c 
                            inner join dat_SortMain s on s.SortMainId = c.SortMainId
                            WHERE c.SortMainId = @SortMainId";

            return (conn ?? Config.Conn).Query<ContactObject>(sql, new { SortMainId = sortMainId }).ToList();
        }

        public ContactObject GetContact(int contactId)
        {
            string sql = @" SELECT c.*, s.Title as 'SortTitle'
                            FROM dat_Contact c 
                            inner join dat_SortMain s on s.SortMainId = c.SortMainId
                            WHERE c.ContactId = @ContactId";

            return Config.Conn.Query<ContactObject>(sql, new { ContactId = contactId }).FirstOrDefault();
        }

        public ContactObject SaveContact(ContactObject contact)
        {
            if (contact.ContactId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Contact
                    SET     SortMainId = @SortMainId,
                            FirstName = @FirstName,
                            MiddleName = @MiddleName,
                            LastName = @LastName,
                            ContactType = @ContactType,
                            Phone = @Phone,
                            EmployeeId = @EmployeeId,
                            WorkOrg = @WorkOrg
                    WHERE   ContactId = @ContactId";
                Config.Conn.Execute(sql, contact);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Contact (
                        SortMainId,
                        FirstName,
                        MiddleName,
                        LastName,
                        ContactType,
                        Phone,
                        EmployeeId,
                        WorkOrg
                    )
                    VALUES (
                        @SortMainId,
                        @FirstName,
                        @MiddleName,
                        @LastName,
                        @ContactType,
                        @Phone,
                        @EmployeeId,
                        @WorkOrg
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                contact.ContactId = Config.Conn.Query<int>(sql, contact).Single();
            }
            return contact;
        }

        public bool DeleteContact(ContactObject contact)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_Contact WHERE ContactId = @ContactId", contact);
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("ContactObject::IsValid", ex);
                return false;
            }
            return true;
        }
    }
}