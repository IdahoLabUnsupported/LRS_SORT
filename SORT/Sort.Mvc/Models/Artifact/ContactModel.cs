using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class ContactModel : IContact
    {
        #region Properties
        public int? ContactId { get; set; }
        [Required]
        public int SortMainId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public string ContactType { get; set; }
        public List<ContactObject> Contacts { get; set; }
        #endregion

        #region Constructor

        public ContactModel() { }

        public ContactModel(ContactObject contact)
        {
            if (contact != null)
            {
                SortMainId = contact.SortMainId;
                ContactId = contact.ContactId;
                EmployeeId = contact.EmployeeId;
                ContactType = contact.ContactType;

                Contacts = ContactObject.GetContacts(SortMainId);
            }
        }
        #endregion

        #region Functions

        public void Save()
        {
            bool needSave = SortMainId > 0;

            if (string.IsNullOrWhiteSpace(EmployeeId)) needSave = false;
            if (string.IsNullOrWhiteSpace(ContactType)) needSave = false;

            if (needSave)
            {
                if (ContactId.HasValue && ContactId.Value > 0)
                {
                    var contact = ContactObject.GetContact(ContactId.Value);
                    if (contact != null)
                    {
                        contact.ContactType = ContactType;
                        if (contact.SetNames(EmployeeId))
                        {
                            contact.Save();
                        }
                    }
                }
                else
                {
                    ContactObject.Add(SortMainId, EmployeeId, ContactType);
                }

            }
        }
        #endregion
    }
}