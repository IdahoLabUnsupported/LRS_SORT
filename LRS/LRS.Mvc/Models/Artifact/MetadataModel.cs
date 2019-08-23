using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Inl.MvcHelper;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class MetadataModel : IValidatableObject
    {
        #region Properties

        public int? MetaDataId { get; set; }
        [Required]
        public int MainId { get; set; }
        [Required]
        public string MetaDataType { get; set; }

        public string Sponsor { get; set; }
        [Display(Name = "Keyword")]
        public string Keyword { get; set; }
        [Display(Name = "Subject")]
        public int? SubjectId { get; set; }
        [Display(Name = "Core Capability")]
        public int? CoreCapabilityId { get; set; }


        private MetaDataTypeEnum MetaDataTypeEnum
        {
            get => MetaDataType.ToEnum<MetaDataTypeEnum>();
            set => MetaDataType = value.ToString();
        } 

        public List<MetaDataObject> Metadata { get; set; } = new List<MetaDataObject>();
        #endregion

        public MetadataModel() { }

        public MetadataModel(int mainId, MetaDataTypeEnum dataType)
        {
            MainId = mainId;
            MetaDataTypeEnum = dataType;
            Metadata = MetaDataObject.GetMetaDatas(MainId, MetaDataTypeEnum);
        }

        public void Save()
        {
            switch (MetaDataTypeEnum)
            {
                case MetaDataTypeEnum.SubjectCategories:
                    var sub = MemoryCache.GetSubjectCategory(SubjectId.Value);
                    if (!MetaDataObject.GetMetaDatas(MainId, MetaDataTypeEnum).Exists(n => n.Data.Equals(sub.FullSubject, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MetaDataObject.AddNew(MetaDataTypeEnum, MainId, sub.FullSubject);
                    }
                    break;
                case MetaDataTypeEnum.SponsoringOrgs:
                    if (!MetaDataObject.GetMetaDatas(MainId, MetaDataTypeEnum).Exists(n => n.Data.Equals(Sponsor, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MetaDataObject.AddNew(MetaDataTypeEnum, MainId, Sponsor);
                    }
                    break;
                case MetaDataTypeEnum.Keywords:
                    if (!MetaDataObject.GetMetaDatas(MainId, MetaDataTypeEnum).Exists(n => n.Data.Equals(Keyword.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MetaDataObject.AddNew(MetaDataTypeEnum, MainId, Keyword.Trim());
                    }
                    break;
                case MetaDataTypeEnum.CoreCapabilities:
                    var core = MemoryCache.GetCoreCapability(CoreCapabilityId.Value);
                    if (!MetaDataObject.GetMetaDatas(MainId, MetaDataTypeEnum).Exists(n => n.Data.Equals(core.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        MetaDataObject.AddNew(MetaDataTypeEnum, MainId, core.Name);
                    }
                    break;
            }
        }

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (MetaDataTypeEnum)
            {
                case MetaDataTypeEnum.SubjectCategories:
                    if (!SubjectId.HasValue)
                    {
                        yield return new ValidationResult("Subject is required", new[] { "SubjectId" });
                    }
                    break;
                case MetaDataTypeEnum.SponsoringOrgs:
                    if (string.IsNullOrWhiteSpace(Sponsor))
                    {
                        yield return new ValidationResult("Sponsoring Orginization is required", new[] { "Sponsor" });
                    }
                    break;
                case MetaDataTypeEnum.Keywords:
                    if (string.IsNullOrWhiteSpace(Keyword))
                    {
                        yield return new ValidationResult("Keyword is required", new[] { "Keyword" });
                    }
                    break;
            }
        }

        #endregion
    }
}