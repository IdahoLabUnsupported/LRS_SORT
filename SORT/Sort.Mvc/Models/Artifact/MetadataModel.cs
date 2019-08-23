using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class MetadataModel : IValidatableObject
    {
        #region Properties

        public int? MetaDataId { get; set; }
        [Required]
        public int SortMainId { get; set; }
        [Required]
        public string MetaDataType { get; set; }

        public string Sponsor { get; set; }
        public string Keyword { get; set; }
        public int? SubjectId { get; set; }

        [Display(Name = "Core Capability")]
        public int? CoreCapabilityId { get; set; }

        private MetaDataTypeEnum MetaDataTypeEnum => MetaDataType.ToEnum<MetaDataTypeEnum>();

        public List<SortMetaDataObject> Metadata { get; set; } = new List<SortMetaDataObject>();
        #endregion

        public MetadataModel() { }

        public MetadataModel(int mainId, MetaDataTypeEnum dataType)
        {
            SortMainId = mainId;
            MetaDataType = dataType.ToString();
            Metadata = SortMetaDataObject.GetSortMetaDatas(SortMainId, MetaDataTypeEnum);
        }

        public void Save()
        {
            switch (MetaDataTypeEnum)
            {
                case MetaDataTypeEnum.SubjectCategories:
                    var sub = MemoryCache.GetSubjectCategory(SubjectId.Value);
                    if (!SortMetaDataObject.GetSortMetaDatas(SortMainId, MetaDataTypeEnum).Exists(n => n.Data.Equals(sub.FullSubject, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        SortMetaDataObject.AddNew(MetaDataTypeEnum, SortMainId, sub.FullSubject);
                    }
                    break;
                case MetaDataTypeEnum.SponsoringOrgs:
                    if (!SortMetaDataObject.GetSortMetaDatas(SortMainId, MetaDataTypeEnum).Exists(n => n.Data.Equals(Sponsor, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        SortMetaDataObject.AddNew(MetaDataTypeEnum, SortMainId, Sponsor);
                    }
                    break;
                case MetaDataTypeEnum.Keywords:
                    if (!SortMetaDataObject.GetSortMetaDatas(SortMainId, MetaDataTypeEnum).Exists(n => n.Data.Equals(Keyword.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        SortMetaDataObject.AddNew(MetaDataTypeEnum, SortMainId, Keyword.Trim());
                    }
                    break;
                case MetaDataTypeEnum.CoreCapabilities:
                    var core = MemoryCache.GetCoreCapability(CoreCapabilityId.Value);
                    if (!SortMetaDataObject.GetSortMetaDatas(SortMainId, MetaDataTypeEnum).Exists(n => n.Data.Equals(core.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        SortMetaDataObject.AddNew(MetaDataTypeEnum, SortMainId, core.Name);
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