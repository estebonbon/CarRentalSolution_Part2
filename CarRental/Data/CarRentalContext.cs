using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class CarRentalContext : DbContext
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<MembershipType> MembershipTypes { get; set; }

        public DbSet<Customer> Customers { get; set; }
        //NEW ONES BELOW
        public DbSet<Location> Locations { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<VehicleLocation> VehicleLocations { get; set; }

        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add a unique index to the Reservation Number
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.ReservationNumber)
                .IsUnique();

            // Add a unique index to the Employee Email
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Add a unique index to the Driver License
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.DriverLicense)
                .IsUnique();

            // Prevent Cascade Delete from Employee to Reservation
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Reservations)
                .WithOne(r => r.Employee)
                .HasForeignKey(r => r.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent Cascade Delete from MembershipType to Customer
            modelBuilder.Entity<MembershipType>()
                .HasMany(m => m.Customers)
                .WithOne(c => c.MembershipType)
                .HasForeignKey(c => c.MembershipTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            //NEW ModelBuilder RULES ***************************************************
            modelBuilder.Entity<Location>()
                .HasIndex(l => l.Name)
                .IsUnique();

            modelBuilder.Entity<Location>()
                .HasMany(l => l.VehicleLocations) //Relates to the ICollection in the Vehicle clas
                .WithOne(vl => vl.Location)
                .HasForeignKey(vl => vl.LocationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.LicensePlate)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.VehicleLocations)
                .WithOne(vl => vl.Vehicle)
                .HasForeignKey(vl => vl.VehicleID)
                .OnDelete(DeleteBehavior.Cascade); //Dont prevent on Casacade delete

            modelBuilder.Entity<VehicleLocation>()
                .HasKey(t => new { t.VehicleID, t.LocationID }); //Creates the composite key

            modelBuilder.Entity<Rental>()
                .HasIndex(r => r.RentalNumber)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Rentals) //IColletion
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Rentals) //ICollection
                .WithOne(r => r.Employee)
                .HasForeignKey(r => r.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Location>()
                .HasMany(l => l.Reservations) //ICollection
                .WithOne(r => r.Location)
                .HasForeignKey(r => r.LocationID)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
