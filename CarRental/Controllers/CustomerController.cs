
using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;

public class CustomerController : Controller
{
    private readonly CarRentalContext _context;

    public CustomerController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: CUSTOMER
    public async Task<IActionResult> Index(string? SearchString, int? MembershipTypeID, string? actionButton, string sortDirection = "asc", string sortField = "Customer")    
    {

        //List of sort options for the columns
        //Must matcht the column headings
        string[] sortOptions = new[] { "Customer", "MembershipType" };

        //Add the count
        ViewData["Filtering"] = "btn-outline-secondary";
        int numberFilters = 0; //Start the variable at zero
        var customers = _context.Customers
                            .Include(c => c.MembershipType)
                            .AsNoTracking();
                            

        //Suplies the dropdown 
        ViewData["MembershipTypeID"] = new SelectList(
            _context.MembershipTypes,
            "ID",
            "Name"
        );

        if (MembershipTypeID.HasValue)
        {
            customers = customers.Where(c => c.MembershipTypeID == MembershipTypeID);
            numberFilters++;
        }
        if (!String.IsNullOrEmpty(SearchString))
        {
            customers = customers.Where(c => c.LastName.ToUpper().Contains(SearchString.ToUpper()) || 
                                    c.FirstName.ToUpper().Contains(SearchString.ToUpper()) ||
                                    c.DriverLicense.ToUpper().Contains(SearchString.ToUpper()));
            numberFilters++;
        }

        //Give feedback on the state of the filters
        if (numberFilters != 0)
        {
            // Toggle the Open/ Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = " btn-danger";
            //Show how many filters have been applied
            ViewData["numberFilters"] = "(" + numberFilters.ToString()
                + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
            //Keep the Bootstrap collapse open
            @ViewData["ShowFilter"] = " show";
        }

        //Before we sort, see if we have called for a change of filtering or sorting
        if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
        {
            //page = 1;//Reset page to start

            if (sortOptions.Contains(actionButton))//Change of sort is requested
            {
                if (actionButton == sortField) //Reverse order on same field
                {
                    sortDirection = sortDirection == "asc" ? "desc" : "asc";
                }
                sortField = actionButton;//Sort by the button clicked
            }
        }
        if (sortField == "Customer")
        {
            if (sortDirection == "asc")
            {
                customers = customers
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName);
            }
            else
            {
                customers = customers
                    .OrderByDescending(c => c.LastName)
                    .ThenBy(c => c.FirstName);
            }
        }

        if (sortField == "MembershipType")
        {
            if (sortDirection == "asc")
            {
                customers = customers
                    .OrderBy(c => c.MembershipType.Name)
                    .ThenBy(c => c.LastName)
                    .ThenBy(c => c.FirstName);
            }
            else
            {
                customers = customers
                    .OrderByDescending(c => c.MembershipType.Name)
                    .ThenBy(c => c.LastName)
                    .ThenBy(c => c.FirstName);
            }
        }

        //Remeber the sort for the next request
        ViewData["sortField"] = sortField;
        ViewData["sortDirection"] = sortDirection;
        //return View(customers); 
        return View(await customers.ToListAsync()); //This makes that filtering happen first and then the customers are displayed, ListAsync makes it wait
    }

    // GET: CUSTOMER/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers
            .Include(c => c.MembershipType)
            .Include(c => c.CustomerReservations)
                .ThenInclude(r => r.Reservation)// This part is for Part 2b, step F
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // GET: CUSTOMER/Create
    public IActionResult Create()
    {
        PopulateDropDownLists();
        return View();
    }

    // POST: CUSTOMER/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Phone,Email,DriverLicense,DOB,MembershipTypeID")] Customer customer)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Customers.DriverLicense"))
            {
                ModelState.AddModelError("DriverLicense", "Unable to save changes. Remember, you cannot have duplicate Driver License numbers.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
        }

        PopulateDropDownLists(customer);
        return View(customer);
    }

    // GET: CUSTOMER/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers
            .Include(c => c.MembershipType)
            .FirstOrDefaultAsync(m => m.ID == id);

        if (customer == null)
        {
            return NotFound();
        }

        PopulateDropDownLists(customer);
        return View(customer);
    }

    // POST: CUSTOMER/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id)
    {
        //Go get the customer to update
        var customerToUpdate = await _context.Customers
            .Include(c => c.MembershipType)
            .FirstOrDefaultAsync(c => c.ID == id);

        //Check that you got it or exit with a not found error
        if (customerToUpdate == null)
        {
            return NotFound();
        }

        //Try updating it with the values posted
        if (await TryUpdateModelAsync<Customer>(customerToUpdate, "",
            c => c.FirstName, c => c.MiddleName, c => c.LastName, c => c.Phone,
            c => c.Email, c => c.DriverLicense, c => c.DOB, c => c.MembershipTypeID))
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customerToUpdate.ID))
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
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Customers.DriverLicense"))
                {
                    ModelState.AddModelError("DriverLicense", "Unable to save changes. Remember, you cannot have duplicate Driver License numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
        }
        PopulateDropDownLists(customerToUpdate);
        return View(customerToUpdate);
    }

    // GET: CUSTOMER/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers
            .Include(c => c.MembershipType)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // POST: CUSTOMER/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var customer = await _context.Customers
            .Include(c => c.MembershipType)
            .FirstOrDefaultAsync(m => m.ID == id);

        try
        {
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            //This if for the user friendly message in step 2g
            if (await _context.CustomerReservations
                .AnyAsync(cr => cr.CustomerID == id)) //Check if there are any exsisting reservations
            {
                ModelState.AddModelError("",
                    "Cannot delete this Customer because they have existing Reservations.");
            }
            else
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            
        }
        return View(customer);
    }

    // Populates the Membership Type dropdown and selects the customer's current membership when editing.
    private void PopulateDropDownLists(Customer? customer = null)
    {
        var mQuery = from m in _context.MembershipTypes
                     orderby m.Name
                     select m;

        ViewData["MembershipTypeID"] = new SelectList(mQuery, "ID", "Name", customer?.MembershipTypeID);
    }

    private bool CustomerExists(int? id)
    {
        return _context.Customers.Any(e => e.ID == id);
    }
}
