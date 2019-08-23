using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Configuration;
using System.Linq;

namespace Sort.Business
{
    public class Email
    {
        #region Properties
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> SendTo { get; set; }
        public string AddTo
        {
            set
            {
                if (SendTo == null)
                    SendTo = new List<string>();
                SendTo.Add(value);
            }
        }
        public string CC { get; set; }
        #endregion

        #region constructor

        public Email() { }

        #endregion

        #region Static Methods

        public static bool SendEmail(SortMainObject sort, EmailTypeEnum emailType, bool includeAuthors = false)
        {
            string errorMsg = string.Empty;
            return SendEmail(sort, emailType, includeAuthors, ref errorMsg);
        }

        public static bool SendEmail(SortMainObject sort, EmailTypeEnum emailType, bool includeAuthors, ref string errorMsg)
        {
            try
            {
                var template = EmailTemplateObject.GetEmailTemplate(emailType.ToString());
                if (template != null)
                {
                    string subject = template.Header.Replace("{STI_Number}", sort.TitleStr).Replace("{PublishTitle}", sort.PublishTitle);
                    string body = template.Body
                        .Replace("{STI_Number}", sort.TitleStr)
                        .Replace("{PublishTitle}", sort.PublishTitle)
                        .Replace("{Authors}", string.Join(", ", sort.Authors.Select(n => $"{n.FirstName} {n.LastName}")))
                        .Replace("{DueDate}", $"{sort.DueDate:d}")
                        .Replace("{ReviewStatus}", sort.ReviewStatus)
                        .Replace("{ReviewProgress}", $"{sort.ReviewProgress:P}")
                        .Replace("{ViewUrl}", $"<a href=\"{Config.SortUrl(false).TrailingForwardSlash()}Artifact/{sort.SortMainId}\">STI Artifact</a>")
                        .Replace("{EditUrl}", $"<a href=\"{Config.SortUrl(false).TrailingForwardSlash()}Artifact/Edit/{sort.SortMainId}\">STI Artifact</a>");

                    if (emailType == EmailTypeEnum.FirstYearReminder || emailType == EmailTypeEnum.DelayedReminder)
                    {
                        return SendReleaseOfficerEmail(sort, subject, body, emailType, ref errorMsg);
                    }

                    return SendOwnerEmail(sort, subject, body, includeAuthors, emailType, ref errorMsg);
                }

                errorMsg = $"Unable to find the Template for Email Type: {emailType}";
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("Email::SendEmail", ex);
                errorMsg = $"Exception Caught while attempting to send an email: {ex.Message}";
            }

            return false;
        }
        #endregion

        #region Object Methods

