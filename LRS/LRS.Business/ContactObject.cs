using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class ContactObject
    {
        #region Properties

        public int ContactId { get; set; }
        public int MainId { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        [Display(Name = "Name")]
        public string EmployeeId { get; set; }
        [Display(Name = "Org")]
        public string WorkOrg { get; set; }
        public string OrcidId { get; set; }
        [Display(Name = "Contact Name")]
        public string Name { get; set; }
        #endregion

        #region Extended Properties

        public bool IsValid
        {
            get
            {
                bool valid = true;
                try
                {
                    if (string.IsNullOrWhiteSpace(Name)) valid = false;
                    if (string.IsNullOrWhiteSpace(EmployeeId)) valid = false;
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

        public ContactObject() { }

        #endregion

        #region Repository

        private static IContactRepository repo => new ContactRepository();

        #endregion

        #region Static Methods

        public static List<ContactObject> GetContacts(int mainId, IDbConnection conn = null) => repo.GetContacts(mainId, conn);

        public static ContactObject GetContact(int contactId) => repo.GetContact(contactId);

        public static void Add(int mainId, string EmployeeId)
        {
            ContactObject co = new ContactObject();
            co.MainId = mainId;
            if (co.SetNames(EmployeeId))
            {
                co.Save();
            }
        }

        public static bool CopyData(int fromMainId, int toMainId) => repo.CopyData(fromMainId, toMainId);
        #endregion

        #region Object Methods

        public bool SetNames(string EmployeeId)
        {
            var person = EmployeeCache.GetEmployee(EmployeeId, true);
            if (person != null)
            {
                Name = person.PreferredName;
                Phone = person.PhoneNumber;
                OrcidId = person.OrcidId;
                Location = person.WorkLocation;
                EmployeeId = person.EmployeeId;
                WorkOrg = person.WorkOrginization;

                return true;
            }

            return false;
        }


        public void Save()
        {
            repo.SaveContact(this);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public void Delete()
        {
            repo.DeleteContact(this);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public bool SaveAsAuthor(ref string title, ref string message)
        {
            if (AuthorObject.Add(null, MainId, EmployeeId, Name, AuthorAffilitionEnum.INL, "Idaho National Laboratory", OrcidId, false, null, null))
            {
                title = "Author Added";
                message = "Contact was successfully added to the Authors list.";
                return true;
            }

            message = "Contact is already an Author. Can not add the Contact more than once.";
            return false;
        }
        #endregion
    }

    public interface IContactRepository
    {
        List<ContactObject> GetContacts(int mainId, IDbConnection conn = null);
        ContactObject GetContact(int contactId);
        ContactObject SaveContact(ContactObject contact);
        bool DeleteContact(ContactObject contact);
        bool CopyData(int fromMainId, int toMainId);
    }

    public class ContactRepository : IContactRepository
    {
        public List<ContactObject> GetContacts(int mainId, IDbConnection conn = null) => (conn ?? Config.Conn).Query<ContactObject>("SELECT * FROM dat_Contact WHERE MainId = @MainId", new { MainId = mainId }).ToList();

        public ContactObject GetContact(int contactId) => Config.Conn.Query<ContactObject>("SELECT * FROM dat_Contact WHERE ContactId = @ContactId", new { ContactId = contactId }).FirstOrDefault();

        public ContactObject SaveContact(ContactObject contact)
        {
            if (contact.ContactId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Contact
                    SET     [Name] = @Name,
                            Phone = @Phone,
                            Location = @Location,
                            EmployeeId = @EmployeeId,
                            WorkOrg = @WorkOrg,
                            OrcidId = @OrcidId
                    WHERE   ContactId = @ContactId";
                Config.Conn.Execute(sql, contact);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Contact (
                        MainId,
                        [Name],
                        Phone,
                        Location,
                        EmployeeId,
                        WorkOrg,
                        OrcidId
                    )
                    VALUES (
                        @MainId,
                        @Name,
                        @Phone,
                        @Location,
                        @EmployeeId,
                        @WorkOrg,
                        @OrcidId
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
            catch { return false; }
            return true;
        }

        public bool CopyData(int fromMainId, int toMainId)
        {
            string sql = @" insert into dat_Contact (MainId, [Name], Phone, EmployeeId, WorkOrg, [Location], OrcidId)
                            select	@NewMainId, [Name], Phone, EmployeeId, WorkOrg, [Location], OrcidId
                            FROM dat_Contact
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