

namespace CarRental.Models
{
    public class CustomerReservation
    {
        public int CustomerID { get; set; }

        public Customer? Customer { get; set; }

        public int ReservationID { get; set; }

        public Reservation? Reservation { get; set; }
    }
}
