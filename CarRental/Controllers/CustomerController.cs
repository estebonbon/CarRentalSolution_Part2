
using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class CustomerController : Controller
{
    private readonly CarRentalContext _context;

    public CustomerController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: CUSTOMER
    public async Task<IActionResult> Index()    
    {
        var customers = await _context.Customers
                            .Include(c => c.MembershipType)
                            .AsNoTracking()
                            .ToListAsync();

        return View(customers);
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
            //Note: there is really no reason a delete should fail if you can "talk" to the database.
            ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
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
