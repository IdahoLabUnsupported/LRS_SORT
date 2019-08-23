using System.ComponentModel.DataAnnotations;

namespace Sort.Business
{
	public enum UserRole
	{
        [Display(Name = "Admin")] Admin,
        [Display(Name = "User")] User,
        [Display(Name = "Releasing Official")] ReleaseOfficial,
	    [Display(Name = "Org Manager")] OrgManager,
	    [Display(Name = "Read All Permissions")] ReadAll
    }

    public enum ProductTypeEnum
    {
        //[Display(Name = "Unknown", ShortName = "")]
        //Unknown,
        [Display(Name = "Technicial Report or Workshop Report", ShortName = "TR")]
        TechReport,
        [Display(Name = "Conference/Event Paper, Proceedings, or Presentation", ShortName = "CO")]
        ConfEvent,
        [Display(Name = "Factsheet", ShortName = "FS")]
        FactSheet,
        [Display(Name = "Journal Article", ShortName = "JA")]
        JournalArticle,
        [Display(Name = "Thesis or Dissertation", ShortName = "TD")] 
        ThesisDissertation,
        [Display(Name = "Program Document", ShortName = "PD")]
        ProgramDocument,
        [Display(Name = "Book/Monograph", ShortName = "B")]
        BookMonograph,
        [Display(Name = "Accomplishment Report", ShortName = "AR")]
        AccomplishmentReport,
        [Display(Name = "Patent", ShortName = "P")]
        Patent
    }

    public enum JournalTypeEnum
    {
        //[Display(Name = "Unknown", ShortName = "")]
        //Unknown,
        [Display(Name = "Announcement Citation Only", ShortName = "AC")]
        AnnouncementCitation,
        [Display(Name = "Author's Early Manuscript", ShortName = "FT")]
        EarlyManuscript,
        [Display(Name = "Accepted Manuscript - Author's Final Version", ShortName = "AM")]
        AcceptedManuscript
    }

    public enum AccessLimitationEnum
    {
        [Display(Name = "Unlimited Announcement", ShortName = "unl", Description = "STI Product should have unlimited and unrestricted distribution to the U.S. and non-U.S. public.")]
        Unlimited,
        [Display(Name = "OpenNet", ShortName = "opn", Description = "See OpenNet Policy and guidance at https://www.osti.gov/opennet/")]
        OpenNet,
        [Display(Name = "Copyright Material with Restrictions", ShortName = "cpy", Description = "Copyrighted material may be subject to restrictions on reproducation, distribution, the preparation of derivative workds, including translations; public display of the material; and public performance of the material.")]
        CopyrightRestrictions,
        [Display(Name = "Official Use Only", ShortName = "ouo")]
        OfficialUseOnly,
        [Display(Name = "Export Controlled Information", ShortName = "ouoeci", Description = "Information containing technical data as  defined in and controlled by U.S. export control statutes (e.g., unser ITAR/EAR)(FOIA Exemption 3)")]
        ExportControlledInfo,
        [Display(Name = "Security Sensitive Information", ShortName = "ouossi")]
        SecuritySensitiveInfo,
        [Display(Name = "Protected Data - CRADA or other", ShortName = "ouoprot", Description = "Information produced in the performance of a CRADA that is marked as being Protected CRADA Information by a party to the agreement and that would have been proprietary information had it been obtained from a non-Federal entity. It may be protected for a period up to 5 years from the date it was produced, except as expressly provided for in the CRADA.")]
        ProtectedData,
        [Display(Name = "Patent Pending", ShortName = "ouopat", Description = "Federal agencies are authoized to withhold from public disclosure any invention in which the Government owns or may own a right, title, or interest, for any reasonable length of time so that a patent application can be filed.")]
        PatentPending,
        [Display(Name = "Limited Rights Data (Proprietary/Trade Secret)", ShortName = "ouolrd", Description = "Information that enbodies trad secrets or is commercial or financial and confidential or privileged, to the extent that such data pertain to items compenents, or processes developed at private (not government) expense, including minor modifications")]
        LimitedRightsData,
        [Display(Name = "Applied Technology", ShortName = "ouoat", Description = "Unclassified category established by NE to preserve the forign trade value of certain NE-funded progress and typical reports containing engineering, development, design, construction, and operation information pertaining to perticular programs.")]
        AppliedTech,
        [Display(Name = "Program-Determained Official Use Only", ShortName = "ouopdouo", Description = "This type of OUO is based on approved guidance as reflected in DOE Order 471.3, Identifying and Protecting Official Use Only Information.")]
        ProgramDeterminedOfficialUseOnly,
        [Display(Name = "Naval Nuclear Propulsion Information", ShortName = "nnpi", Description = "Unclassified Information related to any aspect of prepulsion plants of naval nuclear-powered ships and prototypes, including the associated shipboard and shorebased nuclear support facilities. See Chief of Naval Operations Instruction N9210.3, Safeguarding of Naval Nuclear Propulsion Information.")]
        NavalNuclearPropInfo
    }

