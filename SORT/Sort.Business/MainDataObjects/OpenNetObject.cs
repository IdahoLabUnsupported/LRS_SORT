using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dapper;

namespace Sort.Business
{
    public class OpenNetObject
    {
        #region Properties

        public int? OpenNetId { get; set; }
        public int SortMainId { get; set; }
        [Display(Name = "Accession Number", ShortName = "opennet_accession_no")]
        public string AccessNumber { get; set; }
        [Display(Name = "Document Location", ShortName = "opennet_doc_location")]
        public string DocLocation { get; set; }
        [Display(Name = "Applicable Field Office Acronym", ShortName = "opennet_fieldoffice_acronym")]
        public string FieldOfficeAym { get; set; }
        [Display(Name = "Declassification Status", ShortName = "opennet_declass_status")]
        public string DeclassificationStatus { get; set; }
        [Display(Name = "Declassification Date", ShortName = "opennet_declass_date")]
        public DateTime? DeclassificationDate { get; set; }
        [Display(Name = "Document Keywords", ShortName = "opennet_document_keywords")]
        public string Keywords { get; set; }
        #endregion

        #region Extended Properties
        public DeclassStatusEnum DeclassStatusEnum
        {
            get { return DeclassificationStatus.ToEnum<DeclassStatusEnum>(); }
            set { DeclassificationStatus = value.ToString(); }
        }

        public string DeclassStatusEnumDisplayName => DeclassStatusEnum.GetEnumDisplayName();

        public string Display => $"Accession Number: {AccessNumber}\nDocument Location: {DocLocation}\nApplicable Field Office Acronym: {FieldOfficeAym}\n" +
                                 $"Declassificaton Status: {DeclassStatusEnum.GetEnumDisplayName()}\nDeclassificaton Date: {DeclassificationDate}\n" +
                                 $"Document Keywords: {Keywords}";
        #endregion

        #region Constructor
        public OpenNetObject() { }

        public OpenNetObject(int sortMainId)
        {
            SortMainId = sortMainId;
        }
        #endregion

        #region Repository

        private static IOpenNetRepository repo => new OpenNetRepository();

        #endregion

        #region Static Methods

        public static OpenNetObject GetOpenNetData(int sortMainId) => repo.GetOpenNetData(sortMainId);

        public static OpenNetObject GetOpenNet(int openNetId) => repo.GetOpenNet(openNetId);

        #endregion

        #region Object Methods

        public void Save()
        {
            bool dataChanged = CheckDataChanged();
            repo.SaveOpenNet(this);
            SortMainObject.CheckStatusUpdate(SortMainId, dataChanged);
        }

        public void Delete()
        {
            repo.DeleteOpenNet(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        #endregion

        #region Private Functions

        private bool CheckDataChanged()
        {
            bool changed = false;
            if (OpenNetId.HasValue)
            {
                var o = GetOpenNet(OpenNetId.Value);
                if (!Extensions.StringsAreEqual(AccessNumber, o.AccessNumber)) changed = true;
                if (!Extensions.StringsAreEqual(DocLocation, o.DocLocation)) changed = true;
                if (!Extensions.StringsAreEqual(FieldOfficeAym, o.FieldOfficeAym)) changed = true;
                if (!Extensions.StringsAreEqual(DeclassificationStatus, o.DeclassificationStatus)) changed = true;
                if (!Extensions.StringsAreEqual(Keywords, o.Keywords)) changed = true;
                if (DeclassificationDate != o.DeclassificationDate) changed = true;
            }

            return changed;
        }

        #endregion
    }
    public interface IOpenNetRepository
    {
        OpenNetObject GetOpenNetData(int sortMainId);
        OpenNetObject GetOpenNet(int openNetId);
        OpenNetObject SaveOpenNet(OpenNetObject openNet);
        bool DeleteOpenNet(OpenNetObject openNet);
    }

    public class OpenNetRepository : IOpenNetRepository
    {
        public OpenNetObject GetOpenNetData(int sortMainId) => Config.Conn.Query<OpenNetObject>("SELECT * FROM dat_OpenNet WHERE SortMainId = @SortMainId", new { SortMainId  = sortMainId}).FirstOrDefault() ?? new OpenNetObject(sortMainId);

        public OpenNetObject GetOpenNet(int openNetId) => Config.Conn.Query<OpenNetObject>("SELECT * FROM dat_OpenNet WHERE OpenNetId = @OpenNetId", new { OpenNetId = openNetId }).FirstOrDefault();

        public OpenNetObject SaveOpenNet(OpenNetObject openNet)
        {
            if (openNet.OpenNetId.HasValue) // Update
            {
                string sql = @"
                    UPDATE  dat_OpenNet
                    SET     SortMainId = @SortMainId,
                            AccessNumber = @AccessNumber,
                            DocLocation = @DocLocation,
                            FieldOfficeAym = @FieldOfficeAym,
                            DeclassificationStatus = @DeclassificationStatus,
                            DeclassificationDate = @DeclassificationDate,
                            KeyWords = @KeyWords
                    WHERE   OpenNetId = @OpenNetId";
                Config.Conn.Execute(sql, openNet);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_OpenNet (
                        SortMainId,
                        AccessNumber,
                        DocLocation,
                        FieldOfficeAym,
                        DeclassificationStatus,
                        DeclassificationDate,
                        KeyWords
                    )
                    VALUES (
                        @SortMainId,
                        @AccessNumber,
                        @DocLocation,
                        @FieldOfficeAym,
                        @DeclassificationStatus,
                        @DeclassificationDate,
                        @KeyWords
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                openNet.OpenNetId = Config.Conn.Query<int>(sql, openNet).Single();
            }
            return openNet;
        }

        public bool DeleteOpenNet(OpenNetObject openNet)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_OpenNet WHERE OpenNetId = @OpenNetId", openNet);
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("OpenNetObject::DeleteOpenNet", ex);
                return false;
            }
            return true;
        }
    }
}
