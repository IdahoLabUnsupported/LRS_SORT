using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class JournalModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public string JournalType { get; set; }
        public string JournalName { get; set; }
        public string JournalVolume { get; set; }
        public string JournalIssue { get; set; }
        public string JournalSerial { get; set; }
        public int? JournalStartPage { get; set; }
        public int? JournalEndPage { get; set; }
        public string JournalDoi { get; set; }
        #endregion

        #region Constructor

        public JournalModel() { }

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
                    if (JournalType != o.JournalType)
                    {
                        o.JournalType = JournalType;
                        needSave = true;
                    }

                    if (JournalName?.Trim() != o.JournalName)
                    {
                        o.JournalName = JournalName?.Trim();
                        needSave = true;
                    }

                    if (JournalVolume?.Trim() != o.JournalVolume)
                    {
                        o.JournalVolume = JournalVolume?.Trim();
                        needSave = true;
                    }

                    if (JournalIssue?.Trim() != o.JournalIssue)
                    {
                        o.JournalIssue = JournalIssue.Trim();
                        needSave = true;
                    }

                    if (JournalSerial?.Trim() != o.JournalSerial)
                    {
                        o.JournalSerial = JournalSerial?.Trim();
                        needSave = true;
                    }

                    if (JournalStartPage != o.JournalStartPage)
                    {
                        o.JournalStartPage = JournalStartPage;
                        needSave = true;
                    }

                    if (JournalEndPage != o.JournalEndPage)
                    {
                        o.JournalEndPage = JournalEndPage;
                        needSave = true;
                    }

                    if (JournalDoi?.Trim() != o.JournalDoi)
                    {
                        o.JournalDoi = JournalDoi?.Trim();
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