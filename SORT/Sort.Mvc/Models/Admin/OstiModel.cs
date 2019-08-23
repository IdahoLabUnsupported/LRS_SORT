using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sort.Mvc.Models
{
    public class OstiModel : IValidatableObject
    {
        [Required, Display(Name = "Username")]
        public string Username { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "New password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password), Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public OstiModel() { }

        public OstiModel(string userName)
        {
            Username = userName;
        }

        public void Save()
        {
            Config.OstiUserName = Username;
            Config.OstiPassword = NewPassword;
        }

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                yield return new ValidationResult("Username is required", new[] { "Username" });
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                yield return new ValidationResult("New password is required", new[] { "NewPassword" });
            }
            else if (!NewPassword.Equals(ConfirmPassword, StringComparison.InvariantCulture))
            {
                yield return new ValidationResult("New password and Confirm password are not identical", new[] { "NewPassword" });
            }
        }

        #endregion
    }
}