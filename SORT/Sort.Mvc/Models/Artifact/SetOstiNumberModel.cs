using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class SetOstiNumberModel
    {
        [Required]
        public int SortMainId { get; set; }

        [Required, Display(Name = "OSTI Number")]
        public string OstiId { get; set; }

        public string DisplayTitle => SortMainObject.GetSortMain(SortMainId)?.DisplayTitle;

        public SetOstiNumberModel() { }

        public SetOstiNumberModel(int sortMainId)
        {
            SortMainId = sortMainId;
        }

        public void Save()
        {
            var data = SortMainObject.GetSortMain(SortMainId);
            if (data != null)
            {
                data.OstiId = OstiId;
                data.StatusEnum = StatusEnum.Published;
                data.Save();
            }
        }
    }
}