
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Data;

public class MembershipTypeController : Controller
{
    private readonly CarRentalContext _context;

    public MembershipTypeController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: MEMBERSHIPTYPE
    public async Task<IActionResult> Index()    
    {
        var membershipTypes = await _context.MembershipTypes
                        .AsNoTracking()
                        .ToListAsync();
        return View(membershipTypes);
    }

    // GET: MEMBERSHIPTYPE/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershipType = await _context.MembershipTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

        if (membershipType == null)
        {
            return NotFound();
        }

        return View(membershipType);
    }

    // GET: MEMBERSHIPTYPE/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MEMBERSHIPTYPE/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Name,Discount,Description")] MembershipType membershipType)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        }

        return View(membershipType);
    }

    // GET: MEMBERSHIPTYPE/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershipType = await _context.MembershipTypes.FindAsync(id);
        if (membershipType == null)
        {
            return NotFound();
        }
        return View(membershipType);
    }

    // POST: MEMBERSHIPTYPE/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id)
    {
        //Go get the Membership Type to update
        var membershipTypeToUpdate = await _context.MembershipTypes.FirstOrDefaultAsync(m => m.ID == id);

        //Check that you got it or exit with a not found error
        if (membershipTypeToUpdate == null)
        {
            return NotFound();
        }

        //Try updating it with the values posted
        if (await TryUpdateModelAsync<MembershipType>(membershipTypeToUpdate, "",
                m => m.Name, m => m.Discount, m => m.Description))
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MembershipTypeExists(membershipTypeToUpdate.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
        }
        return View(membershipTypeToUpdate);
    }

    // GET: MEMBERSHIPTYPE/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershipType = await _context.MembershipTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

        if (membershipType == null)
        {
            return NotFound();
        }

        return View(membershipType);
    }

    // POST: MEMBERSHIPTYPE/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var membershipType = await _context.MembershipTypes
           .FirstOrDefaultAsync(m => m.ID == id);

        try
        {
            if (membershipType != null)
            {
                _context.MembershipTypes.Remove(membershipType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
            {
                ModelState.AddModelError("", "Unable to Delete Membership Type. Remember, you cannot delete a " +
                    "Membership Type that has customers assigned.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem" +
                    " persists see your system administrator.");
            }
        }
        return View(membershipType);

    }

    private bool MembershipTypeExists(int? id)
    {
        return _context.MembershipTypes.Any(e => e.ID == id);
    }
}
