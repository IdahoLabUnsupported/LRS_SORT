using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Inl.MvcHelper;
using Sort.Business;

namespace Sort.Mvc.Models.Report
{
    public class ReportExportModel
    {
        #region Constants
        private const int MetaDataRowCount = 3;
        private const int ContactRowCount = 5;
        private const int AuthorRowCount = 7;
        private const int FundingRowCount = 22;
        private const int ReviewersRowCount = 8;
        private const string DefaultSheetName = "Main Data";
        private const string ContactSheetName = "Contacts";
        private const string AuthorsSheetName = "Authors";
        private const string FundingSheetName = "Funding";
        private const string SubjectsSheetName = "Subjects";
        private const string SponsoringOrgsSheetName = "Sponsoring Orgs";
        private const string KeywordsSheetName = "Keywords";
        private const string ReviewersSheetName = "Reviewers";
        #endregion

        #region Properties
        [Required]
        public List<int> SelectedIds { get; set; }
        [Required]
        public List<string> ColSelection { get; set; }
        public List<string> SheetSelection { get; set; }
        #endregion

        #region Constructor
        public ReportExportModel()
        {
            SelectedIds = new List<int>();
            ColSelection = new List<string>();
            SheetSelection = new List<string>();
        }
        #endregion

        #region Public Functions
        public List<SortMainObject> Sorts
        {
            get
            {
                var items = new List<SortMainObject>();
                foreach (int id in SelectedIds)
                {
                    items.Add(SortMainObject.GetSortMain(id));
                }
                return items;
            }
        }

        public byte[] GenerateExcelFile()
        {
            // Create main export with requested fields.
            var sortFile = Talon.Excel(Sorts).Name(DefaultSheetName).Sheet(DefaultSheetName).Columns(c =>
            {
                c.Bound(x => x.TitleStr).Title("STI Number and Rev");
                foreach (var column in ColSelection)
                {
                    switch (column)
                    {
                        case "PublishTitle":
                            c.Bound(x => x.PublishTitle).Title("Title");
                            break;
                        case "Osti":
                            c.Bound(x => x.OstiId).Title("OSTI ID");
                            c.Bound(x => x.OstiDate).Title("OSTI Publish Date");
                            c.Bound(x => x.OstiStatus).Title("OSTI Publish Status");
                            c.Bound(x => x.OstiStatusMsg).Title("OSTI Publish Message");
                            break;
                        case "PublicationDate":
                            c.Bound(x => x.PublicationDate).Title("Publication/Issue Date");
                            break;
                        case "ProductType":
                            c.Bound(x => x.ProductTypeDisplayName).Title("Artifact Type");
                            break;
                        case "ReportNumbers":
                            c.Bound(x => x.ReportNumbers).Title("Report/Product Number(s)");
                            break;
                        case "Country":
                            c.Bound(x => x.Country).Title("Country of Origin/Publication");
                            break;
                        case "Language":
                            c.Bound(x => x.Language).Title("Publication Language");
                            break;
                        case "AccessLimitation":
                            c.Bound(x => x.AccessLimitationDisplayName).Title("Access Limitation");
                            break;
                        case "Abstract":
                            c.Bound(x => x.AbstractWithAccessCheck).Title("Description/Abstract");
                            break;
                        case "Publisher":
                            c.Bound(x => x.PublisherName).Title("Publisher Name");
                            c.Bound(x => x.PublisherCity).Title("Publisher City");
                            c.Bound(x => x.PublisherState).Title("Publisher State");
                            c.Bound(x => x.PublisherCountry).Title("Publisher Country");
                            break;
                        case "Conference":
                            c.Bound(x => x.ConferenceName).Title("Conference Name");
                            c.Bound(x => x.ConferenceLocation).Title("Conference Location");
                            c.Bound(x => x.ConferenceBeginDate).Title("Conference Start Date");
                            c.Bound(x => x.ConferenceEndDate).Title("Conference End Date");
                            break;
                        case "Journal":
                            c.Bound(x => x.JournalTypeDisplayName).Title("Journal Type");
                            c.Bound(x => x.JournalName).Title("Journal Name");
                            c.Bound(x => x.JournalVolume).Title("Journal Volume");
                            c.Bound(x => x.JournalIssue).Title("Journal Issue");
                            c.Bound(x => x.JournalSerial).Title("Journal Serial");
                            c.Bound(x => x.JournalStartPage).Title("Journal Beginning Page");
                            c.Bound(x => x.JournalEndPage).Title("Journal Ending Page");
                            c.Bound(x => x.JournalDoi).Title("Journal DOI");
                            break;
                        case "Patent":
                            c.Bound(x => x.PatentAssignee).Title("Patent Assignee");
                            break;
                        case "OpenNet":
                            c.Bound(x => x.OpenNetData.AccessNumber).Title("OpenNet Accession Number");
                            c.Bound(x => x.OpenNetData.DocLocation).Title("OpenNet Document Location");
                            c.Bound(x => x.OpenNetData.FieldOfficeAym).Title("OpenNet Applicable Field Office Acronym");
                            c.Bound(x => x.OpenNetData.DeclassStatusEnumDisplayName).Title("OpenNet Declassification Status");
                            c.Bound(x => x.OpenNetData.DeclassificationDate).Title("OpenNet Declassificaton Date");
                            c.Bound(x => x.OpenNetData.Keywords).Title("OpenNet Document Keywords");
                            break;
                        case "ProtectedData":
                            c.Bound(x => x.ProtectedData.Crada).Title("Protected Data CRADA");
                            c.Bound(x => x.ProtectedData.ReleaseDate).Title("Protected Access Limitation Release Date");
                            c.Bound(x => x.ProtectedData.ExemptNumber).Title("Protected Exemption Number");
                            c.Bound(x => x.ProtectedData.Description).Title("Protected Description");
                            break;
                        case "OfficialUseOnly":
                            c.Bound(x => x.AccessReleaseDate).Title("Official Use Access Limitation Release Date");
                            c.Bound(x => x.ExemptionNumber).Title("Official Use Exemption Number");
                            break;
                    }
                }
            });

            int currentColumn = 1;
            int currentRow = Sorts.Count + 3;

            // Setup all output text before exporting
            if (ColSelection.Contains("Contacts") && !SheetSelection.Contains("Contacts")) { sortFile.SetValue(currentColumn, currentRow, "Contacts"); currentColumn += ContactRowCount; }
            if (ColSelection.Contains("Authors") && !SheetSelection.Contains("Authors")) { sortFile.SetValue(currentColumn, currentRow, "Authors"); currentColumn += AuthorRowCount; }
            if (ColSelection.Contains("Funding") && !SheetSelection.Contains("Funding")) { sortFile.SetValue(currentColumn, currentRow, "Funding"); currentColumn += FundingRowCount; }
            if (ColSelection.Contains("SubjectCategories") && !SheetSelection.Contains("SubjectCategories")) { sortFile.SetValue(currentColumn, currentRow, "Subjects"); currentColumn += MetaDataRowCount; }
            if (ColSelection.Contains("SponsoringOrgs") && !SheetSelection.Contains("SponsoringOrgs")) { sortFile.SetValue(currentColumn, currentRow, "Sponsoring Orgs"); currentColumn += MetaDataRowCount; }
            if (ColSelection.Contains("Keywords") && !SheetSelection.Contains("Keywords")) { sortFile.SetValue(currentColumn, currentRow, "Keywords"); currentColumn += MetaDataRowCount; }
            if (ColSelection.Contains("Reviewers") && !SheetSelection.Contains("Reviewers")) { sortFile.SetValue(currentColumn, currentRow, "Reviewers"); currentColumn += ReviewersRowCount; }

            // Export for a template
            byte[] excelFile = sortFile.Export();

            // Setup columns for data additions
            currentColumn = 1;
            currentRow++;

            // Add in all extra information
            if (ColSelection.Contains("Contacts")) { excelFile = GenerateContactsExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Authors")) { excelFile = GenerateAuthorsExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Funding")) { excelFile = GenerateFundingExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("SubjectCategories")) { excelFile = GenerateSubjectCategoryExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("SponsoringOrgs")) { excelFile = GenerateSponsoringOrgsExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Keywords")) { excelFile = GenerateKeywordsExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Reviewers")) { excelFile = GenerateReviewersExcel(excelFile, currentRow, ref currentColumn); }

