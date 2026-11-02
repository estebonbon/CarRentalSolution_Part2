using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class CarRentalContext : DbContext
    {

        //Give Access to IHttpContextAccessor for the Audit Data with IAuditable

        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the username
        public string UserName
        {
            get; private set;
        }

        public CarRentalContext(DbContextOptions<CarRentalContext> options, IHttpContextAccessor httpContextAccessor)
                    : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
                UserName ??= "Unknown"; //If no one is authenticated, set the username to "Unknown"
            }
            else
            {
                //No HttpContext so seeding data
                UserName = "Seed Data"; //Records at the beginning of the app cuased by the seed Data, will have a username called Seed Data. So the user knows how it was created.
            }
        }

        public CarRentalContext(DbContextOptions<CarRentalContext> options)
            : base(options)
        {
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                _httpContextAccessor = null!; //The "!" tells the program not to worry about the nullability of this variable, because we know it will be null in this case
                UserName = "Seed Data";
            }
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

        public DbSet<CustomerReservation> CustomerReservations { get; set; } //2B

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

            //ModelBuilder for Part 2B
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.CustomerReservations) //ICollection
                .WithOne(cr => cr.Customer)
                .HasForeignKey(cr => cr.CustomerID)
                .OnDelete(DeleteBehavior.Restrict); //Wont allow Customers to be deleted who have reservations

            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.CustomerReservations) //ICollection
                .WithOne(cr => cr.Reservation)
                .HasForeignKey(cr => cr.ReservationID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerReservation>()
                .HasKey(t => new { t.CustomerID, t.ReservationID }); //This creates the composite key

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);

        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        }

        //The custom method that will be called from the SaveChanges methods to set the audit data
        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }
}
