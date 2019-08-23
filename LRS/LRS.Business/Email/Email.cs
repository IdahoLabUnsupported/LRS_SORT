using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;

namespace LRS.Business
{
    public class Email
    {
        #region Properties
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> SendTo { get; set; }
        #endregion

        #region constructor

        public Email() { }

        #endregion

        #region Static Methods

        public static bool SendEmail(MainObject main, ReviewObject review, EmailTypeEnum emailType, string comment)
        {
            string errorMsg = string.Empty;
            return SendEmail(main, review, emailType, comment, ref errorMsg);
        }

        public static bool SendEmail(MainObject main, ReviewObject review, EmailTypeEnum emailType, string comment, ref string errorMsg)
        {
            try
            {
                var template = EmailTemplateObject.GetEmailTemplate(emailType);
                if (template != null)
                {
                    string subject = template.Header.Replace("{STI_Number}", main.StiNumber ?? string.Empty).Replace("{Title}", main.Title ?? string.Empty);
                    string body = ConstructBody(main, review, comment, template);
                    switch (emailType)
                    {
                        case EmailTypeEnum.ReleaseOfficerComplete:
                            return SendReleaseOfficerEmail(subject, body, ref errorMsg);
                        case EmailTypeEnum.ClassReviewer:
                        case EmailTypeEnum.ExportReviewer:
                        case EmailTypeEnum.ManagerReviewer:
                        case EmailTypeEnum.PeerReviewer:
                        case EmailTypeEnum.TechReviewer:
                        case EmailTypeEnum.ReviewReminder:
                            return review != null ? SendReviewerEmail(review, subject, body, ref errorMsg) : SendReviewersEmail(main, subject, body, emailType, ref errorMsg);
                        default:
                            return SendOwnerEmail(main, subject, body, template.IncludeAuthors, template.IncludeContacts, ref errorMsg);
                    }
                }

                errorMsg = $"Unable to find the Template for Email Type: {emailType}";
            }
            catch (Exception ex)
            {
                errorMsg = $"Exception Caught while attempting to send an email: {ex.Message}";
            }

            return false;
        }
        
        #endregion

        #region Object Methods
        
        private static string ConstructBody(MainObject main, ReviewObject review, string comment, EmailTemplateObject template)
        {
            if (template != null)
            {
                return template.Body
                    .Replace("{STI_Number}", main.StiNumber ?? string.Empty)
                    .Replace("{Title}", main.Title ?? string.Empty)
                    .Replace("{Authors}", string.Join(", ", main.Authors.Select(n => n.Name)) ?? string.Empty)
                    //.Replace("{DueDate}", $"{main.DueDate:d}")
                    .Replace("{ReviewStatus}", main.ReviewStatus ?? string.Empty)
                    .Replace("{ReviewProgress}", $"{main.ReviewProgress:P}" ?? string.Empty)
                    .Replace("{ReviewerName}", review?.ReviewerName ?? string.Empty)
                    .Replace("{Comment}", comment ?? string.Empty)
                    .Replace("{ReviewType}", review?.ReviewerTypeDisplayName ?? string.Empty)
                    .Replace("{Url}", $"<a href=\"{Config.SiteUrl.TrailingSlash()}Artifact/Index/{main.MainId}\">LRS Artifact</a>")
                    .Replace("{OUO-FOIA}", main.OuoEmailText ?? string.Empty)
                    .Replace("{FundingOrg}", string.Join(", ", main?.Funding?.Where(m => !string.IsNullOrWhiteSpace(m.FundingOrgName)).Select(n => n.FundingOrgName)) ?? string.Empty);
            }

            return string.Empty;
        }

