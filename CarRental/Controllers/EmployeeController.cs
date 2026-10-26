
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Data;

public class EmployeeController : Controller
{
    private readonly CarRentalContext _context;

    public EmployeeController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: EMPLOYEE
    public async Task<IActionResult> Index()    
    {
        var employees = await _context.Employees
                        .AsNoTracking()
                        .ToListAsync();

        return View(employees);
    }

    // GET: EMPLOYEE/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // GET: EMPLOYEE/Create
    public IActionResult Create()
    {
        Employee employee = new Employee();
        employee.HireDate = DateTime.Today;
        return View(employee);
    }

    // POST: EMPLOYEE/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Phone,Email,HireDate,IsActive")] Employee employee)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Employees.Email"))
            {
                ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate Email addresses.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
        }

        return View(employee);
    }

    // GET: EMPLOYEE/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: EMPLOYEE/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id)
    {
        //Go get the Employee to update
        var employeeToUpdate = await _context.Employees.FirstOrDefaultAsync(e => e.ID == id);

        //Check that you got it or exit with a not found error
        if (employeeToUpdate == null)
        {
            return NotFound();
        }

        //Try updating it with the values posted
        if (await TryUpdateModelAsync<Employee>(employeeToUpdate, "",
                e => e.FirstName, e => e.MiddleName, e => e.LastName, e => e.Phone,
                e => e.Email, e => e.HireDate, e => e.IsActive))
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employeeToUpdate.ID))
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
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Employees.Email"))
                {
                    ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate Email addresses.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
        }
        return View(employeeToUpdate);
    }

    // GET: EMPLOYEE/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: EMPLOYEE/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(m => m.ID == id);

        try
        {
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
            {
                ModelState.AddModelError("", "Unable to Delete Employee. Remember, you cannot delete an " +
                    "Employee that has reservations assigned.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem" +
                    " persists see your system administrator.");
            }
        }
        return View(employee);
    }

    private bool EmployeeExists(int? id)
    {
        return _context.Employees.Any(e => e.ID == id);
    }
}
