using AABC.Domain2.Cases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AABC.Web.App.Staffing.Models
{
    public class AddParentContactLogVM
    {
        public int CaseId { get; set; }

        [Required(ErrorMessage = "Contact Person is required.")]
        public int GuardianRelationshipId { get; set; }

        public string ContactedPersonName { get; set; }

        [Required(ErrorMessage = "Contact Date is required.")]
        public DateTime ContactDate { get; set; }

        [Required(ErrorMessage = "Contact Method is required.")]
        public ContactMethodTypes ContactMethodType { get; set; }

        [CustomValidation(typeof(AddParentContactLogVM), "ValidateContactMethodValue")]
        public string ContactMethodValue { get; set; }

        [Required(ErrorMessage = "Notes is required.")]
        public string Notes { get; set; }

        public List<Domain.General.GuardianRelationship> Relationships { get; set; }

        public static ValidationResult ValidateContactMethodValue(string value, ValidationContext context)
        {
            if (!(context.ObjectInstance is AddParentContactLogVM model) || !string.IsNullOrEmpty(value))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(model.ContactMethodType == ContactMethodTypes.Phone
                ? "Phone Number is required."
                : "Email Address is required.");
        }
    }
}