using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class IntellectualModel
    {
        #region Properties
        public int? IntellectualPropertyId { get; set; }
        public int MainId { get; set; }
        [Display(Name = "IDR Number")]
        public string IdrNumber { get; set; }
        [Display(Name = "Docket Number")]
        public string DocketNumber { get; set; }
        [Display(Name = "Aty")]
        public string Aty { get; set; }
        [Display(Name = "Ae")]
        public string Ae { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        public List<IntellectualPropertyObject> Intellectuals { get; set; }
        #endregion

        #region Constructor

        public IntellectualModel() { }

        public IntellectualModel(int mainId, int? intellectualId)
        {
            MainId = mainId;
            Intellectuals = IntellectualPropertyObject.GetIntellectualProperties(MainId);

            if (intellectualId.HasValue)
            {
                var ip = IntellectualPropertyObject.GetIntellectualProperty(intellectualId.Value);
                if (ip != null)
                {
                    IntellectualPropertyId = ip.IntellectualPropertyId;
                    IdrNumber = ip.IdrNumber;
                    DocketNumber = ip.DocketNumber;
                    Aty = ip.Aty;
                    Ae = ip.Ae;
                    Title = ip.Title;
                }
            }
        }
        #endregion

        #region Functions

        public void Save()
        {
            bool needSave = false;

            if (string.IsNullOrWhiteSpace(IdrNumber)) return;

            var data = IntellectualPropertyObject.GetIntellectualProperty(IntellectualPropertyId ?? 0) ?? new IntellectualPropertyObject();
            if (data.MainId == 0)
            {
                data.MainId = MainId;
            }

            if (IdrNumber?.Trim() != data.IdrNumber)
            {
                data.IdrNumber = IdrNumber?.Trim();
                needSave = true;
            }

            if (DocketNumber?.Trim() != data.DocketNumber)
            {
                data.DocketNumber = DocketNumber?.Trim();
                needSave = true;
            }

            if (Aty?.Trim() != data.Aty)
            {
                data.Aty = Aty?.Trim();
                needSave = true;
            }

            if (Ae?.Trim() != data.Ae)
            {
                data.Ae = Ae?.Trim();
                needSave = true;
            }

            if (Title?.Trim() != data.Title)
            {
                data.Title = Title?.Trim();
                needSave = true;
            }

            if (needSave)
            {
                data.Save();
            }
        }
        #endregion
    }
}