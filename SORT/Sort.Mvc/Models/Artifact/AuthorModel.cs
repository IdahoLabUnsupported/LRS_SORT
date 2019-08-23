using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class AuthorModel : IAuthor, IValidatableObject
    {
        #region Properties

        public int? AuthorId { get; set; }
        [Required]
        public int SortMainId { get; set; }
        [Required]
        public string Affiliation { get; set; }
        public AffiliationEnum AffiliationType { get; set; }
        public string AffiliateEmployeeId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorOrcidId { get; set; }
        public bool IsPrimary { get; set; }
        public List<AuthorObject> Authors { get; set; }
        [Display(Name = "Affiliation Country")]
        public int? CountryId { get; set; }
        [Display(Name = "Affiliation State")]
        public int? StateId { get; set; }
        #endregion

        #region Constructor

        public AuthorModel() { }

        public AuthorModel(AuthorObject author)
        {
            if (author != null)
            {
                SortMainId = author.SortMainId;
                AuthorId = author.AuthorId;
                Affiliation = author.Affiliation;
                AuthorName = author.FullName;
                AuthorOrcidId = author.OrcidId;
                IsPrimary = author.IsPrimary;
                AffiliationType = author.AffiliationEnum;
                AffiliateEmployeeId = author.EmployeeId;
                Authors = AuthorObject.GetAuthors(SortMainId);
            }
        }
        #endregion

        #region Functions

        public void Save()
        {
            if (SortMainId > 0)
            {
                if (AuthorId.HasValue)
                {
                    var author = AuthorObject.GetAuthor(AuthorId.Value);
                    if (author != null)
                    {
                        author.AffiliationEnum = AffiliationType;
                        author.Affiliation = Affiliation;
                        author.OrcidId = AuthorOrcidId;
                        author.IsPrimary = IsPrimary;
                        author.EmployeeId = AffiliateEmployeeId;
                        author.SetName(AuthorName, AffiliationType, AffiliateEmployeeId);
                        if (AffiliationType == AffiliationEnum.INL)
                        {
                            author.CountryId = null;
                            author.StateId = null;
                        }
                        else
                        {
                            author.CountryId = CountryId;
                            if (CountryId.HasValue && CountryId.Value == Helper.UnitedStatesCountryId)
                            {
                                author.StateId = StateId;
                            }
                            else
                            {
                                author.StateId = null;
                            }
                        }
                        author.Save();
                    }
                }
                else
                {
                    AuthorObject.Add(AuthorId, SortMainId, AffiliateEmployeeId, AuthorName, AffiliationType, Affiliation, AuthorOrcidId, IsPrimary, CountryId, StateId);
                }
            }
        }

        #endregion

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AffiliationType == AffiliationEnum.INL && string.IsNullOrWhiteSpace(AffiliateEmployeeId))
            {
                yield return new ValidationResult("Author Name is required", new[] { "AffiliateEmployeeId" });
            }

            if (string.IsNullOrWhiteSpace(AuthorName))
            {
                yield return new ValidationResult("Author Name  is required", new[] { "AuthorName" });
            }
        }

        #endregion
    }
}