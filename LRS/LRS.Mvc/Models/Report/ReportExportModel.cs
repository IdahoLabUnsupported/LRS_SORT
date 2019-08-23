using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Inl.MvcHelper;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class ReportExportModel
    {
        #region Constants
        private const int MetaDataRowCount = 3;
        private const int ContactRowCount = 8;
        private const int AuthorRowCount = 7;
        private const int FundingRowCount = 23;
        private const int ReviewersRowCount = 7;
        private const int ReviewerCommentsRowCount = 5;
        private const int ReviewerHistoryRowCount = 8;
        private const int IntellectualProprtyRowCount = 7;
        private const string DefaultSheetName = "Artifact Data";
        private const string ContactSheetName = "Contacts";
        private const string AuthorsSheetName = "Authors";
        private const string FundingSheetName = "Funding";
        private const string SubjectsSheetName = "Subjects";
        private const string IntellectualsSheetName = "Intellectual Property";
        private const string KeywordsSheetName = "Keywords";
        private const string ReviewersSheetName = "Reviewers";
        private const string ReviewerHistorySheetName = "Reviewer History";
        #endregion

        #region Properties

        [Required]
        public List<int> SelectedIds { get; set; } = new List<int>();
        [Required]
        public List<string> ColSelection { get; set; } = new List<string>();
        public List<string> SheetSelection { get; set; } = new List<string>();
        #endregion

        #region Constructor
        public ReportExportModel(){}
        #endregion

        #region Public Functions
        public List<MainObject> Mains
        {
            get
            {
                var items = new List<MainObject>();
                foreach (int id in SelectedIds)
                {
                    items.Add(MainObject.GetMain(id));
                }
                return items;
            }
        }

        public byte[] GenerateExcelFile()
        {
            // Create main export with requested fields.
            var mainSheet = Talon.Excel(Mains).Name(DefaultSheetName).Sheet(DefaultSheetName).Columns(c =>
            {
                c.Bound(x => x.StiNumber).Title("STI Number");
                c.Bound(x => x.Revision).Title("Revision");
                foreach (var column in ColSelection)
                {
                    switch (column)
                    {
                        case "Title":
                            c.Bound(x => x.Title).Title("Title");
                            break;
                        case "Status":
                            c.Bound(x => x.StatusDisplayName).Title("Status");
                            break;
                        case "NumberPages":
                            c.Bound(x => x.NumberPagesStr).Title("Number Pages");
                            break;
                        case "ProductType":
                            c.Bound(x => x.DocumentTypeStr).Title("Product Type");
                            break;
                        case "CreationDate":
                            c.Bound(x => x.CreateDateStr).Title("Creation Date");
                            break;
                        case "ApprovedDate":
                            c.Bound(x => x.ApprovalDateStr).Title("Approved Date");
                            break;
                        case "Abstract":
                            c.Bound(x => x.AbstractWithAccessCheck).Title("Abstract");
                            break;
                        case "Conference":
                            c.Bound(x => x.ConferenceName).Title("Conference Name");
                            c.Bound(x => x.ConferenceSponsor).Title("Conference Sponsor");
                            c.Bound(x => x.ConferenceLocation).Title("Conference Location");
                            c.Bound(x => x.ConferenceBeginDateStr).Title("Conference Start Date");
                            c.Bound(x => x.ConferenceEndDateStr).Title("Conference End Date");
                            break;
                        case "Journal":
                            c.Bound(x => x.JournalName).Title("Journal Name");
                            break;
                    }
                }
            });

            int currentColumn = 1;
            int currentRow = Mains.Count + 3;

            // Setup all output text before exporting
            if (ColSelection.Contains("Contacts") && !SheetSelection.Contains("Contacts")) { mainSheet.SetValue(currentColumn, currentRow, "Contacts"); currentColumn += ContactRowCount; }
            if (ColSelection.Contains("Authors") && !SheetSelection.Contains("Authors")) { mainSheet.SetValue(currentColumn, currentRow, "Authors"); currentColumn += AuthorRowCount; }
            if (ColSelection.Contains("Funding") && !SheetSelection.Contains("Funding")) { mainSheet.SetValue(currentColumn, currentRow, "Funding"); currentColumn += FundingRowCount; }
            if (ColSelection.Contains("Subjects") && !SheetSelection.Contains("Subjects")) { mainSheet.SetValue(currentColumn, currentRow, "Subjects"); currentColumn += MetaDataRowCount; }
            if (ColSelection.Contains("Intellectual") && !SheetSelection.Contains("Intellectual")) { mainSheet.SetValue(currentColumn, currentRow, "Intellectual Property"); currentColumn += IntellectualProprtyRowCount; }
            if (ColSelection.Contains("Keywords") && !SheetSelection.Contains("Keywords")) { mainSheet.SetValue(currentColumn, currentRow, "Keywords"); currentColumn += MetaDataRowCount; }

            if (ColSelection.Contains("Reviewers") && !SheetSelection.Contains("Reviewers"))
            {
                mainSheet.SetValue(currentColumn, currentRow, "Reviewers"); currentColumn += ReviewersRowCount;
                mainSheet.SetValue(currentColumn, currentRow, "Reviewer Comments"); currentColumn += ReviewerCommentsRowCount;
            }

            if (ColSelection.Contains("ReviewerHistory") && !SheetSelection.Contains("ReviewerHistory"))
            {
                mainSheet.SetValue(currentColumn, currentRow, "Reviewer History"); currentColumn += ReviewerHistoryRowCount;
                mainSheet.SetValue(currentColumn, currentRow, "Reviewer Comments History"); currentColumn += ReviewerHistoryRowCount;
            }

            // Export for a template
            byte[] excelFile = mainSheet.Export();

            // Setup columns for data additions
            currentColumn = 1;
            currentRow++;

            // Add in all extra information
            if (ColSelection.Contains("Contacts")) { excelFile = GenerateContactsExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Authors")) { excelFile = GenerateAuthorsExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Funding")) { excelFile = GenerateFundingExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Subjects")) { excelFile = GenerateSubjectExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Intellectual")) { excelFile = GenerateIntellectualPropertyExcel(excelFile, currentRow, ref currentColumn); }
            if (ColSelection.Contains("Keywords")) { excelFile = GenerateKeywordsExcel(excelFile, currentRow, ref currentColumn); }

            if (ColSelection.Contains("Reviewers"))
            {
                excelFile = GenerateReviewersExcel(excelFile, currentRow, ref currentColumn);
                excelFile = GenerateReviewerCommentsExcel(excelFile, currentRow, ref currentColumn);
            }

            if (ColSelection.Contains("ReviewerHistory"))
            {
                excelFile = GenerateReviewerHistoryExcel(excelFile, currentRow, ref currentColumn);
                excelFile = GenerateReviewerHistoryCommentsExcel(excelFile, currentRow, ref currentColumn);
            }

            return excelFile;
        }
        #endregion

        #region Private Functions

        private byte[] GenerateContactsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<ContactObject> contacts = new List<ContactObject>();
            foreach (var c in Mains)
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

            var data = Talon.Excel(contacts)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? ContactSheetName : DefaultSheetName)
                .Name("ContactsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
                    c.Bound(x => x.EmployeeId).Title("Employee ID");
                    c.Bound(x => x.Name).Title("Name");
                    c.Bound(x => x.Phone).Title("Phone #");
                    c.Bound(x => x.WorkOrg).Title("Org");
                    c.Bound(x => x.Location).Title("Location");
                    c.Bound(x => x.OrcidId).Title("Orcid ID");
                });

            return data.Export();
        }

        private byte[] GenerateAuthorsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<AuthorObject> authors = new List<AuthorObject>();
            foreach (var c in Mains)
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

            var data = Talon.Excel(authors)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? AuthorsSheetName : DefaultSheetName)
                .Name("AuthorsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
                    c.Bound(x => x.EmployeeId).Title("Employee ID");
                    c.Bound(x => x.Name).Title("Name");
                    c.Bound(x => x.Affiliation).Title("Affiliation");
                    c.Bound(x => x.OrcidId).Title("Orcid ID");
                    c.Bound(x => x.IsPrimary).Title("Primary");
                });

            return data.Export();
        }

        private byte[] GenerateFundingExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<FundingObject> funding = new List<FundingObject>();
            foreach (var c in Mains)
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

            var data = Talon.Excel(funding)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? FundingSheetName : DefaultSheetName)
                .Name("FundingTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
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

            return data.Export();
        }

        private byte[] GenerateSubjectExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<MetaDataObject> category = new List<MetaDataObject>();
            foreach (var c in Mains)
            {
                category.AddRange(c.SubjectCategories);
            }

            bool newSheet = SheetSelection.Contains("Subjects");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += MetaDataRowCount;
            }

            var data = Talon.Excel(category)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? SubjectsSheetName : DefaultSheetName)
                .Name("SubjectsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
                    c.Bound(x => x.Data).Title("Subject");
                });

            return data.Export();
        }

        private byte[] GenerateIntellectualPropertyExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<IntellectualPropertyObject> intellect = new List<IntellectualPropertyObject>();
            foreach (var c in Mains)
            {
                intellect.AddRange(c.Intellectuals);
            }

            bool newSheet = SheetSelection.Contains("Intellectual");
            int row = 1;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += IntellectualProprtyRowCount;
            }

            var data = Talon.Excel(intellect)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? IntellectualsSheetName : DefaultSheetName)
                .Name("IntellectualsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
                    c.Bound(x => x.IdrNumber).Title("IDR Number");
                    c.Bound(x => x.DocketNumber).Title("Docket Number");
                    c.Bound(x => x.Aty).Title("Aty");
                    c.Bound(x => x.Ae).Title("Ae");
                    c.Bound(x => x.Title).Title("Title");
                });

            return data.Export();
        }

        private byte[] GenerateKeywordsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<MetaDataObject> words = new List<MetaDataObject>();
            foreach (var c in Mains)
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

            var data = Talon.Excel(words)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? KeywordsSheetName : DefaultSheetName)
                .Name("KeywordsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
                    c.Bound(x => x.Data).Title("Keyword");
                });

            return data.Export();
        }

        private byte[] GenerateReviewersExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<ReviewObject> reviewers = new List<ReviewObject>();
            foreach (var c in Mains)
            {
                reviewers.AddRange(c.Reviewers);
            }

            bool newSheet = SheetSelection.Contains("Reviewers");
            int row = 2;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += ReviewersRowCount;
            }

            var data = Talon.Excel(reviewers)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? ReviewersSheetName : DefaultSheetName)
                .Name("ReviewersTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainDisplayTitle).Title("STI Number and Rev");
                    c.Bound(x => x.ReviewerName).Title("Reviewer");
                    c.Bound(x => x.ReviewerTypeDisplayName).Title("Reviewer Type");
                    c.Bound(x => x.ReviewDateStr).Title("Date Reviewed");
                    c.Bound(x => x.ReviewStatusTxt).Title("Status");
                    c.Bound(x => x.StatusDateStr).Title("Status Date");
                    c.Bound(x => x.NumberPagesStr).Title("Number Pages");
                });

            if (newSheet)
            {
                data.SetValue(1, 1, "Reviewers");
            }

            return data.Export();
        }

        private byte[] GenerateReviewerCommentsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<ReviewCommentObject> reviewercomments = new List<ReviewCommentObject>();
            foreach (var c in Mains)
            {
                reviewercomments.AddRange(ReviewCommentObject.GetReviewComments(c.MainId.Value));
            }

            bool newSheet = SheetSelection.Contains("Reviewers");
            int row = 2;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += ReviewerCommentsRowCount;
            }
            else
            {
                column += ReviewersRowCount;
            }

            var data = Talon.Excel(reviewercomments)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? ReviewersSheetName : DefaultSheetName)
                .Name("ReviewerCommentsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
                    c.Bound(x => x.ReviewerName).Title("Reviewer");
                    c.Bound(x => x.EntryDateStr).Title("Comment Date");
                    c.Bound(x => x.Comment).Title("Comment");
                });

            if (newSheet)
            {
                data.SetValue(column, 1, "Comments");
            }

            return data.Export();
        }

        private byte[] GenerateReviewerHistoryExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<ReviewHistoryObject> reviewers = new List<ReviewHistoryObject>();
            foreach (var c in Mains)
            {
                reviewers.AddRange(c.ReviewHistory);
            }

            bool newSheet = SheetSelection.Contains("ReviewerHistory");
            int row = 2;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += ReviewerHistoryRowCount;
            }

            var data = Talon.Excel(reviewers)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? ReviewerHistorySheetName : DefaultSheetName)
                .Name("ReviewerHistoryTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
                    c.Bound(x => x.ReviewerName).Title("Reviewer");
                    c.Bound(x => x.ReviewerTypeDisplayName).Title("Reviewer Type");
                    c.Bound(x => x.ReviewDateStr).Title("Date Reviewed");
                    c.Bound(x => x.ReviewStatusTxt).Title("Last Status");
                    c.Bound(x => x.StatusDateStr).Title("Status Date");
                    c.Bound(x => x.NumberPagesStr).Title("Number Pages");
                    c.Bound(x => x.HistoryDateStr).Title("Date Moved to History");
                });

            if (newSheet)
            {
                data.SetValue(1, 1, "Reviewers");
            }

            return data.Export();
        }

        private byte[] GenerateReviewerHistoryCommentsExcel(byte[] template, int currentRow, ref int currentColumn)
        {
            List<ReviewCommentHistoryObject> reviewercomments = new List<ReviewCommentHistoryObject>();
            foreach (var c in Mains)
            {
                reviewercomments.AddRange(ReviewCommentHistoryObject.GetReviewComments(c.MainId.Value));
            }

            bool newSheet = SheetSelection.Contains("ReviewerHistory");
            int row = 2;
            int column = 1;
            if (!newSheet)
            {
                row = currentRow;
                column = currentColumn;
                currentColumn += ReviewerCommentsRowCount;
            }
            else
            {
                column += ReviewerHistoryRowCount;
            }

            var data = Talon.Excel(reviewercomments)
                .Template(template)
                .StartAt(column, row)
                .Sheet(newSheet ? ReviewerHistorySheetName : DefaultSheetName)
                .Name("ReviewerHistoryCommentsTable")
                .Columns(c =>
                {
                    c.Bound(x => x.MainTitle).Title("STI Number and Rev");
                    c.Bound(x => x.ReviewerName).Title("Reviewer");
                    c.Bound(x => x.EntryDateStr).Title("Comment Date");
                    c.Bound(x => x.Comment).Title("Comment");
                });

            if (newSheet)
            {
                data.SetValue(column, 1, "Comments");
            }

            return data.Export();
        }
        #endregion
    }
}