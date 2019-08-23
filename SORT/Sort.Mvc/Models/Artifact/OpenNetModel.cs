using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class OpenNetModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public string AccessNumber { get; set; }
        public string DocLocation { get; set; }
        public string FieldOfficeAym { get; set; }
        public string DeclassificationStatus { get; set; }
        public DateTime? DeclassificationDate { get; set; }
        public string Keywords { get; set; }

        #endregion

        #region Constructor

        public OpenNetModel() { }

        #endregion

        #region Functions

        public void Save()
        {
            bool needSave = false;

            if (SortMainId > 0)
            {
                var o = SortMainObject.GetSortMain(SortMainId).OpenNetData;

                if (o == null)
                {
                    o = new OpenNetObject(SortMainId);
                }

                if (o.SortMainId != SortMainId)
                {
                    o.SortMainId = SortMainId;
                }

                if (AccessNumber?.Trim() != o.AccessNumber)
                {
                    o.AccessNumber = AccessNumber?.Trim();
                    needSave = true;
                }

                if (DocLocation?.Trim() != o.DocLocation)
                {
                    o.DocLocation = DocLocation?.Trim();
                    needSave = true;
                }

                if (FieldOfficeAym?.Trim() != o.FieldOfficeAym)
                {
                    o.FieldOfficeAym = FieldOfficeAym?.Trim();
                    needSave = true;
                }

                if (DeclassificationStatus != o.DeclassificationStatus)
                {
                    o.DeclassificationStatus = DeclassificationStatus;
                    needSave = true;
                }

                if (DeclassificationDate != o.DeclassificationDate)
                {
                    o.DeclassificationDate = DeclassificationDate;
                    needSave = true;
                }

                if (Keywords?.Trim() != o.Keywords)
                {
                    o.Keywords = Keywords?.Trim();
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