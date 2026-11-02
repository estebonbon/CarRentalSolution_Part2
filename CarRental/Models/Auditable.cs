using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public abstract class Auditable : IAuditable //Uses the interface, Only Vehicle, Customer, and Reservation will inherit from this class, so it is abstract.
    {
        [ScaffoldColumn(false)] //Prevents Scaffolding from generating a field for this property 
        [StringLength(256)]
        public string? CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? CreatedOn { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string? UpdatedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? UpdatedOn { get; set; }
    }
}