        private static bool SendOwnerEmail(MainObject main, string subject, string body, bool includeAuthors, bool includeContacts, ref string errorMsg)
        {
            bool allMailSent = false;
            List<string> recipientNames = new List<string>();

            try
            {
                Email email = new Email();
                string addInfo = string.Empty;
                email.SendTo = new List<string>();
                email.Subject = subject;

                foreach (var contact in main.Contacts)
                {
                    if (!string.IsNullOrWhiteSpace(contact.EmployeeId))
                    {
                        var user = UserObject.GetUser(contact.EmployeeId);
                        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                        {
                            if (!email.SendTo.Exists(n => n.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                email.SendTo.Add(user.Email);
                                recipientNames.Add(user.FullName);
                            }
                        }
                    }
                }

                if (email.SendTo.Count == 0 || includeContacts)
                {
                    if (!string.IsNullOrWhiteSpace(main.OwnerEmail))
                    {
                        if (!email.SendTo.Exists(n => n.Equals(main.OwnerEmail, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            email.SendTo.Add(main.OwnerEmail);
                            recipientNames.Add(main.OwnerName);
                        }
                    }
                }

                if (includeAuthors)
                {
                    foreach (var author in main.Authors.Where(n => n.AffiliationEnum == AuthorAffilitionEnum.INL))
                    {
                        if (!string.IsNullOrWhiteSpace(author.EmployeeId))
                        {
                            var user = UserObject.GetUser(author.EmployeeId);
                            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                            {
                                if (!email.SendTo.Exists(n => n.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    email.SendTo.Add(user.Email);
                                    recipientNames.Add(user.FullName);
                                }
                            }
                        }
                    }
                }
                
                if (email.SendTo.Count == 0)
                {
                    email.SendTo.Add(Config.OwnerEmail);
                    addInfo = "<em><p><strong>This message has been sent to you in lieu of the intended target.  Their email was not found in Employee Repo. Please review the email and determine appropriate course of action.</strong></p></em><hr />";
                }

                email.Body = addInfo + body.Replace("{RecipientsName}", string.Join(" : ", recipientNames));
                AddContactInfo(ref email);
                email.Send();

                allMailSent = true;
            }
            catch (Exception ex)
            {
                errorMsg = $"Exeption Caught on sending Email: {ex.Message}";
            }
            return allMailSent;
        }

        private static bool SendReviewerEmail(ReviewObject review, string subject, string body, ref string errorMsg)
        {
            bool allMailSent = false;
            List<string> recipientNames = new List<string>();

            try
            {
                Email email = new Email();
                string addInfo = string.Empty;
                email.SendTo = new List<string>();
                email.Subject = subject;
                if (review.SystemReviewer)
                {
                    if ((MemoryCache.GetGenericReviewData(review.ReviewerTypeEnum)?.Count ?? 0) > 0)
                    {
                        MemoryCache.GetGenericReviewData(review.ReviewerTypeEnum).ForEach(n => email.SendTo.Add(n.GenericEmail));

                        switch (review.ReviewerTypeEnum)
                        {
                            case ReviewerTypeEnum.Classification:
                                recipientNames.Add("Classification Reviewer");
                                break;
                            case ReviewerTypeEnum.ExportControl:
                                recipientNames.Add("Export Compliance Reviewer");
                                break;
                            case ReviewerTypeEnum.TechDeployment:
                                recipientNames.Add("Technical Deployment Reviewer");
                                break;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(review.Email))
                    {
                        email.SendTo.Add(review.Email);
                        recipientNames.Add(review.ReviewerName);
                    }
                }

                if (email.SendTo.Count == 0)
                {
                    return false;
                }

                review.LastEmailDate = DateTime.Now;
                review.Save();

                email.Body = addInfo + body.Replace("{RecipientsName}", string.Join(" : ", recipientNames));
                AddContactInfo(ref email);
                email.Send();

                allMailSent = true;
            }
            catch (Exception ex)
            {
                errorMsg = $"Exeption Caught on sending Email: {ex.Message}";
            }
            return allMailSent;
        }

        private static bool SendReviewersEmail(MainObject main, string subject, string body, EmailTypeEnum emailType, ref string errorMsg)
        {
            bool allMailSent = false;
            List<string> recipientNames = new List<string>();

            try
            {
                Email email = new Email();
                string addInfo = string.Empty;
                email.SendTo = new List<string>();
                email.Subject = subject;
                switch (emailType)
                {
                    case EmailTypeEnum.ClassReviewer:
                        foreach (var reviewer in MemoryCache.GetGenericReviewData(ReviewerTypeEnum.Classification) ?? new List<GenericReviewDataObject>())
                        {
                            if (reviewer != null && !string.IsNullOrWhiteSpace(reviewer.GenericEmail))
                            {
                                email.SendTo.Add(reviewer.GenericEmail);
                            }
                        }

                        foreach (var r in main.Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification))
                        {
                            r.LastEmailDate = DateTime.Now;
                            r.Save();
                        }

                        recipientNames.Add("Classification Reviewer");
                        break;
                    case EmailTypeEnum.ExportReviewer:
                        foreach (var reviewer in MemoryCache.GetGenericReviewData(ReviewerTypeEnum.ExportControl) ?? new List<GenericReviewDataObject>())
                        {
                            if (reviewer != null && !string.IsNullOrWhiteSpace(reviewer.GenericEmail))
                            {
                                email.SendTo.Add(reviewer.GenericEmail);
                            }
                        }

                        foreach (var r in main.Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification))
                        {
                            r.LastEmailDate = DateTime.Now;
                            r.Save();
                        }

                        recipientNames.Add("Export Compliance Reviewer");
                        break;
                    case EmailTypeEnum.TechReviewer:
                        foreach (var reviewer in MemoryCache.GetGenericReviewData(ReviewerTypeEnum.TechDeployment) ?? new List<GenericReviewDataObject>())
                        {
                            if (reviewer != null && !string.IsNullOrWhiteSpace(reviewer.GenericEmail))
                            {
                                email.SendTo.Add(reviewer.GenericEmail);
                            }
                        }

                        foreach (var r in main.Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment))
                        {
                            r.LastEmailDate = DateTime.Now;
                            r.Save();
                        }

                        recipientNames.Add("Technical Deployment Reviewer");
                        break;
                    case EmailTypeEnum.ManagerReviewer:
                        foreach (var reviewer in main.Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Manager))
                        {
                            if (reviewer != null && !string.IsNullOrWhiteSpace(reviewer.Email))
                            {
                                email.SendTo.Add(reviewer.Email);
                                recipientNames.Add(reviewer.ReviewerName);
                                reviewer.LastEmailDate = DateTime.Now;
                                reviewer.Save();
                            }
                        }

                        break;
                    case EmailTypeEnum.PeerReviewer:
                        foreach (var reviewer in main.Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical))
                        {
                            if (reviewer != null && !string.IsNullOrWhiteSpace(reviewer.Email))
                            {
                                email.SendTo.Add(reviewer.Email);
                                recipientNames.Add(reviewer.ReviewerName);
                                reviewer.LastEmailDate = DateTime.Now;
                                reviewer.Save();
                            }
                        }

                        break;
                    default:
                        return false;
                }

                if (email.SendTo.Count == 0)
                {
                    return false;
                }

                email.Body = addInfo + body.Replace("{RecipientsName}", string.Join(" : ", recipientNames));
                AddContactInfo(ref email);
                email.Send();

                allMailSent = true;
            }
            catch (Exception ex)
            {
                errorMsg = $"Exeption Caught on sending Email: {ex.Message}";
            }
            return allMailSent;
        }

