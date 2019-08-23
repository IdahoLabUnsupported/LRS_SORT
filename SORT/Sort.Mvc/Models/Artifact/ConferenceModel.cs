using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class ConferenceModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public string ConferenceName { get; set; }
        public string ConferenceLocation { get; set; }
        public DateTime? ConferenceBeginDate { get; set; }
        public DateTime? ConferenceEndDate { get; set; }

        #endregion

        #region Constructor

        public ConferenceModel() { }

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
                    if (ConferenceName?.Trim() != o.ConferenceName)
                    {
                        o.ConferenceName = ConferenceName?.Trim();
                        needSave = true;
                    }

                    if (ConferenceLocation?.Trim() != o.ConferenceLocation)
                    {
                        o.ConferenceLocation = ConferenceLocation?.Trim();
                        needSave = true;
                    }

                    if (ConferenceBeginDate != o.ConferenceBeginDate)
                    {
                        o.ConferenceBeginDate = ConferenceBeginDate;
                        needSave = true;
                    }

                    if (ConferenceEndDate != o.ConferenceEndDate)
                    {
                        o.ConferenceEndDate = ConferenceEndDate;
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