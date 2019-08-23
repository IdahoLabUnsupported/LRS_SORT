using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class AbstractModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public string Abstract { get; set; }

        #endregion

        #region Constructor

        public AbstractModel() { }

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
                    if (!string.IsNullOrWhiteSpace(Abstract) && Abstract.Trim().Length > 5000)
                    {
                        Abstract = Abstract.Trim().Substring(0, 5000);
                    }

                    if (Abstract?.Trim() != o.Abstract)
                    {
                        o.Abstract = Abstract?.Trim();
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