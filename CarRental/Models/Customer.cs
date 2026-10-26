using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Customer : IValidatableObject
    {
        #region Summary Properties

        [Display(Name = "Customer")]
        public string Summary
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                        (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }

        [Display(Name = "Phone")]
        public string PhoneFormatted
        {
            get
            {
                if (!string.IsNullOrEmpty(Phone) && Phone.Length == 10 && Phone.All(char.IsDigit))
                {
                    return "(" + Phone.Substring(0, 3) + ") "
                        + Phone.Substring(3, 3) + "-" + Phone[6..];
                }

                return Phone;
            }
        }

        #endregion

        [Key]
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle name cannot be more than 50 characters long.")]
        public string? MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(75, ErrorMessage = "Last name cannot be more than 75 characters long.")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit phone number.")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(255, ErrorMessage = "Email cannot be more than 255 characters long.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "";

        [Display(Name = "Driver License")]
        [Required(ErrorMessage = "Driver license is required.")]
        [StringLength(15, ErrorMessage = "Driver license cannot be more than 15 characters long.")]
        public string DriverLicense { get; set; } = "";

        [Required(ErrorMessage = "Date of birth is required.")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "You must select a membership type.")]
        [Display(Name = "Membership Type")]
        public int MembershipTypeID { get; set; }

        public MembershipType? MembershipType { get; set; }

        //Navigation Properties
        public ICollection<Rental> Rentals { get; set; } = new HashSet<Rental>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB.Date > DateTime.Today)
            {
                yield return new ValidationResult("Date of birth cannot be in the future.", new[] { nameof(DOB) });
            }
            else if (DOB.Date > DateTime.Today.AddYears(-19))
            {
                yield return new ValidationResult("Customer must be at least 19 years old.", new[] { nameof(DOB) });
            }
        }
    }
}
