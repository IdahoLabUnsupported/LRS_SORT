using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class ProtectedDataModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public bool Crada { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ExemptNumber { get; set; }
        public string Description { get; set; }
        #endregion

        #region Constructor

        public ProtectedDataModel() { }

        #endregion

        #region Functions

        public void Save()
        {
            bool needSave = false;

            if (SortMainId > 0)
            {
                var sort = SortMainObject.GetSortMain(SortMainId);
                var o = sort?.ProtectedData;

                if (o == null)
                {
                    o = new ProtectedDataObject(SortMainId);
                }

                if (o.SortMainId != SortMainId)
                {
                    o.SortMainId = SortMainId;
                }

                if (Crada != o.Crada)
                {
                    o.Crada = Crada;
                    needSave = true;
                }

                if (ReleaseDate != o.ReleaseDate)
                {
                    o.ReleaseDate = ReleaseDate;
                    needSave = true;
                }

                if (ExemptNumber?.Trim() != o.ExemptNumber)
                {
                    o.ExemptNumber = ExemptNumber?.Trim();
                    needSave = true;
                }

                if (Description?.Trim() != o.Description)
                {
                    o.Description = Description?.Trim();
                    needSave = true;
                }

                if (needSave)
                {
                    o.Save();
                }
            }
        }
        #endregion
    }
}