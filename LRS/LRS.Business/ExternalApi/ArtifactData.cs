using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace LRS.Business
{
    [DataContract(Namespace = "Artifact")]
    public class ArtifactData
    {
        #region Properties

        [DataMember] public int LrsId { get; set; }
        [DataMember] public string OwnerEmployeeId { get; set; }
        [DataMember] public string OwnerName { get; set; }
        [DataMember] public string OwnerEmail { get; set; }
        [DataMember] public string ReviewStatus { get; set; }
        [DataMember] public int ReviewProgress { get; set; }
        [DataMember] public string DocumentType { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public string StiNumber { get; set; }
        [DataMember] public int Revision { get; set; }
        [DataMember] public DateTime? CreateDate { get; set; }
        [DataMember] public DateTime? ModifiedDate { get; set; }
        [DataMember] public DateTime? ApprovedDate { get; set; }
        [DataMember] public string Abstract { get; set; }
        [DataMember] public string ConferenceName { get; set; }
        [DataMember] public string ConferenceSponsor { get; set; }
        [DataMember] public string ConferenceLocation { get; set; }
        [DataMember] public DateTime? ConferenceBeginDate { get; set; }
        [DataMember] public DateTime? ConferenceEndDate { get; set; }
        [DataMember] public string JournalName { get; set; }
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

        public ArtifactData(){}

        public ArtifactData(MainObject main)
        {
            LrsId = main.MainId.Value;
            OwnerEmployeeId = main.OwnerEmployeeId;
            OwnerName = main.OwnerName;
            OwnerEmail = main.OwnerEmail;
            ReviewStatus = "Approved";//main.ReviewStatus;
            ReviewProgress = (int)main.ReviewPercent;
            DocumentType = main.DocumentType;
            Title = main.Title;
            StiNumber = main.StiNumber;
            Revision = main.Revision;
            CreateDate = main.CreateDate;
            ModifiedDate = main.ActivityDate;
            ApprovedDate = main.ApprovalDate;
            Abstract = main.Abstract;
            ConferenceName = main.ConferenceName;
            ConferenceSponsor = main.ConferenceSponsor;
            ConferenceLocation = main.ConferenceLocation;
            ConferenceBeginDate = main.ConferenceBeginDate;
            ConferenceEndDate = main.ConferenceEndDate;
            JournalName = main.JournalName;
            RelatedSti = main.RelatedSti;
            main.Contacts.ForEach(n => Contacts.Add(new ContactData(n)));
            main.Authors.ForEach(n => Authors.Add(new AuthorData(n)));
            main.Funding.ForEach(n => Fundings.Add(new FundingData(n)));
            main.Reviewers.ForEach(n => Reviewers.Add(new ReviewerData(n)));
            main.SubjectCategories.ForEach(n => Subjects.Add(n.Data));
            main.KeyWordList.ForEach(n => Keywords.Add(n.Data));
            main.CoreCapabilities.ForEach(n => CoreCapabilities.Add(n.Data));
        }

        #endregion

        #region Functions

        public static ArtifactData GenerateArtifact(MainObject main) => new ArtifactData(main);

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

        public ContactData() { }

        public ContactData(ContactObject co)
        {
            FirstName = co.Name.FirstName();
            MiddleName = co.Name.MiddelName();
            LastName = co.Name.LastName();
            Phone = co.Phone;
            Location = co.Location;
            EmployeeId = co.EmployeeId;
            WorkOrg = co.WorkOrg;
            OrcidId = co.OrcidId;
        }
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

        public AuthorData() { }

        public AuthorData(AuthorObject ao)
        {
            AuthorId = ao.AuthorId;
            FirstName = ao.Name.FirstName();
            MiddleName = ao.Name.MiddelName();
            LastName = ao.Name.LastName();
            Affiliation = ao.Affiliation;
            Email = ao.Email;
            OrcidId = ao.OrcidId;
            IsPrimary = ao.IsPrimary;
            EmployeeId = ao.EmployeeId;
            AffiliationType = ao.AffiliationType;
            WorkOrg = ao.WorkOrg;
            CountryCode = MemoryCache.GetCountry(ao.CountryId ?? 0)?.CountryCode;
            StateCode = MemoryCache.GetState(ao.StateId ?? 0)?.ShortName;
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

        public FundingData() { }

        public FundingData(FundingObject fo)
        {
            Year = fo.Year;
            FundingTypeId = fo.FundingTypeId;
            Org = fo.Org;
            ContractNumber = fo.ContractNumber;
            Percent = fo.Percent;
            DoeFundingCategoryId = fo.DoeFundingCategoryId;
            GrantNumber = fo.GrantNumber;
            TrackingNumber = fo.TrackingNumber;
            SppCategoryId = fo.SppCategoryId;
            SppApproved = fo.SppApproved;
            FederalAgencyId = fo.FederalAgencyId;
            ApproveNoReason = fo.ApproveNoReason;
            OtherDescription = fo.OtherDescription;
            CountryId = fo.CountryId;
            AdditionalInfo = fo.AdditionalInfo;
            ProjectArea = fo.ProjectArea;
            ProjectNumber = fo.ProjectNumber;
            PrincipalInvEmployeeId = fo.PrincipalInvEmployeeId;
            MilestoneTrackingNumber = fo.MilestoneTrackingNumber;
        }
    }

    [DataContract(Namespace = "Reviewer")]
    public class ReviewerData
    {
        [DataMember] public string ReviewerEmployeeId { get; set; }
        [DataMember] public string ReviewerName { get; set; }
        [DataMember] public string ReviewerType { get; set; }
        [DataMember] public DateTime? ReviewDate { get; set; }
        [DataMember] public string Status { get; set; }

        public ReviewerData() { }

        public ReviewerData(ReviewObject ro)
        {
            ReviewerEmployeeId = ro.ReviewerEmployeeId;
            ReviewerName = ro.ReviewerName;
            ReviewerType = ro.ReviewerType;
            ReviewDate = ro.ReviewDate;
            Status = ro.Status;
        }
    }
}