        private static bool SendReleaseOfficerEmail(string subject, string body, ref string errorMsg)
        {
            bool allMailSent = false;
            List<string> recipientNames = new List<string>();

            try
            {
                Email email = new Email();
                string addInfo = string.Empty;
                email.SendTo = new List<string>();
                email.Subject = subject;

                foreach (var user in UserObject.GetUsers().Where(n => n.IsInAnyRole("ReleaseOfficial")) ?? new List<UserObject>())
                {
                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        email.SendTo.Add(user.Email);
                        recipientNames.Add(user.FullName);
                    }
                }

                if (email.SendTo.Count == 0)
                {
                    email.SendTo.Add(Config.OwnerEmail);
                    addInfo = "<em><p><strong>This message has been sent to you in lieu of the intended target.  Their email was not found in Employee Repo. Please review the email and determine appropriate course of action.</strong></p></em><hr />";
                }

                email.Body = addInfo + body.Replace("{RecipientsName}", string.Join(" : ", recipientNames));
                AddContactInfo(ref email);
                email.Send();

                allMailSent = true;
            }
            catch (Exception ex)
            {
                errorMsg = $"Exeption Caught on sending Email: {ex.Message}";
            }
            return allMailSent;
        }

        private static void AddContactInfo(ref Email email)
        {
            email.Body += "<hr /><p> You are being contacted because you are either the Owner or an Author for the Artifact.</p>";
            email.Body += "<p> If you have received this notification in error, please contact the Admin: " + Config.OwnerEmail + "</p>";
        }

        private void Send()
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(Config.FromAddress);
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = true;
            message.CC.Clear();
            message.Bcc.Clear();
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["EmailServer"]);
            if (Config.ApplicationMode == ApplicationMode.Production)
            {
                // We are in prodution, send the email to the person(s) we should.
                foreach (string to in SendTo)
                {
                    message.To.Clear();
                    message.To.Add(new MailAddress(to));
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
                try
                {
                    if (!string.IsNullOrWhiteSpace(UserObject.RealCurrentUser.Email))
                    {
                        message.To.Add(UserObject.RealCurrentUser.Email);
                    }
                }
                catch {}

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
