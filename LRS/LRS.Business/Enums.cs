using System.ComponentModel.DataAnnotations;

namespace LRS.Business
{
	public enum UserRole
	{
	    [Display(Name = "Admin")] Admin,
	    [Display(Name = "User")] User,
	    [Display(Name = "Releasing Official")] ReleaseOfficial,
	    [Display(Name = "DOE/ID User")] DoeUser,
        [Display(Name = "Org Manager")] OrgManager,
	    [Display(Name = "Generic Release User")] GenericReleaseUser,
        [Display(Name = "Historical Worker")] HistWorker
    }

    public enum AttachmentTypeEnum
    {
        [Display(Name = "Rough Draft")] RoughDraft,
        [Display(Name = "Final Document / Accepted Manuscript")] FinalDoc,
        [Display(Name = "Copyright Agreement")] CopyrightAgreement,
        [Display(Name = "Supporting Documents")] Support
    }

    public enum OrgOptionEnum
    {
        MyArtifacts = 1,
        OrgArtifacts = 2
    }

    public enum MetaDataTypeEnum
    {
        Unknown,
        Keywords,
        SponsoringOrgs,
        SubjectCategories,
        CoreCapabilities
    }

    public enum ReviewTypeEnum
    {
        New,
        [Display(Name = "Follow On")] FollowOn,
        Continue
    }

    public enum DocumentTypeEnum
    {
        [Display(Name = "Internal Report", ShortName = "INT", Description = "A technical report that is NOT intended to be distributed beyond INL or ICP.")]
        Internal,
        [Display(Name = "External Report", ShortName = "EXT", Description = "A technical report that is primarily intended for distribution beyond the INL.")]
        External,
        [Display(Name = "Journal", ShortName = "JOU", Description = "A manuscript being submitted for publication in a journal.")]
        Journal,
        [Display(Name = "Conference", ShortName = "CON", Description = "An abstract, summary, full paper, poster session, or slide presentation for a  technical or professional conference. An abstract submitted to the conference for consideration must be reviewed/approved in advance, and then subsequent submissions, such as a full paper, slides, posters, etc., must be submitted for review before being sent to the conference organizer.")]
        Conference,
        [Display(Name = "Miscellaneous", ShortName = "MIS", Description = "This category includes documents intended for distribution beyond INL,  but which DO NOT carry an INL or DOE number (e.g., company controlled documents), or a website intended to be made available outside the firewall. Some examples of miscellaneous documents include Engineering Design Files, Technical/ Functional Requirements,Specifications, Emergency Plan Implementing Procedures, Technical Safety Requirements, etc.  ")]
        Misc,
        [Display(Name = "Limited Report", ShortName = "LTD", Description = "A document prepared for an external customer or audience, but which is intended for a closely defined, limited distribution, usually at the customer’s request.")]
        Limited,
        [Display(Name = "DOE/ID # Generation", ShortName = "DOE/ID #", Description = "A technical report that conveys the appearance of being written and issued by the DOE Idaho Operations Office. Numbers for this type of report are obtained bysending an e-mail request, containing the title and author(s) name(s), to Dorraine Burt(burtdc@inel.gov), or Dale Claflin (dfc@inel.gov) ")]
        DoeId,
        [Display(Name = "Proposal", ShortName = "PRO", Description = "A proposal intended for distribution beyond INL; this sometimes includes pre-proposals or ‘white papers.’")]
        Proposal,
        [Display(Name = "Expo", ShortName = "EXP", Description = "A technical report that is primarily intended for distribution beyond the INL. For tracking interns.’")]
        Expo,
        [Display(Name = "Historical", ShortName = "HST", Description = "Historical Documents")]
        Historical
    }

    public enum JournalTypeEnum
    {
        [Display(Name = "Announcement Citation Only", ShortName = "AC")] AnnouncementCitation,
        [Display(Name = "Author's Early Manuscript", ShortName = "FT")] EarlyManuscript,
        [Display(Name = "Accepted Manuscript - Author's Final Version", ShortName = "AM")] AcceptedManuscript
    }