        private static bool SendOwnerEmail(SortMainObject sort, string subject, string body, bool includeAuthors, EmailTypeEnum emailType, ref string errorMsg)
        {
            bool allMailSent = false;
            string ownerEmail = ConfigurationManager.AppSettings["OwnerEmail"].ToString();

            try
            {
                Email email = new Email();
                string addInfo = string.Empty;
                email.SendTo = new List<string>();
                email.Subject = subject;


                foreach (var contact in sort.Contacts)
                {
                    if (!string.IsNullOrWhiteSpace(contact.EmployeeId))
                    {
                        var user = UserObject.GetUser(contact.EmployeeId);
                        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                        {
                            if (!email.SendTo.Exists(n => n.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                email.SendTo.Add(user.Email);
                            }
                        }
                    }
                }

                if (email.SendTo.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(sort.OwnerEmail))
                    {
                        if (!email.SendTo.Exists(n => n.Equals(sort.OwnerEmail, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            email.SendTo.Add(sort.OwnerEmail);
                        }
                    }
                }

                if (includeAuthors)
                {
                    foreach (var author in sort.Authors.Where(n => n.AffiliationEnum == AffiliationEnum.INL))
                    {
                        if (!string.IsNullOrWhiteSpace(author.EmployeeId))
                        {
                            var user = UserObject.GetUser(author.EmployeeId);
                            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                            {
                                email.SendTo.Add(user.Email);
                            }
                        }
                    }
                }

                if (email.SendTo.Count == 0)
                {
                    email.SendTo.Add(ownerEmail);
                    addInfo = "<em><p><strong>This message has been sent to you in lieu of the intended target.  Their email was not found in Employee Data. Please review the email and determine appropriate course of action.</strong></p></em><hr />";
                }

                email.Body = addInfo + body;
                email.Body += "<hr /><p> You are being contacted because you are either the Owner or a Contact for the Artifact.</p>";
                email.Body += "<p> If you believe this has been in error, please contact the admin: " + ownerEmail + "</p>";
                email.Send(emailType);

                allMailSent = true;
            }
            catch(Exception ex)
            {
                ErrorLogObject.LogError("Email::SendOwnerEmail", ex);
                errorMsg = $"Exeption Caught on sending Email: {ex.Message}";
            }
            return allMailSent;
        }

        private static bool SendReleaseOfficerEmail(SortMainObject sort, string subject, string body, EmailTypeEnum emailType, ref string errorMsg)
        {
            bool allMailSent = false;
            string ownerEmail = ConfigurationManager.AppSettings["OwnerEmail"].ToString();

            try
            {
                Email email = new Email();
                string addInfo = string.Empty;
                email.SendTo = new List<string>();
                email.Subject = subject;

                foreach (var user in UserObject.GetUsers().Where(n => n.IsInAnyRole("ReleaseOfficial")))
                {
                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        email.SendTo.Add(user.Email);
                    }
                }
                
                if (email.SendTo.Count == 0)
                {
                    email.SendTo.Add(ownerEmail);
                    addInfo = "<em><p><strong>This message has been sent to you in lieu of the intended target.  Their email was not found in Employee Data. Please review the email and determine appropriate course of action.</strong></p></em><hr />";
                }

                email.Body = addInfo + body;
                email.Body += "<hr /><p> You are being contacted because you are a Release Official in the SORT system.</p>";
                email.Body += "<p> If you believe this has been in error, please contact the admin: " + ownerEmail + "</p>";
                email.Send(emailType);

                allMailSent = true;
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("Email::SendReleaseOfficerEmail", ex);
                errorMsg = $"Exeption Caught on sending Email: {ex.Message}";
            }
            return allMailSent;
        }

        private void Send(EmailTypeEnum emailType)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(Config.FromAddress);
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["EmailServer"]);
            if (Config.ApplicationMode == ApplicationMode.Production)// || (Config.ApplicationMode == ApplicationMode.Acceptance && emailType == EmailTypeEnum.Initial))
            {
                // We are in prodution, send the email to the person(s) we should.
                foreach (string to in SendTo)
                {
                    message.To.Clear();
                    message.CC.Clear();
                    message.Bcc.Clear();
                    message.To.Add(new MailAddress(to));
                    if (!String.IsNullOrEmpty(CC))
                        message.CC.Add(new MailAddress(CC));
                    smtp.Send(message);
                }
            }
            else if (Config.ApplicationMode != ApplicationMode.CyberScan)
            {
                // As long as we are not in production or CyberScan, send email to user
                // or if user does not have email, send to developer or acceptance person.
                message.Subject = "**TESTING** " + message.Subject;
                message.Body = "<em>Original Email Recipient(s): " + string.Join(", ", SendTo) + "</em><hr />" + Body;
                message.To.Clear();
                message.CC.Clear();
                message.Bcc.Clear();
                try
                {
                    if (!string.IsNullOrWhiteSpace(UserObject.RealCurrentUser.Email))
                    {
                        message.To.Add(UserObject.RealCurrentUser.Email);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogObject.LogError("Email::Send", ex);
                }

                if (message.To.Count == 0)
                {
                    if (Config.ApplicationMode == ApplicationMode.Acceptance)
                    {
                        message.To.Add(Config.OwnerEmail);
                    }
                    else
                    {
                        message.To.Add(Config.DeveloperEmail);
                    }
                }
                smtp.Send(message);
            }
        }

        #endregion
    }
}
