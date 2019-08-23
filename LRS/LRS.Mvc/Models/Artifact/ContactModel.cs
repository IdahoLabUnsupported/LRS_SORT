using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class ContactModel
    {
        #region Properties
        public int? ContactId { get; set; }
        [Required]
        public int MainId { get; set; }
        [Required, Display(Name = "Contact Name")]
        public string ContactEmployeeId { get; set; }
        public List<ContactObject> Contacts { get; set; }
        #endregion

        #region Constructor

        public ContactModel() { }

        public ContactModel(int mainId, int? contactId)
        {
            MainId = mainId;
            Contacts = ContactObject.GetContacts(mainId);

            if (contactId.HasValue)
            {
                var contact = ContactObject.GetContact(contactId.Value);
                if (contact != null)
                {
                    ContactId = contact.ContactId;
                    ContactEmployeeId = contact.EmployeeId;
                }
            }
        }
        #endregion

        #region Functions

        public void Save()
        {
            bool needSave = MainId > 0 && !string.IsNullOrWhiteSpace(ContactEmployeeId);

            if (needSave)
            {
                if (ContactId.HasValue && ContactId.Value > 0)
                {
                    var contact = ContactObject.GetContact(ContactId.Value);
                    if (contact != null)
                    {
                        if (contact.SetNames(ContactEmployeeId))
                        {
                            contact.Save();
                        }
                    }
                }
                else
                {
                    ContactObject.Add(MainId, ContactEmployeeId);
                }

            }
        }
        #endregion
    }
}