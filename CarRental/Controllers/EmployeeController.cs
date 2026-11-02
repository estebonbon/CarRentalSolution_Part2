
using CarRental.CustomControllers;
using CarRental.Data;
using CarRental.Models;
using CarRental.Utilities;
using CarRental.Utilities.CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class EmployeeController : ElephantController
{
    private readonly CarRentalContext _context;

    public EmployeeController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: EMPLOYEE
    public async Task<IActionResult> Index(string? SearchString, bool? Active, string actionButton, int? page, int? pageSizeID, string sortField = "Name", string sortDirection = "asc")    
    {
        //Create the sort options
        string[] sortOptions = new[] { "Name", "Hire Date" };

        //Add the Count
        ViewData["Filtering"] = "btn-outline-secondary";
        int numberFilters = 0; //Start the variable at zero

        var employees = _context.Employees
                        .AsNoTracking();
            //.ToListAsync();

        //Create the dropdown Revisit this LATER
        ViewData["Active"] = new SelectList(new[]
            {
                new { Value = "true", Text = "Active" },
                new { Value = "false", Text = "Inactive" }
            }, "Value", "Text");

        //These provide functionality for the drop downs
        if (Active.HasValue)
        {
            employees = employees.Where(e => e.IsActive == Active.Value);
            numberFilters++;
        }

        //Search Feature
        if (!String.IsNullOrEmpty(SearchString))
        {
            employees = employees.Where(e => e.FirstName.ToUpper().Contains(SearchString.ToUpper())
                || e.LastName.ToUpper().Contains(SearchString.ToUpper()));
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
            page = 1;//Reset page to start

            if (sortOptions.Contains(actionButton))//Change of sort is requested
            {
                if (actionButton == sortField) //Reverse order on same field
                {
                    sortDirection = sortDirection == "asc" ? "desc" : "asc";
                }
                sortField = actionButton;//Sort by the button clicked
            }
        }
        if (sortField == "Name")
        {
            if (sortDirection == "asc")
            {
                employees = employees
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName);
            }
            else
            {
                employees = employees
                    .OrderByDescending(e => e.LastName)
                    .ThenBy(e => e.FirstName);

            }
        }

        if (sortField == "Hire Date")
        {
            if (sortDirection == "asc")
            {
                employees = employees.OrderBy(e => e.HireDate);
            }
            else
            {
                employees = employees.OrderByDescending(e => e.HireDate);
            }
        }

        //Remeber the sort for the next request
        ViewData["sortField"] = sortField;
        ViewData["sortDirection"] = sortDirection;

        //Handle Paging
        int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
        ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

        var pagedData = await PaginatedList<Employee>.CreateAsync(employees.AsNoTracking(), page ?? 1, pageSize); //Use 1 by defualt,

        //return View(await employees.ToListAsync()); old version before pagedData query
        return View(pagedData);
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
                RedirectToAction("Details", new { employee.ID });
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
                RedirectToAction("Details", new { employeeToUpdate.ID });
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
            var returnURL = ViewData["returnURL"]?.ToString();
            if (string.IsNullOrEmpty(returnURL))
            {
                return RedirectToAction(nameof(Index)); //Incase the ELephant forgets and feel asleep
            }

            return Redirect(returnURL);
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
