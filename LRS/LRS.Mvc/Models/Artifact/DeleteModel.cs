using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class DeleteModel
    {
        [Required]
        public int MainId { get; set; }
        [Required]
        public string Reason { get; set; }

        public string StiNumber { get; set; }

        public DeleteModel() { }

        public DeleteModel(int mainId)
        {
            MainId = mainId;
            StiNumber = MainObject.GetMain(MainId)?.DisplayTitle;
        }

        public void Save()
        {
            if (Current.IsAdmin)
            {
                MainObject.GetMain(MainId)?.Delete(Reason);
            }
        }
    }
}