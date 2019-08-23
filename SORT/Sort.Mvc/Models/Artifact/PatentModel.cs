using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class PatentModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public string PatentAssignee { get; set; }
        #endregion

        #region Constructor

        public PatentModel() { }

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
                    if (PatentAssignee?.Trim() != o.PatentAssignee)
                    {
                        o.PatentAssignee = PatentAssignee?.Trim();
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