
using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            .AsNoTracking()
            .ToListAsync();

        return View(reservations);
    }
    // GET: RESERVATION/Create
    public IActionResult Create()
    {
        PopulateDropDownLists();
        return View();
    }

    // POST: RESERVATION/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,ReservationNumber,PickupDate,ReturnDate,Status,EmployeeID,LocationID")] Reservation reservation)
    {
        try
        {
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
            .FirstOrDefaultAsync(m => m.ID == id);

        if (reservation == null)
        {
            return NotFound();
        }

        PopulateDropDownLists(reservation);
        return View(reservation);
    }

    // POST: RESERVATION/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id)
    {
        //Go get the reservation to update
        var reservationToUpdate = await _context.Reservations
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(m => m.ID == id);

        //Check that you got it or exit with a not found error
        if (reservationToUpdate == null)
        {
            return NotFound();
        }

        //Try updating it with the values posted
        if (await TryUpdateModelAsync<Reservation>(reservationToUpdate, "",
            r => r.ReservationNumber, r => r.PickupDate, r => r.ReturnDate,
            r => r.Status, r => r.EmployeeID, r => r.LocationID)) //Sister Step is asp-for="LocationID"
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
}
