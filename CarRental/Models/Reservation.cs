using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Reservation : IValidatableObject
    {
        #region Summary Properties

        //Full pickup date and time "Mon, Oct 12, 2026 @ 9:00 AM"
        [Display(Name = "Pickup")]
        public string PickupSummary => PickupDate.ToString("ddd, MMM d, yyyy '@' h:mm tt");

        //Full return date and time "Thu, Oct 15, 2026 @ 5:00 PM"
        [Display(Name = "Return")]
        public string ReturnSummary => ReturnDate.ToString("ddd, MMM d, yyyy '@' h:mm tt");

        #endregion

        [Key]
        public int ID { get; set; }

        [Display(Name = "Reservation #")]
        [Required(ErrorMessage = "Reservation number is required.")]
        [StringLength(20, ErrorMessage = "Reservation number cannot be more than 20 characters long.")]
        public string ReservationNumber { get; set; } = "";

        [Display(Name = "Pickup Date")]
        [Required(ErrorMessage = "Pickup date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime PickupDate { get; set; }

        [Display(Name = "Return Date")]
        [Required(ErrorMessage = "Return date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "Reservation status is required.")]
        public Status Status { get; set; }

        [Required(ErrorMessage = "You must select an employee.")]
        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }

        public Employee? Employee { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Location cannot be left blank")]
        public int LocationID { get; set; }

        public Location? Location { get; set; }

        //Navigation Properties 2B
        public ICollection<CustomerReservation> CustomerReservations { get; set; } = new HashSet<CustomerReservation>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //New Reservations only: a reservation cannot be created with a Pickup Date in the past.
            //Before a new Reservation is saved to the database, its ID has the default value of 0.
            //After it is saved, the database generates an ID greater than 0.
            //Checking ID == 0 lets us apply this validation only when creating a new Reservation,
            //while still allowing existing Reservations with past Pickup Dates to be edited.
            if (ID == 0 && PickupDate < DateTime.Today)
            {
                yield return new ValidationResult("Pickup date cannot be in the past.", new[] { nameof(PickupDate) });
            }

            //The return date must be later than the pickup date.
            if (ReturnDate <= PickupDate)
            {
                yield return new ValidationResult("Return date must be later than the pickup date.", new[] { nameof(ReturnDate) });
            }

            //The reservation cannot be longer than 30 days.
            if (ReturnDate > PickupDate.AddDays(30))
            {
                yield return new ValidationResult("Reservation period cannot exceed 30 days.", new[] { nameof(ReturnDate) });
            }
        }
    }
}
