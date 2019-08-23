using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class AuthorModel : IValidatableObject
    {
        #region Properties

        public int? AuthorId { get; set; }
        [Required]
        public int MainId { get; set; }
        [Required]
        public string Affiliation { get; set; }
        [Display(Name = "Company Affiliation")]
        public AuthorAffilitionEnum AffiliationType { get; set; }
        [Display(Name = "Employee")]
        public string AffiliateEmployeeId { get; set; }
        [Display(Name = "Author Name")]
        public string AuthorName { get; set; }
        [Display(Name = "Orcid ID")]
        public string AuthorOrcidId { get; set; }
        [Display(Name = "Affiliation Country")]
        public int? CountryId { get; set; }
        [Display(Name = "Affiliation State")]
        public int? StateId { get; set; }
        public bool IsPrimary { get; set; }
        public List<AuthorObject> Authors { get; set; }
        #endregion

        #region Constructor

        public AuthorModel() { }

        public AuthorModel(int mainId, int? authorId)
        {
            MainId = mainId;
            Authors = AuthorObject.GetAuthors(MainId);

            if (authorId.HasValue)
            {
                var author = AuthorObject.GetAuthor(authorId.Value);
                if (author != null)
                {
                    AuthorId = author.AuthorId;
                    Affiliation = author.Affiliation;
                    AuthorName = author.Name;
                    AuthorOrcidId = author.OrcidId;
                    IsPrimary = author.IsPrimary;
                    AffiliationType = author.AffiliationEnum;
                    AffiliateEmployeeId = author.EmployeeId;
                    CountryId = author.CountryId ?? Helper.UnitedStatesCountryId;
                    StateId = author.StateId;
                }
            }
            else
            {
                CountryId = Helper.UnitedStatesCountryId;
            }
        }
        #endregion

        #region Functions

        public void Save()
        {
            if (MainId > 0)
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
                        if (AffiliationType == AuthorAffilitionEnum.INL)
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
                    AuthorObject.Add(AuthorId, MainId, AffiliateEmployeeId, AuthorName, AffiliationType, Affiliation, AuthorOrcidId, IsPrimary, CountryId, StateId);
                }
            }
        }

        #endregion

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AffiliationType == AuthorAffilitionEnum.INL && string.IsNullOrWhiteSpace(AffiliateEmployeeId))
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