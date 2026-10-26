namespace CarRental.Models
{
    public class VehicleLocation
    {
        public int VehicleID { get; set; }

        public Vehicle? Vehicle { get; set; } //Navigation property

        public int LocationID  { get; set; }

        public Location? Location { get; set; } //Navigation property


    }
}
