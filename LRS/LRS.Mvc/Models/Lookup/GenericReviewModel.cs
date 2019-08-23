using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class GenericReviewModel
    {
        [Display(Name = "Classification General Email")]
        public string ClassGeneralEmail { get; set; }
        [Display(Name = "Export Compliance General Email")]
        public string ExportGeneralEmail { get; set; }
        [Display(Name = "Technical Deployment General Email")]
        public string TechGeneralEmail { get; set; }

        public GenericReviewModel() { }

        public GenericReviewModel(List<GenericReviewDataObject> data)
        {
            ClassGeneralEmail = data.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification)?.GenericEmail;
            ExportGeneralEmail = data.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl)?.GenericEmail;
            TechGeneralEmail = data.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment)?.GenericEmail;
        }

        public void Save()
        {
            var data = MemoryCache.GetGenericReviewData();
            var cge = data.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification) ?? new GenericReviewDataObject();
            var ege = data.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl) ?? new GenericReviewDataObject();
            var tge = data.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment) ?? new GenericReviewDataObject();

            if (!string.IsNullOrWhiteSpace(ClassGeneralEmail))
            {
                cge.ReviewerTypeEnum = ReviewerTypeEnum.Classification;
                cge.GenericEmail = ClassGeneralEmail.Trim();
                cge.Save();
            }
            else if(cge.GenericReviewDataId > 0)
            {
                cge.Delete();
            }

            if (!string.IsNullOrWhiteSpace(ExportGeneralEmail))
            {
                ege.ReviewerTypeEnum = ReviewerTypeEnum.ExportControl;
                ege.GenericEmail = ExportGeneralEmail.Trim();
                ege.Save();
            }
            else if (ege.GenericReviewDataId > 0)
            {
                ege.Delete();
            }

            if (!string.IsNullOrWhiteSpace(TechGeneralEmail))
            {
                tge.ReviewerTypeEnum = ReviewerTypeEnum.TechDeployment;
                tge.GenericEmail = TechGeneralEmail.Trim();
                tge.Save();
            }
            else if (tge.GenericReviewDataId > 0)
            {
                tge.Delete();
            }
        }
    }
}