            return excelFile;
        }
        #endregion

        #region Private Functions

        private byte[] GenerateContactsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<ContactObject> contacts = new List<ContactObject>();
            foreach (var c in Sorts)
            {
                contacts.AddRange(c.Contacts);
            }
            bool newSheet = SheetSelection.Contains("Contacts");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += ContactRowCount;
            }

            var contactFile = Talon.Excel(contacts)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? ContactSheetName : DefaultSheetName)
                .Name("ContactsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.SortTitle).Title("STI Number and Rev");
                    c.Bound(x => x.EmployeeId).Title("Employee ID");
                    c.Bound(x => x.FullName).Title("Name");
                    c.Bound(x => x.ContactTypeDisplayName).Title("Contact Type");
                });

            return contactFile.Export();
        }

        private byte[] GenerateAuthorsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<AuthorObject> authors = new List<AuthorObject>();
            foreach (var c in Sorts)
            {
                authors.AddRange(c.Authors);
            }

            bool newSheet = SheetSelection.Contains("Authors");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += AuthorRowCount;
            }

            var contactFile = Talon.Excel(authors)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? AuthorsSheetName : DefaultSheetName)
                .Name("AuthorsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.SortTitle).Title("STI Number and Rev");
                    c.Bound(x => x.EmployeeId).Title("Employee ID");
                    c.Bound(x => x.FullName).Title("Name");
                    c.Bound(x => x.Affiliation).Title("Affiliation");
                    c.Bound(x => x.OrcidId).Title("Orcid ID");
                    c.Bound(x => x.IsPrimary).Title("Primary");
                });

            return contactFile.Export();
        }

        private byte[] GenerateFundingExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<FundingObject> funding = new List<FundingObject>();
            foreach (var c in Sorts)
            {
                funding.AddRange(c.Funding);
            }

            bool newSheet = SheetSelection.Contains("Funding");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += FundingRowCount;
            }

            var contactFile = Talon.Excel(funding)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? FundingSheetName : DefaultSheetName)
                .Name("FundingTable")
                .Columns(c =>
                {
                    c.Bound(x => x.SortTitle).Title("STI Number and Rev");
                    c.Bound(x => x.Year).Title("Fiscal Year");
                    c.Bound(x => x.FundingType).Title("Funding Source");
                    c.Bound(x => x.FundingOrgName).Title("Funding Org");
                    c.Bound(x => x.Percent).Title("Percent");
                    c.Bound(x => x.ContractNumber).Title("Contract Number");
                    c.Bound(x => x.ProjectArea).Title("Project Area");
                    c.Bound(x => x.DoeFundingCategory).Title("DOE Program");
                    c.Bound(x => x.MilestoneTrackingNumber).Title("Milestone Tracking Number");
                    c.Bound(x => x.GrantNumber).Title("Grant");
                    c.Bound(x => x.TrackingNumber).Title("Tracking Number");
                    c.Bound(x => x.ProjectNumber).Title("Project Number");
                    c.Bound(x => x.PrincipalInvEmployeeId).Title("Principal Investigator Employee ID");
                    c.Bound(x => x.PrincipalInvName).Title("Principal Investigator");
                    c.Bound(x => x.OtherDescription).Title("Other Description");
                    c.Bound(x => x.SppFundingName).Title("SPP Category");
                    c.Bound(x => x.SppApproved).Title("SPP Approved");
                    c.Bound(x => x.ApproveNoReason).Title("SPP Not Approved Reason");
                    c.Bound(x => x.FederalAgency).Title("SPP Federal Agency");
                    c.Bound(x => x.SppCountry).Title("SPP Foreign Institution Country");
                    c.Bound(x => x.AdditionalInfo).Title("SPP Foreign Institution Additional Info");
                });

            return contactFile.Export();
        }

        private byte[] GenerateSubjectCategoryExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<SortMetaDataObject> category = new List<SortMetaDataObject>();
            foreach (var c in Sorts)
            {
                category.AddRange(c.SubjectCategories);
            }

            bool newSheet = SheetSelection.Contains("SubjectCategories");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += MetaDataRowCount;
            }

            var contactFile = Talon.Excel(category)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? SubjectsSheetName : DefaultSheetName)
                .Name("SubjectCategoriesTable")
                .Columns(c =>
                {
                    c.Bound(x => x.SortTitle).Title("STI Number and Rev");
                    c.Bound(x => x.Data).Title("Subject");
                });

            return contactFile.Export();
        }

        private byte[] GenerateSponsoringOrgsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<SortMetaDataObject> orgs = new List<SortMetaDataObject>();
            foreach (var c in Sorts)
            {
                orgs.AddRange(c.SponsoringOrgs);
            }

            bool newSheet = SheetSelection.Contains("SponsoringOrgs");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += MetaDataRowCount;
            }

            var contactFile = Talon.Excel(orgs)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? SponsoringOrgsSheetName : DefaultSheetName)
                .Name("SponsoringOrgsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.SortTitle).Title("STI Number and Rev");
                    c.Bound(x => x.Data).Title("Sponsor");
                });

            return contactFile.Export();
        }

        private byte[] GenerateKeywordsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<SortMetaDataObject> words = new List<SortMetaDataObject>();
            foreach (var c in Sorts)
            {
                words.AddRange(c.KeyWordList);
            }

            bool newSheet = SheetSelection.Contains("Keywords");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += MetaDataRowCount;
            }

            var contactFile = Talon.Excel(words)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? KeywordsSheetName : DefaultSheetName)
                .Name("KeywordsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.SortTitle).Title("STI Number and Rev");
                    c.Bound(x => x.Data).Title("Keyword");
                });

            return contactFile.Export();
        }

        private byte[] GenerateReviewersExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<ReviewObject> words = new List<ReviewObject>();
            foreach (var c in Sorts)
            {
                words.AddRange(c.Reviewers);
            }

            bool newSheet = SheetSelection.Contains("Reviewers");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += ReviewersRowCount;
            }

            var contactFile = Talon.Excel(words)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? ReviewersSheetName : DefaultSheetName)
                .Name("ReviewersTable")
                .Columns(c =>
                {
                    c.Bound(x => x.SortTitle).Title("STI Number and Rev");
                    c.Bound(x => x.Reviewer).Title("Reviewer");
                    c.Bound(x => x.ReviewerType).Title("Reviewer Type");
                    c.Bound(x => x.ReviewDate).Title("Review Date");
                    c.Bound(x => x.Approval).Title("Approved");
                    c.Bound(x => x.Reason).Title("Reason");
                    c.Bound(x => x.Comments).Title("Comments");
                });

            return contactFile.Export();
        }

        #endregion
    }
}