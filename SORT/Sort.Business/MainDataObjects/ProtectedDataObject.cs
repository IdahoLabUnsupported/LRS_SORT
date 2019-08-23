using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dapper;

namespace Sort.Business
{
    public class ProtectedDataObject
    {
        #region Properties

        public int? ProtectedDataId { get; set; }
        public int SortMainId { get; set; }

        [Display(Name = "CRADA", ShortName = "protected_data_crada")]
        public bool Crada { get; set; }
        [Display(Name = "Description", ShortName = "protected_data_desc")]
        public string Description { get; set; }
        [Display(Name = "Access Limitation Release Date", ShortName = "access_limitation_rel_date")]
        public DateTime? ReleaseDate { get; set; }
        [Display(Name = "Exemption Number", ShortName = "exemption_number")]
        public string ExemptNumber { get; set; }
        #endregion

        #region Extended Properties

        public string Display
        {
            get
            {
                string display = $"CRADA: {Crada}\nAccess Limitation Release Date: {ReleaseDate}\n Exemption Number: {ExemptNumber}";
                if (!Crada)
                    display += $"\nDescription: {Description}";

                return display;
            }
        } 
        #endregion

        #region Constructor
        public ProtectedDataObject(){}

        public ProtectedDataObject(int sortMainId)
        {
            SortMainId = sortMainId;
        }
        #endregion

        #region Repository

        private static IProtectedDataRepository repo => new ProtectedDataRepository();

        #endregion

        #region Static Methods

        public static ProtectedDataObject GetProtectedData(int protectedDataId) => repo.GetProtectedData(protectedDataId);

        public static ProtectedDataObject GetProtectedDataForSortMain(int sortMainId) => repo.GetProtectedDataForSortMain(sortMainId);
        #endregion

        #region Object Methods

        public void Save()
        {
            bool dataChanged = CheckDataChanged();
            repo.SaveProtectedData(this);
            SortMainObject.CheckStatusUpdate(SortMainId, dataChanged);
        }

        public void Delete()
        {
            repo.DeleteProtectedData(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        #endregion

        #region Private Functions

        private bool CheckDataChanged()
        {
            bool changed = false;
            if (ProtectedDataId.HasValue)
            {
                var o = GetProtectedData(ProtectedDataId.Value);
                if (!Extensions.StringsAreEqual(Description, o.Description)) changed = true;
                if (!Extensions.StringsAreEqual(ExemptNumber, o.ExemptNumber)) changed = true;
                if (ReleaseDate != o.ReleaseDate) changed = true;
                if (Crada != o.Crada) changed = true;
            }

            return changed;
        }

        #endregion
    }

    public interface IProtectedDataRepository
    {
        ProtectedDataObject GetProtectedData(int protectedDataId);
        ProtectedDataObject GetProtectedDataForSortMain(int sortMainId);
        ProtectedDataObject SaveProtectedData(ProtectedDataObject protectedData);
        bool DeleteProtectedData(ProtectedDataObject protectedData);
    }

    public class ProtectedDataRepository : IProtectedDataRepository
    {
        public ProtectedDataObject GetProtectedData(int protectedDataId) => Config.Conn.Query<ProtectedDataObject>("SELECT * FROM dat_ProtectedData WHERE ProtectedDataId = @ProtectedDataId", new { ProtectedDataId = protectedDataId }).FirstOrDefault();
        public ProtectedDataObject GetProtectedDataForSortMain(int sortMainId) => Config.Conn.Query<ProtectedDataObject>("SELECT * FROM dat_ProtectedData WHERE SortMainId = @SortMainId", new { SortMainId = sortMainId }).FirstOrDefault() ?? new ProtectedDataObject(sortMainId);

        public ProtectedDataObject SaveProtectedData(ProtectedDataObject protectedData)
        {
            if (protectedData.ProtectedDataId.HasValue) // Update
            {
                string sql = @"
                    UPDATE  dat_ProtectedData
                    SET     SortMainId = @SortMainId,
                            Crada = @Crada,
                            Description = @Description,
                            ReleaseDate = @ReleaseDate,
                            ExemptNumber = @ExemptNumber
                    WHERE   ProtectedDataId = @ProtectedDataId";
                Config.Conn.Execute(sql, protectedData);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_ProtectedData (
                        SortMainId,
                        Crada,
                        Description,
                        ReleaseDate,
                        ExemptNumber
                    )
                    VALUES (
                        @SortMainId,
                        @Crada,
                        @Description,
                        @ReleaseDate,
                        @ExemptNumber
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                protectedData.ProtectedDataId = Config.Conn.Query<int>(sql, protectedData).Single();
            }
            return protectedData;
        }

        public bool DeleteProtectedData(ProtectedDataObject protectedData)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_ProtectedData WHERE ProtectedDataId = @ProtectedDataId", protectedData);
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("ProtectedDataObject::DeleteProtectedData", ex);
                return false;
            }
            return true;
        }
    }

}
