using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class TitleModel
    {
        #region Properties

        public int SortMainId { get; set; }
        public string Title { get; set; }
        public string PublishTitle { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string ProductType { get; set; }
        public string ReportNumbers { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public string AccessLimitation { get; set; }
        public string DoiNum { get; set; }
        public bool HasTechWriter { get; set; }
        public string TechWriterEmployeeId { get; set; }
        #endregion

        #region Constructor

        public TitleModel() { }

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
                    //if (Title?.Trim() != o.Title)
                    //{
                    //    o.Title = Title?.Trim();
                    //    needSave = true;
                    //}

                    if (PublishTitle?.Trim() != o.PublishTitle)
                    {
                        o.PublishTitle = PublishTitle?.Trim();
                        needSave = true;
                    }

                    if (PublicationDate != o.PublicationDate)
                    {
                        o.PublicationDate = PublicationDate;
                        needSave = true;
                    }

                    if (ProductType != o.ProductType)
                    {
                        o.ProductType = ProductType;
                        needSave = true;
                    }

                    if (ReportNumbers?.Trim() != o.ReportNumbers)
                    {
                        o.ReportNumbers = ReportNumbers?.Trim();
                        needSave = true;
                    }

                    if (Country != o.Country)
                    {
                        o.Country = Country;
                        needSave = true;
                    }

                    if (Language != o.Language)
                    {
                        o.Language = Language;
                        needSave = true;
                    }

                    if (AccessLimitation != o.AccessLimitation)
                    {
                        o.AccessLimitation = AccessLimitation;
                        needSave = true;
                    }

                    if (DoiNum?.Trim() != o.DoiNum)
                    {
                        o.DoiNum = DoiNum?.Trim();
                        needSave = true;
                    }

                    if (HasTechWriter != o.HasTechWriter)
                    {
                        o.HasTechWriter = HasTechWriter;
                        needSave = true;
                    }

                    if (HasTechWriter)
                    {
                        if (TechWriterEmployeeId.Trim() != o.TechWriterEmployeeId)
                        {
                            o.TechWriterEmployeeId = TechWriterEmployeeId.Trim();
                            needSave = true;
                        }
                    }
                    else if(!string.IsNullOrWhiteSpace(o.TechWriterEmployeeId))
                    {
                        o.TechWriterEmployeeId = null;
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