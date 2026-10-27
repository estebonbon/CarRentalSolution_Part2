using CarRental.Models;
using CarRental.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CarRental.Data
{
    public static class CarRentalInitializer
    {
        /// <summary>
        /// Prepares the Database and seeds data as required
        /// </summary>
        /// <param name="serviceProvider">DI Container</param>
        /// <param name="DeleteDatabase">Delete the database and start from scratch</param>
        /// <param name="UseMigrations">Use Migrations or EnsureCreated</param>
        /// <param name="SeedSampleData">Add optional sample data</param>
        public static void Initialize(IServiceProvider serviceProvider,
            bool DeleteDatabase = false, bool UseMigrations = true, bool SeedSampleData = true)
        {

            using (var context = new CarRentalContext(
                serviceProvider.GetRequiredService<DbContextOptions<CarRentalContext>>()))
            {
                //Refresh the database as per the parameter options
                #region Prepare the Database
                try
                {
                    //Note: .CanConnect() will return false if the database is not there!
                    if (DeleteDatabase || !context.Database.CanConnect())
                    {
                        if (!SqLiteDBUtility.ReallyEnsureDeleted(context)) //Delete the existing version
                        {
                            Debug.WriteLine("Could not clear the old version " +
                                "of the database out of the way.  You will need to exit " +
                                "Visual Studio and try to do it manually.");
                        }
                        if (UseMigrations)
                        {
                            context.Database.Migrate(); //Create the Database and apply all migrations
                        }
                        else
                        {
                            context.Database.EnsureCreated(); //Create and update the database as per the Model
                        }
                        //Here is a good place to create any additional database objects such as Triggers or Views
                        //----------------------------------------------------------------------------------------
                    }
                    else //The database is already created
                    {
                        if (UseMigrations)
                        {
                            context.Database.Migrate(); //Apply all migrations
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
                #endregion

                //Seed data needed for production and during development
                #region Seed Required Data
                try
                {
                    //Add some Membership Types
                    //Needed for production and during development since every Customer needs one.
                    //Discount must be between 0 and 0.5
                    if (!context.MembershipTypes.Any())
                    {
                        context.MembershipTypes.AddRange(
                         new MembershipType
                         {
                             Name = "Standard",
                             Discount = 0,
                             Description = "Pay-as-you-go rentals at regular daily rates. Basic roadside assistance included."
                         }, new MembershipType
                         {
                             Name = "Premium",
                             Discount = 0.10,
                             Description = "10% off every rental, free additional driver, and express pickup with no counter line."
                         }, new MembershipType
                         {
                             Name = "Corporate",
                             Discount = 0.15,
                             Description = "15% off business travel, monthly invoicing to the company account, and free one-way drop-offs."
                         }, new MembershipType
                         {
                             Name = "VIP",
                             Discount = 0.25,
                             Description = "25% off, guaranteed vehicle upgrade when available, priority pickup, and 24/7 dedicated support."
                         });
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
                #endregion

                //Seed meaningless data as sample data during development
                #region Seed Sample Data
                if (SeedSampleData)
                {
                    //To randomly generate data
                    Random random = new Random();

                    //Seed a few specific Employees, Customers and Reservations. We will add more with random values later,
                    //but it can be useful to know we will have some specific records in the sample data.
                    try
                    {
                        // Seed Employees first since we can't have Reservations without Employees.
                        // Note: Employee Email must be unique.
                        if (!context.Employees.Any())
                        {
                            context.Employees.AddRange(
                            new Employee
                            {
                                FirstName = "Gregory",
                                MiddleName = "A",
                                LastName = "House",
                                Phone = "9055551212",
                                Email = "ghouse@outlook.com",
                                HireDate = DateTime.Parse("2018-09-01"),
                                IsActive = true
                            },
                            new Employee
                            {
                                FirstName = "Doogie",
                                MiddleName = "R",
                                LastName = "Houser",
                                Phone = "9055551213",
                                Email = "dhouser@outlook.com",
                                HireDate = DateTime.Parse("2021-04-23"),
                                IsActive = true
                            },
                            new Employee
                            {
                                FirstName = "Charles",
                                LastName = "Xavier",
                                Phone = "9055551234",
                                Email = "cxavier@outlook.com",
                                HireDate = DateTime.Parse("2016-02-22"),
                                IsActive = true
                            },
                            new Employee
                            {
                                FirstName = "Tony",
                                MiddleName = "Edward",
                                LastName = "Stark",
                                Phone = "9055556789",
                                Email = "tstark@outlook.com",
                                HireDate = DateTime.Parse("2020-05-15"),
                                IsActive = true
                            },
                            new Employee
                            {
                                FirstName = "Bruce",
                                MiddleName = "Thomas",
                                LastName = "Wayne",
                                Phone = "9055557890",
                                Email = "bwayne@outlook.com",
                                HireDate = DateTime.Parse("2019-03-12"),
                                IsActive = true
                            }
                            );
                            context.SaveChanges();
                        }
                        // Seed Customers if there aren't any.
                        // Note: Driver License must be unique and no more than 15 characters.
                        // Customers must be at least 19 years old and DOB cannot be in the future.
                        if (!context.Customers.Any())
                        {
                            context.Customers.AddRange(
                            new Customer
                            {
                                FirstName = "Fred",
                                MiddleName = "Reginald",
                                LastName = "Flintstone",
                                DriverLicense = "F1231231234",
                                DOB = DateTime.Parse("1955-09-01"),
                                Phone = "9055551212",
                                Email = "fflintstone@outlook.com",
                                MembershipTypeID = context.MembershipTypes.FirstOrDefault(m => m.Name == "VIP").ID
                            },
                            new Customer
                            {
                                FirstName = "Wilma",
                                MiddleName = "Jane",
                                LastName = "Flintstone",
                                DriverLicense = "F1321321324",
                                DOB = DateTime.Parse("1964-04-23"),
                                Phone = "9055551212",
                                Email = "wflintstone@outlook.com",
                                MembershipTypeID = context.MembershipTypes.FirstOrDefault(m => m.Name == "Premium").ID
                            },
                            new Customer
                            {
                                FirstName = "Barney",
                                LastName = "Rubble",
                                DriverLicense = "R3213213214",
                                DOB = DateTime.Parse("1964-02-22"),
                                Phone = "9055551213",
                                Email = "brubble@outlook.com",
                                MembershipTypeID = context.MembershipTypes.FirstOrDefault(m => m.Name == "Standard").ID
                            },
                            new Customer
                            {
                                FirstName = "Jane",
                                MiddleName = "Samantha",
                                LastName = "Doe",
                                DriverLicense = "D4124124123",
                                DOB = DateTime.Parse("1990-06-15"),
                                Phone = "9055551234",
                                Email = "jdoe@outlook.com",
                                MembershipTypeID = context.MembershipTypes.FirstOrDefault(m => m.Name == "Corporate").ID
                            },
                            new Customer
                            {
                                FirstName = "Peter",
                                MiddleName = "James",
                                LastName = "Parker",
                                DriverLicense = "P5675675678",
                                DOB = DateTime.Parse("1995-08-10"),
                                Phone = "9055555678",
                                Email = "pparker@outlook.com",
                                MembershipTypeID = context.MembershipTypes.First(m => m.Name == "Standard").ID
                            }
                            );
                            context.SaveChanges();
                        }

                        //Seed Locations if there arent anyy
                        if (!context.Locations.Any())
                        {
                            context.Locations.AddRange(
                                new Location
                                {
                                    Name = "Louth Auto",
                                    Address = "59 Louth Street",
                                    City = "St. Catharines",
                                    PostalCode = "L2R 3M2",
                                    Phone = "9053218897"
                                },
                                new Location
                                {
                                    Name = "Niagara Car Rentals",
                                    Address = "125 Queen Street",
                                    City = "Niagara Falls",
                                    PostalCode = "L2E 2L1",
                                    Phone = "9055551234"
                                },
                                new Location
                                {
                                    Name = "Welland Auto Rentals",
                                    Address = "75 East Main Street",
                                    City = "Welland",
                                    PostalCode = "L3B 3W4",
                                    Phone = "9055552345"
                                },
                                new Location
                                {
                                    Name = "Hamilton Vehicle Rentals",
                                    Address = "210 King Street East",
                                    City = "Hamilton",
                                    PostalCode = "L8N 1B5",
                                    Phone = "9055553456"
                                },
                                new Location
                                {
                                    Name = "Thorold Car Rentals",
                                    Address = "45 Front Street South",
                                    City = "Thorold",
                                    PostalCode = "L2V 1X3",
                                    Phone = "9055554567"
                                }
                            );
                            context.SaveChanges(); //I keep forgeting this line
                        }

                        // Seed Reservations if there aren't any.
                        // Note: New Reservations cannot be created with a Pickup Date in the past.
                        // However, seed data can include past Reservations to represent completed rental history.
                        // The ID == 0 validation only applies the past-date rule when creating a new Reservation.
                        // Return Date must always be later than Pickup Date, and a Reservation cannot exceed 30 days.
                        if (!context.Reservations.Any())
                        {
                            context.Reservations.AddRange(
                            new Reservation
                            {
                                ReservationNumber = "CR-1001",
                                PickupDate = DateTime.Today.AddDays(2).AddHours(9),
                                ReturnDate = DateTime.Today.AddDays(5).AddHours(17),
                                Status = Status.Confirmed,
                                EmployeeID = context.Employees.FirstOrDefault(e => e.FirstName == "Gregory" && e.LastName == "House").ID,
                                LocationID = context.Locations.FirstOrDefault(l => l.Name == "Welland Auto Rentals").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1002",
                                PickupDate = DateTime.Today.AddDays(5).AddHours(13),
                                ReturnDate = DateTime.Today.AddDays(12).AddHours(13),
                                Status = Status.Pending,
                                EmployeeID = context.Employees.FirstOrDefault(e => e.FirstName == "Gregory" && e.LastName == "House").ID,
                                LocationID = context.Locations.FirstOrDefault(l => l.Name == "Niagara Car Rentals").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1003",
                                PickupDate = DateTime.Today.AddDays(-10).AddHours(10),
                                ReturnDate = DateTime.Today.AddDays(-7).AddHours(10),
                                Status = Status.Completed,
                                EmployeeID = context.Employees.FirstOrDefault(e => e.FirstName == "Doogie" && e.LastName == "Houser").ID,
                                LocationID = context.Locations.FirstOrDefault(l => l.Name == "Niagara Car Rentals").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1004",
                                PickupDate = DateTime.Today.AddDays(14).AddHours(8),
                                ReturnDate = DateTime.Today.AddDays(16).AddHours(8),
                                Status = Status.Cancelled,
                                EmployeeID = context.Employees.FirstOrDefault(e => e.FirstName == "Charles" && e.LastName == "Xavier").ID,
                                LocationID = context.Locations.FirstOrDefault(l => l.Name == "Louth Auto").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1005",
                                PickupDate = DateTime.Today.AddDays(3).AddHours(10),
                                ReturnDate = DateTime.Today.AddDays(7).AddHours(10),
                                Status = Status.Confirmed,
                                EmployeeID = context.Employees.First(e => e.LastName == "Stark").ID,
                                LocationID = context.Locations.First(l => l.Name == "Thorold Car Rentals").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1006",
                                PickupDate = DateTime.Today.AddDays(8).AddHours(9),
                                ReturnDate = DateTime.Today.AddDays(11).AddHours(17),
                                Status = Status.Pending,
                                EmployeeID = context.Employees.First(e => e.LastName == "Wayne").ID,
                                LocationID = context.Locations.First(l => l.Name == "Hamilton Vehicle Rentals").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1007",
                                PickupDate = DateTime.Today.AddDays(12).AddHours(11),
                                ReturnDate = DateTime.Today.AddDays(17).AddHours(11),
                                Status = Status.Confirmed,
                                EmployeeID = context.Employees.First(e => e.FirstName == "Gregory" && e.LastName == "House").ID,
                                LocationID = context.Locations.First(l => l.Name == "Welland Auto Rentals").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1008",
                                PickupDate = DateTime.Today.AddDays(18).AddHours(14),
                                ReturnDate = DateTime.Today.AddDays(20).AddHours(14),
                                Status = Status.Pending,
                                EmployeeID = context.Employees.First(e => e.FirstName == "Doogie" && e.LastName == "Houser").ID,
                                LocationID = context.Locations.First(l => l.Name == "Niagara Car Rentals").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1009",
                                PickupDate = DateTime.Today.AddDays(21).AddHours(8),
                                ReturnDate = DateTime.Today.AddDays(28).AddHours(8),
                                Status = Status.Confirmed,
                                EmployeeID = context.Employees.First(e => e.LastName == "Xavier").ID,
                                LocationID = context.Locations.First(l => l.Name == "Louth Auto").ID
                            },
                            new Reservation
                            {
                                ReservationNumber = "CR-1010",
                                PickupDate = DateTime.Today.AddDays(25).AddHours(12),
                                ReturnDate = DateTime.Today.AddDays(29).AddHours(12),
                                Status = Status.Pending,
                                EmployeeID = context.Employees.First(e => e.LastName == "Stark").ID,
                                LocationID = context.Locations.First(l => l.Name == "Thorold Car Rentals").ID
                            }
                            );


                            context.SaveChanges();
                        }

                        // Seed CustomerReservations 2B
                        if (!context.CustomerReservations.Any())
                        {
                            context.CustomerReservations.AddRange(
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Jane" && c.LastName == "Doe").ID,

                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1001").ID
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Wilma" && c.LastName == "Flintstone").ID, //Customer 2 Reservation

                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1002").ID,

                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Wilma" && c.LastName == "Flintstone").ID, //Customer 2 Reservations

                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1003").ID
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Fred" && c.LastName == "Flintstone").ID,

                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1001").ID  //One reservation with 2 customer
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Barney" && c.LastName == "Rubble").ID,
                                
                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1004").ID
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Jane" && c.LastName == "Doe").ID,
                                
                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1005").ID
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Fred" && c.LastName == "Flintstone").ID,
                                
                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1006").ID
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Wilma" && c.LastName == "Flintstone").ID,
                                
                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1007").ID
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Barney" && c.LastName == "Rubble").ID,
                                
                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1008").ID
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Jane" && c.LastName == "Doe").ID,
                                
                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1009").ID
                                },
                                new CustomerReservation
                                {
                                    CustomerID = context.Customers
                                        .First(c => c.FirstName == "Fred" && c.LastName == "Flintstone").ID,
                                
                                    ReservationID = context.Reservations
                                        .First(r => r.ReservationNumber == "CR-1010").ID
                                }
                            );
                            context.SaveChanges();
                        }

                        //Seed the Vechiles if there arent any
                        if (!context.Vehicles.Any())
                        {
                            context.Vehicles.AddRange(
                                new Vehicle
                                {
                                    Make = "Acura",
                                    Model = "ADX",
                                    Year = 2022,
                                    LicensePlate = "LN88216K",
                                    DailyRate = 213.00
                                },
                                new Vehicle
                                {
                                    Make = "Honda",
                                    Model = "Civic",
                                    Year = 2024,
                                    LicensePlate = "ABCD123",
                                    DailyRate = 75.50
                                },
                                new Vehicle
                                {
                                    Make = "Toyota",
                                    Model = "Corolla",
                                    Year = 2023,
                                    LicensePlate = "EFGH456",
                                    DailyRate = 85.99
                                },
                                new Vehicle
                                {
                                    Make = "BMW",
                                    Model = "330i",
                                    Year = 2025,
                                    LicensePlate = "JKLM789",
                                    DailyRate = 175.00
                                },
                                new Vehicle
                                {
                                    Make = "Hyundai",
                                    Model = "Elantra",
                                    Year = 2026,
                                    LicensePlate = "NPQR234",
                                    DailyRate = 95.75
                                }
                            );
                            context.SaveChanges();
                        }

                        //Seed Vechile Location if there arent any
                        if (!context.VehicleLocations.Any())
                        {
                            context.VehicleLocations.AddRange(
                                new VehicleLocation
                                {
                                    VehicleID = 1,
                                    LocationID = 2
                                },
                                new VehicleLocation
                                {
                                    VehicleID = 5,
                                    LocationID = 4
                                },
                                new VehicleLocation
                                {
                                    VehicleID = 2,
                                    LocationID = 2
                                },
                                new VehicleLocation
                                {
                                    VehicleID = 3,
                                    LocationID = 1,
                                },
                                new VehicleLocation
                                {
                                    VehicleID = 4,
                                    LocationID = 5
                                },
                                new VehicleLocation
                                {
                                    VehicleID = 4,
                                    LocationID = 1
                                },
                                new VehicleLocation
                                {
                                    VehicleID = 1,
                                    LocationID = 3
                                }

                            );
                            context.SaveChanges();
                        }

                        //Seed Any Rentals if there arent any
                        if (!context.Rentals.Any())
                        {
                            context.Rentals.AddRange(
                                new Rental
                                {
                                    RentalNumber = "KK29RVS87L9",
                                    PickupDate = DateTime.Parse("2026-10-18"),
                                    ReturnDate = null,
                                    CustomerID = context.Customers.First(c =>
                                        c.FirstName == "Barney" && c.LastName == "Rubble").ID,
                                    EmployeeID = null
                                },
                                new Rental
                                {
                                    RentalNumber = "FR2026101901",
                                    PickupDate = DateTime.Parse("2026-10-19"),
                                    ReturnDate = DateTime.Parse("2026-10-22"),
                                    CustomerID = context.Customers.First(c =>
                                        c.FirstName == "Fred" && c.LastName == "Flintstone").ID,
                                    EmployeeID = context.Employees.First(e =>
                                        e.FirstName == "Gregory" && e.LastName == "House").ID
                                },
                                new Rental
                                {
                                    RentalNumber = "WR2026102001",
                                    PickupDate = DateTime.Parse("2026-10-20"),
                                    ReturnDate = null,
                                    CustomerID = context.Customers.First(c =>
                                        c.FirstName == "Wilma" && c.LastName == "Flintstone").ID,
                                    EmployeeID = context.Employees.First(e =>
                                        e.FirstName == "Doogie" && e.LastName == "Houser").ID
                                },
                                new Rental
                                {
                                    RentalNumber = "JD2026102101",
                                    PickupDate = DateTime.Parse("2026-10-21"),
                                    ReturnDate = DateTime.Parse("2026-10-24"),
                                    CustomerID = context.Customers.First(c =>
                                        c.FirstName == "Jane" && c.LastName == "Doe").ID,
                                    EmployeeID = context.Employees.First(e =>
                                        e.FirstName == "Charles" && e.LastName == "Xavier").ID
                                }

                            );
                        }
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }

                    //Add more data using random values
                    try
                    {
                        //Add more Employees
                        if (context.Employees.Count() < 4)//Don't add a second time
                        {
                            string[] firstNames = new string[] { "Woodstock", "Violet", "Charlie", "Lucy", "Linus", "Franklin", "Marcie", "Schroeder" };
                            string[] lastNames = new string[] { "Hightower", "Broomspun", "Jones" };

                            //Loop through names and add more
                            foreach (string lastName in lastNames)
                            {
                                foreach (string firstname in firstNames)
                                {
                                    //Construct some details
                                    Employee a = new Employee()
                                    {
                                        FirstName = firstname,
                                        LastName = lastName,
                                        //Take second character of the last name and make it the middle name
                                        MiddleName = lastName[1].ToString().ToUpper(),
                                        Phone = random.Next(200, 999).ToString() + random.Next(2000000, 9999999).ToString(),
                                        //Email must be unique.  Every first/last name combination is different.
                                        Email = (firstname + "." + lastName + "@outlook.com").ToLower(),
                                        HireDate = DateTime.Today.AddDays(-random.Next(30, 5000)),
                                        IsActive = true
                                    };
                                    context.Employees.Add(a);
                                }
                            }
                            context.SaveChanges();
                        }

                        //So we can gererate random data, create collections of the primary keys
                        int[] employeeIDs = context.Employees.Select(a => a.ID).ToArray();
                        int employeeIDCount = employeeIDs.Length;
                        int[] membershipTypeIDs = context.MembershipTypes.Select(a => a.ID).ToArray();
                        int membershipTypeIDCount = membershipTypeIDs.Length;

                        //Add more Customers.  Now it gets more interesting because we have Foreign Keys to worry about
                        //and more complicated property values to generate
                        if (context.Customers.Count() < 5)
                        {
                            string[] firstNames = new string[] { "Lyric", "Antoinette", "Kendal", "Vivian", "Ruth", "Jamison", "Emilia", "Natalee", "Yadiel", "Jakayla", "Lukas", "Moses", "Kyler", "Karla", "Chanel", "Tyler", "Camilla", "Quintin", "Braden", "Clarence" };
                            string[] lastNames = new string[] { "Watts", "Randall", "Arias", "Weber", "Stone", "Carlson", "Robles", "Frederick", "Parker", "Morris", "Soto", "Bruce", "Orozco", "Boyer", "Burns", "Cobb", "Blankenship", "Houston", "Estes", "Atkins", "Miranda", "Zuniga", "Ward", "Mayo", "Costa", "Reeves", "Anthony", "Cook", "Krueger", "Crane", "Watts", "Little", "Henderson", "Bishop" };
                            int firstNameCount = firstNames.Length;

                            // Birthdate for randomly produced Customers
                            // We will subtract a random number of days from the date they turned 19
                            // so every Customer is at least 19 years old.
                            DateTime startDOB = DateTime.Today.AddYears(-19);

                            foreach (string lastName in lastNames)
                            {
                                //Choose a random HashSet of 5 (Unique) first names
                                HashSet<string> selectedFirstNames = new HashSet<string>();
                                while (selectedFirstNames.Count() < 5)
                                {
                                    selectedFirstNames.Add(firstNames[random.Next(firstNameCount)]);
                                }

                                foreach (string firstName in selectedFirstNames)
                                {
                                    //Construct some Customer details
                                    Customer customer = new Customer()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = lastName[1].ToString().ToUpper(),
                                        //First letter of the last name followed by 10 digits
                                        DriverLicense = lastName[0].ToString() + random.Next(2, 9).ToString() + random.Next(213214131, 989898989).ToString(),
                                        Email = (firstName.Substring(0, 2) + lastName + random.Next(11, 111).ToString() + "@outlook.com").ToLower(),
                                        Phone = random.Next(200, 999).ToString() + random.Next(2000000, 9999999).ToString(),
                                        DOB = startDOB.AddDays(-random.Next(60, 20000)),
                                        MembershipTypeID = membershipTypeIDs[random.Next(membershipTypeIDCount)]
                                    };
                                    context.Customers.Add(customer);
                                    try
                                    {
                                        //Could be a duplicate Driver License
                                        context.SaveChanges();
                                    }
                                    catch (Exception)
                                    {
                                        //Failed so remove it and go on to the next.
                                        //If you don't remove it from the context it will keep trying to save it each time you
                                        //call .SaveChanges() the the save process will stop and prevent any other records in the que from getting saved.
                                        context.Customers.Remove(customer);
                                    }
                                }
                            }
                        }

                        //Add more Reservations
                        if (context.Reservations.Count() < 5)
                        {
                            int counter = 1005; //Used to give each Reservation its own number

                            //Statuses we can use for future Reservations
                            //(Completed is skipped because it would need a Pickup Date in the past)
                            Status[] statuses = new Status[] { Status.Pending, Status.Confirmed, Status.Cancelled };

                            for (int i = 1; i <= 50; i++)
                            {
                                // Pickup 2 to 90 days from today at a random hour from 8:00 AM to 6:00 PM
                                DateTime pickup = DateTime.Today.AddDays(random.Next(2, 91)).AddHours(random.Next(8, 19));

                                //Construct some Reservation details
                                Reservation reservation = new Reservation()
                                {
                                    ReservationNumber = "CR-" + counter.ToString(),
                                    PickupDate = pickup,
                                    // Return 1 to 29 days after pickup so it is never more than 30 days
                                    ReturnDate = pickup.AddDays(random.Next(1, 30)),
                                    Status = statuses[random.Next(statuses.Length)],
                                    EmployeeID = employeeIDs[random.Next(employeeIDCount)]
                                };
                                counter++;
                                context.Reservations.Add(reservation);
                                try
                                {
                                    //Could be a duplicate Reservation Number
                                    context.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    //Failed so remove it and go on to the next.
                                    context.Reservations.Remove(reservation);
                                }
                            }
                            //Since we didn't guarantee that evey Employee had at least one Reservation assigned, let's remove Employees
                            //without any Reservations.  We could do this other ways, but it gives a chance to show how to execute raw SQL through our DbContext.
                            string cmd = "DELETE FROM Employees WHERE NOT EXISTS(SELECT 1 FROM Reservations " +
                                "WHERE Employees.ID = Reservations.EmployeeID)";
                            context.Database.ExecuteSqlRaw(cmd);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetBaseException().Message);
                    }
                }

                #endregion

            }
        }
    }
}