    public enum AuthorAffilitionEnum
    {
        [Display(Name = "Idaho National Laboratory")] INL,
        University,
        Other
    }

    public enum ReviewerTypeEnum
    {
        None,
        [Display(Name = "Manager")] Manager,
        [Display(Name = "Classification")] Classification,
        [Display(Name = "Export Control")] ExportControl,
        [Display(Name = "Tech Deployment")] TechDeployment,
        [Display(Name = "Peer / Technical")] PeerTechnical
    }

    public enum ReviewerTypeShortEnum
    {
        [Display(Name = "Manager")] Manager,
        [Display(Name = "Peer / Technical")] PeerTechnical
    }

    public enum ReviewerTypeGenericEnum
    {
        [Display(Name = "Classification")] Classification,
        [Display(Name = "Export Control")] ExportControl,
        [Display(Name = "Tech Deployment")] TechDeployment
    }

    public enum StatusEnum
    {
        [Display(Name = "New")] New,
        [Display(Name = "Under Peer Review")] InPeerReview,
        [Display(Name = "Under Review")] InReview,
        [Display(Name = "Approved")] Completed,
        [Display(Name = "Rejected")] Rejected,
        [Display(Name = "Cancelled")] Cancelled,
        [Display(Name = "Deleted")] Deleted,
        [Display(Name = "Review Not Required")] ReviewNotRequired
    }

    public enum EmailTypeEnum
    {
        [Display(Name = "Reminder to Reviewer")] ReviewReminder,
        [Display(Name = "Peer Review to Reviewer")] PeerReviewer,
        [Display(Name = "Manager Review to Reviewer")] ManagerReviewer,
        [Display(Name = "Classification Review to Generic Email")] ClassReviewer,
        [Display(Name = "Export Compliance Review to Generic Email")] ExportReviewer,
        [Display(Name = "Technical Deployment Review to Generic Email")] TechReviewer,
        [Display(Name = "Review was Cancelled to Reviewer")] CancelledReviewer,
        [Display(Name = "Review is Complete to Owner")] ReviewComplete,
        [Display(Name = "Reviewer Declined to Review to Owner")] ReviewerDeclinedReview,
        [Display(Name = "Reviewer Commented on Review to Owner")] ReviewerCommented,
        [Display(Name = "Artifact Returned from Reviewer to Owner")] Return,
        [Display(Name = "Artifact Rejected by Reviewer to Owner")] Rejected,
        [Display(Name = "Artifact Approved by Reviewer to Owner")] Approved,
        [Display(Name = "All Reviews are Complete to Owner")] Complete,
        [Display(Name = "Artifact is Complete to Release Officer")] ReleaseOfficerComplete,
        [Display(Name = "Artifact was Deleted by Admin to Owner")] DeletedArtifact
    }

    public enum ReviewStatusEnum
    {
        [Display(Name = "Pending")] New,
        [Display(Name = "Under Review")] Active,
        [Display(Name = "Complete")] Complete,
        [Display(Name = "Approved")] Approved,
        [Display(Name = "Rejected")] Rejected,
        [Display(Name = "Chose not to review")] NotReviewing,
        [Display(Name = "Returned")] Returned
    }

    public enum ViewModeOptionEnum
    {
        [Display(Name = "In Process")] InProcess,
        [Display(Name = "Under Review")] UnderReview,
        [Display(Name = "All (Except Cancelled)")] All,
        Cancelled,
        [Display(Name = "Completed")] Complete,
        Rejected
    }

    public enum SearchDates
    {
        [Display(Name = "Date Artifact was created")]
        CreateDate = 1,
        [Display(Name = "Date Artifact was approved")]
        ApprovedDate = 2,
        [Display(Name = "Artifact last activity Date")]
        ActivityDate = 3
    }
}
