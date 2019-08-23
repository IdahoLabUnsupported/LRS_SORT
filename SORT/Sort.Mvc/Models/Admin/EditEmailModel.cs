using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sort.Business;

namespace Sort.Mvc.Models
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

        public EditEmailModel()
        {
        }

        public void Save()
        {
            EmailTemplateObject email = new EmailTemplateObject();
            if(EmailTemplateId.HasValue)
                email = EmailTemplateObject.GetEmailTemplate(EmailTemplateId.Value);
            email.EmailType = EmailType;
            email.Header = Header;
            email.Body = Body;
            email.Save();
            EmailTemplateId = email.EmailTemplateId;
            IsSaved = true;
        }

        public void HydrateData()
        {
            var email = EmailTemplateObject.GetEmailTemplate(EmailType);
            if (email != null)
            {
                EmailTemplateId = email.EmailTemplateId;
                Header = email.Header;
                Body = email.Body;
            }
            else
            {
                EmailTemplateId = null;
                Header = string.Empty;
                Body = string.Empty;
                IsSaved = false;
            }
        }
    }
}