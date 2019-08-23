using System;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class ConferenceModel
    {
        #region Properties

        public int MainId { get; set; }
        public string ConferenceName { get; set; }
        public string ConferenceSponsor { get; set; }
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

            if (MainId > 0)
            {
                var o = MainObject.GetMain(MainId);
                if (o != null)
                {
                    if (ConferenceName?.Trim() != o.ConferenceName)
                    {
                        o.ConferenceName = ConferenceName?.Trim();
                        needSave = true;
                    }

                    if (ConferenceSponsor?.Trim() != o.ConferenceSponsor)
                    {
                        o.ConferenceSponsor = ConferenceSponsor?.Trim();
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