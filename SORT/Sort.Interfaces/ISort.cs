using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sort.Interfaces
{
    public interface ISort
    {
        string TitleStr { get; }
        int? SharePointId { get; }
        string ReportNumber { get; }
        [Display(Name = "Artifact Type", ShortName = "product_type")]
        string ProductType { get; }
        [Display(Name = "Publisher", ShortName = "publisher_information")]
        string PublisherInformation { get; }
        [Display(Name = "Conference", ShortName = "conference_information")]
        string ConferenceInfo { get; }
        [Display(Name = "Conference Name")]
        string ConferenceName { get; }
        [Display(Name = "Conference Location")]
        string ConferenceLocation { get; }
        [Display(Name = "Conference Start Date")]
        DateTime? ConferenceBeginDate { get; }
        [Display(Name = "Conference End Date")]
        DateTime? ConferenceEndDate { get; }
        [Display(Name = "Journal Type", ShortName = "journal_type")]
        string JournalType { get; }
        [Display(Name = "Name", ShortName = "journal_name")]
        string JournalName { get; }
        [Display(Name = "Volume", ShortName = "journal_volume")]
        string JournalVolume { get; }
        [Display(Name = "Issue", ShortName = "journal_issue")]
        string JournalIssue { get; }
        [Display(Name = "Serial", ShortName = "journal_serial_id")]
        string JournalSerial { get; }
        [Display(ShortName = "product_size")]
        string JournalPageRage { get; }
        string Authorstr { get; }
        string ContractNumber { get; }
        [Display(Name = "Publication/Issue Date", ShortName = "publication_date")]
        DateTime? PublicationDate { get; }
        [Display(Name = "Publication Language", ShortName = "language")]
        string Language { get; }
        [Display(Name = "Country of Origin/Publication", ShortName = "country_publication_code")]
        string Country { get; }
        [Display(Name = "Sponsoring Organization(s)", ShortName = "sponsor_org")]
        string SponsoringOrgStr { get; }
        [Display(Name = "Access Limitation", ShortName = "access_limitation")]
        string AccessLimitation { get; }
        [Display(ShortName = "released_date")]
        DateTime? ReleasedDate { get; }
        [Display(ShortName = "keywords")]
        string Keywords { get; }
        [Display(Name = "Description / Abstract", ShortName = "description"), MaxLength(5000)]
        [StringLength(5000)]
        string Abstract { get; }
        [Display(Name = "OSTI ID", ShortName = "osti_id")]
        string OstiId { get; }
    }
}
