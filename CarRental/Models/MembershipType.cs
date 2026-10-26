using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class MembershipType
    {
        #region Summary Properties

        [Display(Name = "Discount")]
        public string DiscountSummary => Discount.ToString("P0");

        #endregion

        [Key]
        public int ID { get; set; }

        [Display(Name = "Membership")]
        [Required(ErrorMessage = "Membership type is required.")]
        [StringLength(50, ErrorMessage = "Membership type cannot be more than 50 characters long.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Discount is required.")]
        [Range(0, 0.5, ErrorMessage = "Discount must be between 0 and 0.5 (0% - 50%).")]
        public double Discount { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot be more than 200 characters long.")]
        public string? Description { get; set; }

        public ICollection<Customer> Customers { get; set; } = new HashSet<Customer>();
    }
}