    public enum DeclassStatusEnum
    {
        [Display(Name = "Unknown", ShortName = "U")]
        Unknown,
        [Display(Name = "Declassified", ShortName = "D")]
        Declassified,
        [Display(Name = "Sanitized", ShortName = "S")]
        Sanitized,
        [Display(Name = "Never Classified", ShortName = "N")]
        NeverClassified
    }

    public enum MetaDataTypeEnum
    {
        Unknown,
        SponsoringOrgs,
        RdProjectId,
        WorkProposalNum,
        WorkAuthNum,
        OtherNum,
        SubjectCategories,
        Contributors,
        Keywords,
        CoreCapabilities
    }

    public enum StatusEnum
    {
        [Display(Name = "Need Data", ShortName = "")]
        Imported,
        [Display(Name = "Data Complete", ShortName = "")]
        Complete,
        [Display(Name = "At OSTI", ShortName = "")]
        Published,
        [Display(Name = "Cancelled", ShortName = "")]
        Cancelled
    }

    public enum ContactTypeEnum
    {
        [Display(Name = "POC", ShortName = "")]
        POC,
        [Display(Name = "Editor", ShortName = "")]
        Editor,
        [Display(Name = "Recorder", ShortName = "")]
        Recorder
    }

    public enum AffiliationEnum
    {
        [Display(Name = "Other", ShortName = "")]
        Other,
        [Display(Name = "Idaho National Laboratory", ShortName = "")]
        INL,
        [Display(Name = "University", ShortName = "")]
        University
    }

    public enum ReviewerTypeEnum
    {
        [Display(Name = "Manager")]
        Manager,
        [Display(Name = "Classification")]
        Classification,
        [Display(Name = "Export Control")]
        ExportControl,
        [Display(Name = "Tech Deployment")]
        TechDeployment,
        [Display(Name = "Peer / Technical")]
        PeerTechnical
    }

    public enum ViewModeOptionEnum
    {
        [Display(Name = "All")]
        All,
        [Display(Name = "In Process / Needs Data")]
        InProcess,
        [Display(Name = "Past Due / Needs Data")]
        PastDue,
        [Display(Name = "Completed / Needs Review Approved")]
        CompletedNeedsApproved,
        [Display(Name = "Completed / Needs Release")]
        CompletedNeedsPublished,
        [Display(Name = "At OSTI")]
        Published,
        [Display(Name = "Cancelled")]
        Cancelled
    }

    public enum AttachmentTypeEnum
    {
        [Display(Name = "Rough Draft")]
        RoughDraft,
        [Display(Name = "Final Document / Accepted Manuscript")]
        FinalDoc,
        [Display(Name = "Copyright Agreement")]
        CopyrightAgreement,
        [Display(Name = "Supporting Documents")]
        Support
    }

    public enum SearchDates
    {
        [Display(Name = "Date Artifact was created in Sharepoint")]
        CreateDate = 1,
        [Display(Name = "Date Artifact was sent to OSTI")]
        OstiDate = 2,
        [Display(Name = "Date Artifact was approved in Sharepoint")]
        ApprovedDate = 3,
        [Display(Name = "Date Articate was last modified in Sharepoint")]
        ModifiedDate = 4,
        [Display(Name = "Date Artifact First released to OSTI")]
        ReleasedDate = 5,
        [Display(Name = "Artifact Publication / Issue Date")]
        PublicationDate = 6
    }

    public enum EmailTypeEnum
    {
        Initial,
        Reminder,
        [Display(Name = "One Year Reminder")]
        FirstYearReminder,
        [Display(Name = "Delayed Release Reminder")]
        DelayedReminder
    }

    public enum OrgOptionEnum
    {
        MyArtifacts = 1,
        OrgArtifacts = 2
    }
}
