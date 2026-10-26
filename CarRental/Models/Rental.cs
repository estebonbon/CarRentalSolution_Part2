using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Rental
    {
        public int ID { get; set; }

        [Display(Name = "Rental Number")] //Unique
        [Required(ErrorMessage = "Rental Number cannot be blank please type in your number")]
        [StringLength(20, ErrorMessage = "Rental number cannot be greater than 20 characters")]
        public string RentalNumber { get; set; } = string.Empty;

        [Display(Name = "Pickup Date")]
        [Required(ErrorMessage = "Pickup Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime PickupDate { get; set; }

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        
        public DateTime? ReturnDate { get; set; } //Nullable value

        [Display(Name = "Customer")]
        [Required(ErrorMessage = "Customer input cannot be left blank")]

        public int CustomerID { get; set; }
        public Customer? Customer { get; set; }

        [Display(Name = "Employee")]
        public int? EmployeeID { get; set; }

        public Employee? Employee { get; set; }

    }
}
