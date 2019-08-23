using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inl.MvcHelper;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class EditEmailModel
    {
        public string EmailType { get; set; }
        public int? EmailTemplateId { get; set; }
        [Required, Display(Name = "Subject")]
        public string Header { get; set; }
        [Required, AllowHtml]
        public string Body { get; set; }
        public bool IsSaved { get; set; }
        [Required, Display(Name = "CC: All Authors")]
        public bool IncludeAuthors { get; set; }
        [Required, Display(Name = "CC: All Contacts")]
        public bool IncludeContacts { get; set; }

        public EmailTypeEnum EmailTypeEnum
        {
            get { return EmailType.ToEnum<EmailTypeEnum>(); }
            set { EmailType = value.ToString(); }
        }

        public EditEmailModel()
        {
        }

        public void Save()
        {
            EmailTemplateObject email = new EmailTemplateObject();
            if (EmailTemplateId.HasValue)
                email = EmailTemplateObject.GetEmailTemplate(EmailTemplateId.Value);
            email.EmailType = EmailType;
            email.Header = Header;
            email.Body = Body;
            email.IncludeAuthors = IncludeAuthors;
            email.IncludeContacts = IncludeContacts;
            email.Save();
            EmailTemplateId = email.EmailTemplateId;
            IsSaved = true;
        }

        public void HydrateData()
        {
            var email = EmailTemplateObject.GetEmailTemplate(EmailTypeEnum);
            if (email != null)
            {
                EmailTemplateId = email.EmailTemplateId;
                Header = email.Header;
                Body = email.Body;
                IncludeAuthors = email.IncludeAuthors;
                IncludeContacts = email.IncludeContacts;
            }
            else
            {
                EmailTemplateId = null;
                Header = string.Empty;
                Body = string.Empty;
                IsSaved = false;
                IncludeAuthors = false;
                IncludeContacts = false;
            }
        }
    }
}