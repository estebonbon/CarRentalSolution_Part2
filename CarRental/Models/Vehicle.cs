using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Vehicle
    {
        public int ID { get; set; }

        [Display(Name = "Make")]
        [Required(ErrorMessage = "Make is blank, type in the make of the vechile")]
        [StringLength(50, ErrorMessage = "Make cannot be greater than 50 characters")]
        public string Make { get; set; } = string.Empty;

        [Display(Name = "Model")]
        [Required(ErrorMessage = "Model is blank, type in the model of the vechile")]
        [StringLength(50, ErrorMessage = "Model cannot be greater than 50 characters")]
        public string Model{ get; set; } = string.Empty;

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is required.")]
        [Range(2021, 2027, ErrorMessage = "Year must be between 2021 and 2027.")]
        public int Year { get; set; }

        [Display(Name = "License Plate")]
        [Required(ErrorMessage = "License Plate is required.")]
        [StringLength(8, ErrorMessage = "License Plate cannot exceed 8 characters.")]
        //[RegularExpression(@"^[A-Z]{4}[0-9]{3}$",
        //ErrorMessage = "License plate must have 4 letters followed by 3 numbers.")]
        public string LicensePlate { get; set; } = "";

        [Required(ErrorMessage = "Daily Rate is required.")]
        [Range(25.00, 500.00, ErrorMessage = "Daily Rate must be between $25 and $500.")]
        [DataType(DataType.Currency)]//Formats the currency
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Daily Rate")]
        public double DailyRate { get; set; }

        //Navigation Properties
        public ICollection<VehicleLocation> VehicleLocations { get; set; } = new HashSet<VehicleLocation>();

        public override string ToString()
        {
            return $"{Year} {Make} {Model}";
        }

    }
}
