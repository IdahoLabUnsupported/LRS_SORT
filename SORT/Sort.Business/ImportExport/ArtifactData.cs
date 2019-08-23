using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sort.Business
{
    [DataContract(Namespace = "Artifact")]
    public class ArtifactData
    {
        #region Properties

        [DataMember]public int LrsId { get; set; }
        [DataMember]public string OwnerEmployeeId { get; set; }
        [DataMember] public string OwnerName { get; set; }
        [DataMember] public string OwnerEmail { get; set; }
        [DataMember]public string ReviewStatus { get; set; }
        [DataMember]public int ReviewProgress { get; set; }
        [DataMember]public string DocumentType { get; set; }
        [DataMember]public string Title { get; set; }
        [DataMember]public string StiNumber { get; set; }
        [DataMember]public int Revision { get; set; }
        [DataMember]public DateTime? CreateDate { get; set; }
        [DataMember]public DateTime? ModifiedDate { get; set; }
        [DataMember]public DateTime? ApprovedDate { get; set; }
        [DataMember]public string Abstract { get; set; }
        [DataMember]public string ConferenceName { get; set; }
        [DataMember]public string ConferenceSponsor { get; set; }
        [DataMember]public string ConferenceLocation { get; set; }
        [DataMember]public DateTime? ConferenceBeginDate { get; set; }
        [DataMember]public DateTime? ConferenceEndDate { get; set; }
        [DataMember]public string JournalName { get; set; }
        [DataMember] public string RelatedSti { get; set; }

        #endregion

        #region Lists

        [DataMember, XmlArray("Contacts")] public List<ContactData> Contacts { get; set; } = new List<ContactData>();
        [DataMember, XmlArray("Authors")] public List<AuthorData> Authors { get; set; } = new List<AuthorData>();
        [DataMember, XmlArray("Fundings")] public List<FundingData> Fundings { get; set; } = new List<FundingData>();
        [DataMember, XmlArray("Subjects")] public List<string> Subjects { get; set; } = new List<string>();
        [DataMember, XmlArray("Keywords")] public List<string> Keywords { get; set; } = new List<string>();
        [DataMember, XmlArray("Reviewers")] public List<ReviewerData> Reviewers { get; set; } = new List<ReviewerData>();
        [DataMember, XmlArray("CoreCapabilities")] public List<string> CoreCapabilities { get; set; } = new List<string>();
        #endregion

        #region Constructor

        public ArtifactData()
        {
        }

        #endregion

        #region Functions

        public int? Import()
        {
            int? sortId = null;

            var sort = new SortMainObject();
            sort.StatusEnum = StatusEnum.Imported;
            sort.SharePointId = LrsId;
            sort.Title = $"{StiNumber}-Revision-{Revision}";
            sort.StiNumber = StiNumber;
            sort.Revision = Revision;

            sort.PublishTitle = Title;
            sort.Abstract = Abstract;
            sort.JournalName = JournalName;
            sort.ConferenceName = ConferenceName;
            sort.ConferenceSponsor = ConferenceSponsor;
            sort.ConferenceLocation = ConferenceLocation;
            sort.ConferenceBeginDate = ConferenceBeginDate;
            sort.ConferenceEndDate = ConferenceEndDate;

            sort.OwnerName = OwnerName ?? EmployeeCache.GetName(OwnerEmployeeId);
            sort.OwnerEmployeeId = OwnerEmployeeId;
            sort.CreateDate = CreateDate;
            sort.ModifiedDate = ModifiedDate;
            sort.ApprovedDate = ApprovedDate;
            sort.RelatedSti = RelatedSti;
            if (sort.ApprovedDate.HasValue && sort.ApprovedDate.Value > new DateTime(2000, 1, 1))
            {
                sort.DueDate = sort.ApprovedDate.Value.AddMonths(1);
            }

            sort.ReviewStatus = ReviewStatus;
            sort.ReviewProgress = ReviewProgress;
            sort.AccessLimitationEnum = AccessLimitationEnum.Unlimited; //Default to Unlimited
            sort.Country = "US";
            sort.Language = "English";
            sort.IsFromLrs = true;
            if (!string.IsNullOrWhiteSpace(sort.OwnerEmployeeId) || !string.IsNullOrWhiteSpace(sort.OwnerName))
            {
                sort.Save();
                sortId = sort.SortMainId;
                ProcessLogObject.Add(sort.SortMainId, "Added Entry");
                if (Contacts != null)
                {
                    foreach (var contact in Contacts)
                    {
                        var cont = new ContactObject();
                        cont.SortMainId = sort.SortMainId.Value;
                        cont.FirstName = contact.FirstName ?? EmployeeCache.GetEmployee(contact.EmployeeId)?.FirstName;
                        cont.MiddleName = contact.MiddleName ?? EmployeeCache.GetEmployee(contact.EmployeeId)?.MiddleName;
                        cont.LastName = contact.LastName ?? EmployeeCache.GetEmployee(contact.EmployeeId)?.LastName;
                        cont.ContactTypeEnum = ContactTypeEnum.POC;
                        cont.Phone = contact.Phone ?? EmployeeCache.GetEmployee(contact.EmployeeId)?.PhoneNumber;
                        cont.EmployeeId = contact.EmployeeId;
                        cont.WorkOrg = contact.WorkOrg;
                        //cont.OrcidId = contact.OrcidId;
                        cont.Save();
                        ProcessLogObject.Add(sort.SortMainId, "Added Contact");
                    }
                }

                if (Authors != null)
                {
                    foreach(var author in Authors)
                    {
                        var auth = new AuthorObject();
                        auth.SharePointId = author.AuthorId;
                        auth.SortMainId = sort.SortMainId.Value;
                        auth.FirstName = author.FirstName;
                        auth.MiddleName = author.MiddleName;
                        auth.LastName = author.LastName;
                        auth.Affiliation = author.Affiliation;
                        switch (author.AffiliationType)
                        {
                            case "INL":
                                auth.AffiliationEnum = AffiliationEnum.INL;
                                auth.Affiliation = "Idaho National Laboratory";
                                break;
                            case "University":
                                auth.AffiliationEnum = AffiliationEnum.University;
                                break;
                            default:
                                auth.AffiliationEnum = AffiliationEnum.Other;
                                break;
                        }

                        auth.Email = author.Email;
                        auth.OrcidId = author.OrcidId;
                        auth.IsPrimary = author.IsPrimary;
                        auth.EmployeeId = author.EmployeeId;
                        auth.WorkOrg = author.WorkOrg;
                        auth.CountryId = MemoryCache.GetCountryByCode(author.CountryCode)?.CountryId;
                        auth.StateId = MemoryCache.GetStateByShortName(author.StateCode)?.StateId;
                        auth.Save();
                        ProcessLogObject.Add(sort.SortMainId, "Added Author");
                    }
                }

                if (Fundings != null)
                {
                    foreach (var funding in Fundings)
                    {
                        var fund = new FundingObject();
                        fund.SortMainId = sort.SortMainId.Value;
                        fund.Year = funding.Year;
                        fund.FundingTypeId = funding.FundingTypeId;
                        fund.Org = funding.Org;
                        fund.ContractNumber = funding.ContractNumber;
                        fund.Percent = funding.Percent;
                        fund.DoeFundingCategoryId = funding.DoeFundingCategoryId;
                        fund.GrantNumber = funding.GrantNumber;
                        fund.TrackingNumber = funding.TrackingNumber;
                        fund.SppCategoryId = funding.SppCategoryId;
                        fund.SppApproved = funding.SppApproved;
                        fund.FederalAgencyId = funding.FederalAgencyId;
                        fund.ApproveNoReason = funding.ApproveNoReason;
                        fund.OtherDescription = funding.OtherDescription;
                        fund.CountryId = funding.CountryId;
                        fund.AdditionalInfo = funding.AdditionalInfo;
                        fund.ProjectArea = funding.ProjectArea;
                        fund.ProjectNumber = funding.ProjectNumber;
                        fund.PrincipalInvEmployeeId = funding.PrincipalInvEmployeeId;
                        fund.MilestoneTrackingNumber = funding.MilestoneTrackingNumber;

                        try
                        {
                            fund.Save();
                            ProcessLogObject.Add(sort.SortMainId, "Added Funding Type");
                        }
                        catch (Exception ex)
                        {
                            ErrorLogObject.LogError("Console:ImportFromSharepoint:FundingSave", ex);
                        }
                    }
                }

                if (Subjects != null)
                {
                    foreach (var subject in Subjects)
                    {
                        SortMetaDataObject.AddNew(MetaDataTypeEnum.SubjectCategories, sort.SortMainId.Value, subject);
                        ProcessLogObject.Add(sort.SortMainId, "Added subject Metadata");
                    }
                }

                if (Keywords != null)
                {
                    foreach (var keyword in Keywords)
                    {
                        SortMetaDataObject.AddNew(MetaDataTypeEnum.Keywords, sort.SortMainId.Value, keyword);
                        ProcessLogObject.Add(sort.SortMainId, "Added Keyword Metadata");
                    }
                }

                if (CoreCapabilities != null)
                {
                    foreach (var core in CoreCapabilities)
                    {
                        SortMetaDataObject.AddNew(MetaDataTypeEnum.CoreCapabilities, sort.SortMainId.Value, core);
                        ProcessLogObject.Add(sort.SortMainId, "Added Core Capability Metadata");
                    }
                }

                if (Reviewers != null)
                {
                    foreach (var reviewer in Reviewers)
                    {
                        try
                        {
                            ReviewObject review = new ReviewObject();
                            review.SortMainId = sort.SortMainId.Value;
                            switch (reviewer.ReviewerType)
                            {
                                case "Manager":
                                    review.ReviewerTypeEnum = ReviewerTypeEnum.Manager;
                                    break;
                                case "Classification":
                                    review.ReviewerTypeEnum = ReviewerTypeEnum.Classification;
                                    break;
                                case "ExportControl":
                                    review.ReviewerTypeEnum = ReviewerTypeEnum.ExportControl;
                                    break;
                                case "TechDeployment":
                                    review.ReviewerTypeEnum = ReviewerTypeEnum.TechDeployment;
                                    break;
                                case "PeerTechnical":
                                    review.ReviewerTypeEnum = ReviewerTypeEnum.PeerTechnical;
                                    break;
                            }

                            review.Reviewer = reviewer.ReviewerName ?? EmployeeCache.GetName(reviewer.ReviewerEmployeeId) ?? "Unknown";
                            reviewer.ReviewerEmployeeId = reviewer.ReviewerEmployeeId;
                            review.ReviewDate = reviewer.ReviewDate;
                            review.Approval = reviewer.Status;
                            review.Save();
                        }
                        catch (Exception ex)
                        {
                            ErrorLogObject.LogError("Console:ProcessReviewers", ex);
                        }

                    }
                }

                Email.SendEmail(sort, EmailTypeEnum.Initial, true);
                ProcessLogObject.Add(sort.SortMainId, "Email Sent");
                ProcessLogObject.Add(sort.SortMainId, "Success");
            }

            return sortId;
        }

        #endregion
    }

    [DataContract(Namespace = "Contact")]
    public class ContactData
    {
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string MiddleName { get; set; }
        [DataMember] public string LastName { get; set; }
        [DataMember] public string Phone { get; set; }
        [DataMember] public string Location { get; set; }
        [DataMember] public string EmployeeId { get; set; }
        [DataMember] public string WorkOrg { get; set; }
        [DataMember] public string OrcidId { get; set; }
    }

    [DataContract(Namespace = "Author")]
    public class AuthorData
    {
        [DataMember] public int AuthorId { get; set; }
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string MiddleName { get; set; }
        [DataMember] public string LastName { get; set; }
        [DataMember] public string Affiliation { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public string OrcidId { get; set; }
        [DataMember] public bool IsPrimary { get; set; }
        [DataMember] public string EmployeeId { get; set; }
        [DataMember] public string AffiliationType { get; set; }
        [DataMember] public string WorkOrg { get; set; }
        [DataMember] public string CountryCode { get; set; }
        [DataMember] public string StateCode { get; set; }

        public string Name
        {
            set
            {
                FirstName = value.FirstName();
                MiddleName = value.MiddelName();
                LastName = value.LastName();
            }
            get => $"{FirstName} {MiddleName} {LastName}";
        }
    }

    [DataContract(Namespace = "Funding")]
    public class FundingData
    {
        [DataMember] public string Year { get; set; }
        [DataMember] public int FundingTypeId { get; set; }
        [DataMember] public string Org { get; set; }
        [DataMember] public string ContractNumber { get; set; }
        [DataMember] public string Percent { get; set; }
        [DataMember] public int? DoeFundingCategoryId { get; set; }
        [DataMember] public string GrantNumber { get; set; }
        [DataMember] public string TrackingNumber { get; set; }
        [DataMember] public int? SppCategoryId { get; set; }
        [DataMember] public bool? SppApproved { get; set; }
        [DataMember] public int? FederalAgencyId { get; set; }
        [DataMember] public string ApproveNoReason { get; set; }
        [DataMember] public string OtherDescription { get; set; }
        [DataMember] public int? CountryId { get; set; }
        [DataMember] public string AdditionalInfo { get; set; }
        [DataMember] public string ProjectArea { get; set; }
        [DataMember] public string ProjectNumber { get; set; }
        [DataMember] public string PrincipalInvEmployeeId { get; set; }
        [DataMember] public string MilestoneTrackingNumber { get; set; }
    }

    [DataContract(Namespace = "Reviewer")]
    public class ReviewerData
    {
        [DataMember] public string ReviewerEmployeeId { get; set; }
        [DataMember] public string ReviewerName { get; set; }
        [DataMember] public string ReviewerType { get; set; }
        [DataMember] public DateTime? ReviewDate { get; set; }
        [DataMember] public string Status { get; set; }
    }
}
