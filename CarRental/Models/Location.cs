using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Location
    {   
        public int ID { get; set; }

        [Display(Name = "Name")] //Must be Unique
        [Required(ErrorMessage = "Name is blank, please enter a name")]
        [StringLength(50, ErrorMessage = "Name cannot be greater than 50 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string City { get; set; } = "";

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(7, ErrorMessage = "Postal Code cannot exceed 7 characters.")]
        [RegularExpression(@"[A-Za-z]\d[A-Za-z] ?\d[A-Za-z]\d$",
            ErrorMessage = "Enter a valid Canadian postal code (e.g. L2G 3L7")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$",
        ErrorMessage = "Phone number must contain exactly 10 digits.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = "";

        //Navigation Properties
        public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();

        public ICollection<VehicleLocation> VehicleLocations { get; set; } = new HashSet<VehicleLocation>();



    }
}
