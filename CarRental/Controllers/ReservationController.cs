using CarRental.Data;
using CarRental.Models;
using CarRental.ViewModels; //Part 2b
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.AccessControl;

public class ReservationController : Controller
{
    private readonly CarRentalContext _context;

    public ReservationController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: RESERVATION
    public async Task<IActionResult> Index()    
    {
        var reservations = await _context.Reservations
            .Include(r => r.Employee)
            .Include(r => r.Location) //New
            .Include(r => r.CustomerReservations)
                .ThenInclude(cr => cr.Customer) //Part 2B new
            .AsNoTracking()
            .ToListAsync();

        return View(reservations);
    }
    // GET: RESERVATION/Create
    public async Task<IActionResult> Create()
    {
        Reservation reservation = new Reservation(); //This is the reservation that will get passed into the create list
        
        await MakeCustomerListAsync(reservation); // kNw the list of Customers can be created
        PopulateDropDownLists();
        return View(reservation);
    }

    // POST: RESERVATION/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,ReservationNumber,PickupDate,ReturnDate,Status,EmployeeID,LocationID")] Reservation reservation, string[] selectedOptions)
    {
        try
        {
            if (selectedOptions == null || selectedOptions.Length == 0)
            {
                ModelState.AddModelError("",
                    "You must select at least one Customer.");
            }
            else
            {
                await UpdateCustomerListAsync(selectedOptions, reservation);
            }



            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Reservations.ReservationNumber"))
            {
                ModelState.AddModelError("ReservationNumber", "Unable to save changes. Remember, you cannot have duplicate Reservation numbers.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
        }

        await MakeCustomerListAsync(reservation);
        PopulateDropDownLists(reservation);
        return View(reservation);
    }

    // GET: RESERVATION/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Employee) //This is for a drop down
            .Include(r => r.Location) //Also for a drop down
            .Include(r => r.CustomerReservations)
                .ThenInclude(c => c.Customer)//This will be used for the SelectBoxes
            .FirstOrDefaultAsync(m => m.ID == id);

        if (reservation == null)
        {
            return NotFound();
        }
        await MakeCustomerListAsync(reservation);
        PopulateDropDownLists(reservation);
        return View(reservation);
    }

    // POST: RESERVATION/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string[] selectedOptions)
    {
        //Go get the reservation to update
        var reservationToUpdate = await _context.Reservations
            .Include(r => r.Employee)
            .Include(r => r.CustomerReservations)
                .ThenInclude(c => c.Customer)//This will be used for the SelectBoxes
            .FirstOrDefaultAsync(m => m.ID == id);

        //Check that you got it or exit with a not found error
        if (reservationToUpdate == null)
        {
            return NotFound();
        }

        await UpdateCustomerListAsync(selectedOptions, reservationToUpdate);

        //Try updating it with the values posted
        if (await TryUpdateModelAsync<Reservation>(reservationToUpdate, "",
            r => r.ReservationNumber, r => r.PickupDate, r => r.ReturnDate,
            r => r.Status, r => r.EmployeeID, r => r.LocationID)) //Sister Step is asp-for="LocationID"
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { reservationToUpdate.ID  });
                //return RedirectToAction(nameof(Index));
            }
            catch (RetryLimitExceededException /* dex */)//This is a Transaction in the Database!
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. " +
                    "Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservationToUpdate.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Reservations.ReservationNumber"))
                {
                    ModelState.AddModelError("ReservationNumber", "Unable to save changes. Remember, you cannot have duplicate Reservation numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
        }

        //Redirect the user incase there was a validation error
        await MakeCustomerListAsync(reservationToUpdate);
        PopulateDropDownLists(reservationToUpdate);
        return View(reservationToUpdate);
    }

    // GET: RESERVATION/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Employee)
            .Include(r => r.Location)
            .Include(r => r.CustomerReservations)
                .ThenInclude(cr => cr.Customer) //Part 2B NEW
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    // GET: RESERVATION/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Employee)
            .Include(r => r.Location)
            .Include(r => r.CustomerReservations)
                .ThenInclude(cr => cr.Customer) //Part 2b
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    // POST: RESERVATION/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var reservation = await _context.Reservations
             .Include(r => r.Employee)
             .FirstOrDefaultAsync(m => m.ID == id);

        try
        {
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            //Note: there is really no reason a delete should fail if you can "talk" to the database.
            ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
        }
        return View(reservation);
    }

    // Populates the Employee dropdown and selects the reservation's current Employee when editing.
    private void PopulateDropDownLists(Reservation? reservation = null)
    {
        //Exsisting Code
        var eQuery = from e in _context.Employees
                     orderby e.LastName, e.FirstName
                     select e;

        ViewData["EmployeeID"] = new SelectList(eQuery, "ID", "Summary", reservation?.EmployeeID);

        //Theses are the steps used to create a query so that we can create a new viewdata
        //Viewdata is the box sent to the view from the controller, ViewData["Label to identify the box"]
        //new SelectList Creates a list of options for the drop Down
        //new SelectList (locations retrieved, value of the location, the text displayed for that location, which location should have already been selected")
        //Location Drop Down

        var lQuery = from l in _context.Locations       
                     orderby l.Name
                     select l;

        ViewData["LocationID"] = new SelectList(lQuery, "ID", "Name", reservation?.EmployeeID);
    }

    private bool ReservationExists(int? id)
    {
        return _context.Reservations.Any(e => e.ID == id);
    }


    //SECTION FOR CREATING CUSTOMER LISTS
    private async Task MakeCustomerListAsync(Reservation reservation)
    {
        var allCustomers = await _context.Customers  //Grab all exsisting customers put them in this variable
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();

        var assignedCustomer = new HashSet<int>(
            reservation.CustomerReservations.Select(cr => cr.CustomerID)) ?? Enumerable.Empty<int>(); //?? the double ? means here is a backup incase it is null

        //Makes two seperate lists we are going to compare
        var selected = new List<ListOptionVM>();
        var available = new List<ListOptionVM>();

        foreach (var customer in allCustomers)
        {
            var option = new ListOptionVM() //One instance of the object
            {
                ID = customer.ID,
                DisplayText = customer.FirstName + " " + customer.LastName
            };

            if(assignedCustomer.Contains(customer.ID))
            {
                selected.Add(option); //This is where that option object with ID and Name get sent to the list named "selected"
            }
            else
            {
                available.Add(option); //This is where any options not selected get sent to the other list
            }
        }

        //Send both Lists to the ViewData Dictionary
        ViewData["selectCust"] = new MultiSelectList(selected, "ID", "DisplayText");
        ViewData["availCust"] = new MultiSelectList(available, "ID", "DisplayText");
    }

    private async Task UpdateCustomerListAsync(string[] selectedOptions, Reservation reservationToUpdate)
    {
        if(selectedOptions == null || selectedOptions.Length == 0)
        {
            //reservationToUpdate.CustomerReservations = new List<CustomerReservation>(); // Create an empty list with no customers and then stops the method Every Reservation must have at least one customer
            return;
        }

        var selectedIds = selectedOptions.Select(int.Parse).ToHashSet(); //Converts string into intergers
        var currentIds = reservationToUpdate.CustomerReservations.Select(cr => cr.CustomerID).ToHashSet(); // Goes through the current Reservation Relationship to retrieve the CustomerID

        var allCustomers = await _context.Customers.ToListAsync(); //Grabs all the customer objects, loops through ever possinle pssibility

        foreach(var customer in allCustomers)
        {
            var customerID = customer.ID; //Saves it to the variable
            var isSelected = selectedIds.Contains(customerID); //This will store a bool value
            var isAssigned = currentIds.Contains(customerID); //This again checks whether the current reservation object has this customer linked to it

            if(isSelected &&  !isAssigned)
            {
                reservationToUpdate.CustomerReservations.Add(new CustomerReservation
                {
                    CustomerID = customerID,
                    ReservationID = reservationToUpdate.ID // The reservationID is the ID of the current ReservationObject we are trying to update
                });
            }

            if (!isSelected && isAssigned)
            {
                var toRemove = reservationToUpdate.CustomerReservations.FirstOrDefault(cr => cr.CustomerID == customer.ID); //Go look in the ICollection find the matching IDs and store that relatioship in ToRemove

                if (toRemove != null)
                {
                    _context.CustomerReservations.Remove(toRemove); //This removes it from that database
                }
            }
        }

    }
}
