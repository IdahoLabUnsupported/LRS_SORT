using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class EmailTemplateObject
    {
        #region Properties

        public int EmailTemplateId { get; set; }
        public string EmailType { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string EditedByEmployeeId { get; set; }
        public DateTime LastEditDate { get; set; }
        public bool IncludeAuthors { get; set; }
        public bool IncludeContacts { get; set; }

        #endregion

        #region Extended Properties
        public EmailTypeEnum EmailTypeEnum
        {
            get { return EmailType.ToEnum<EmailTypeEnum>(); }
            set { EmailType = value.ToString(); }
        }
        #endregion

        #region Constructor

        public EmailTemplateObject() { }

        #endregion

        #region Repository

        private static IEmailTemplateRepository repo => new EmailTemplateRepository();

        #endregion

        #region Static Methods

        public static List<EmailTemplateObject> GetEmailTemplates() => repo.GetEmailTemplates();

        public static EmailTemplateObject GetEmailTemplate(int emailTemplateId) => repo.GetEmailTemplate(emailTemplateId);

        public static EmailTemplateObject GetEmailTemplate(EmailTypeEnum emailType) => repo.GetEmailTemplate(emailType);

        #endregion

        #region Object Methods

        public void Save()
        {
            if (EmailTemplateId > 0 && string.IsNullOrWhiteSpace(Header) && string.IsNullOrWhiteSpace(Body))
            {
                Delete();
            }
            else
            {
                EditedByEmployeeId = UserObject.CurrentUser?.EmployeeId;
                LastEditDate = DateTime.Now;
                repo.SaveEmailTemplate(this);
            }
        }

        public bool Delete()
        {
            return repo.DeleteEmailTemplate(this);
        }

        #endregion
    }

    public interface IEmailTemplateRepository
    {
        List<EmailTemplateObject> GetEmailTemplates();
        EmailTemplateObject GetEmailTemplate(int emailTemplateId);
        EmailTemplateObject GetEmailTemplate(EmailTypeEnum emailType);
        EmailTemplateObject SaveEmailTemplate(EmailTemplateObject emailTemplate);
        bool DeleteEmailTemplate(EmailTemplateObject emailTemplate);
    }

    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        public List<EmailTemplateObject> GetEmailTemplates() => Config.Conn.Query<EmailTemplateObject>("SELECT * FROM dat_EmailTemplate").ToList();

        public EmailTemplateObject GetEmailTemplate(int emailTemplateId) => Config.Conn.QueryFirstOrDefault<EmailTemplateObject>("SELECT * FROM dat_EmailTemplate WHERE EmailTemplateId = @EmailTemplateId", new { EmailTemplateId = emailTemplateId });

        public EmailTemplateObject GetEmailTemplate(EmailTypeEnum emailType) => Config.Conn.QueryFirstOrDefault<EmailTemplateObject>("SELECT * FROM dat_EmailTemplate WHERE EmailType = @EmailType", new { EmailType = emailType.ToString() });

        public EmailTemplateObject SaveEmailTemplate(EmailTemplateObject emailTemplate)
        {
            if (emailTemplate.EmailTemplateId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_EmailTemplate
                    SET     Header = @Header,
                            Body = @Body,
                            EditedByEmployeeId = @EditedByEmployeeId,
                            LastEditDate = @LastEditDate,
                            IncludeAuthors = @IncludeAuthors,
                            IncludeContacts = @IncludeContacts
                    WHERE   EmailTemplateId = @EmailTemplateId";
                Config.Conn.Execute(sql, emailTemplate);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_EmailTemplate (
                        EmailType,
                        Header,
                        Body,
                        EditedByEmployeeId,
                        LastEditDate,
                        IncludeAuthors,
                        IncludeContacts
                    )
                    VALUES (
                        @EmailType,
                        @Header,
                        @Body,
                        @EditedByEmployeeId,
                        @LastEditDate,
                        @IncludeAuthors,
                        @IncludeContacts
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                emailTemplate.EmailTemplateId = Config.Conn.Query<int>(sql, emailTemplate).Single();
            }
            return emailTemplate;
        }

        public bool DeleteEmailTemplate(EmailTemplateObject emailTemplate)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_EmailTemplate WHERE EmailTemplateId = @EmailTemplateId", emailTemplate);
            }
            catch { return false; }
            return true;
        }
    }
}
