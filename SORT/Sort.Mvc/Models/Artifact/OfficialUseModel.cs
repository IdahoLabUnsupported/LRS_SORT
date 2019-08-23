using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class OfficialUseModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public DateTime? AccessReleaseDate { get; set; }
        public string ExemptionNumber { get; set; }

        #endregion

        #region Constructor

        public OfficialUseModel() { }
        
        #endregion

        #region Functions

        public void Save()
        {
            bool needSave = false;

            if (SortMainId > 0)
            {
                var o = SortMainObject.GetSortMain(SortMainId);
                if (o != null)
                {
                    if (AccessReleaseDate != o.AccessReleaseDate)
                    {
                        o.AccessReleaseDate = AccessReleaseDate;
                        needSave = true;
                    }

                    if (ExemptionNumber?.Trim() != o.ExemptionNumber)
                    {
                        o.ExemptionNumber = ExemptionNumber?.Trim();
                        needSave = true;
                    }

                    if (needSave)
                    {
                        o.Save();
                    }
                }
            }
        }
        
        #endregion
    }
}