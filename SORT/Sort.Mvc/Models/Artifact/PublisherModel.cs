using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class PublisherModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public string PublisherName { get; set; }
        public string PublisherCity { get; set; }
        public string PublisherState { get; set; }
        public string PublisherCountry { get; set; }
        #endregion

        #region Constructor

        public PublisherModel() { }

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
                    if (PublisherName?.Trim() != o.PublisherName)
                    {
                        o.PublisherName = PublisherName?.Trim();
                        needSave = true;
                    }

                    if (PublisherCity?.Trim() != o.PublisherCity)
                    {
                        o.PublisherCity = PublisherCity?.Trim();
                        needSave = true;
                    }

                    if (PublisherState?.Trim() != o.PublisherState)
                    {
                        o.PublisherState = PublisherState?.Trim();
                        needSave = true;
                    }

                    if (PublisherCountry != o.PublisherCountry)
                    {
                        o.PublisherCountry = PublisherCountry;
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