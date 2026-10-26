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
        }
    }
}
