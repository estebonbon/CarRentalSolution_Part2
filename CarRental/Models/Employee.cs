using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Employee
    {
        #region Summary Properties

        [Display(Name = "Employee")]
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

        [Required(ErrorMessage = "Hire date is required.")]
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        //Navigation Properties
        public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();

        public ICollection<Rental> Rentals { get; set; } = new HashSet<Rental>();
    }
}
