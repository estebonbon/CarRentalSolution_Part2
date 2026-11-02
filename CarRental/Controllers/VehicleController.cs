
using CarRental.CustomControllers;
using CarRental.Data;
using CarRental.Models;
using CarRental.Utilities;
using CarRental.Utilities.CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

public class VehicleController : ElephantController
{
    private readonly CarRentalContext _context;

    public VehicleController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: VEHICLES
    public async Task<IActionResult> Index(string? SearchString, string? Make, string? Model, int? Year, string? actionButton, int? page, int? pageSizeID, string sortField = "Make", string sortDirection ="asc")    
    {
        

        //List of sort options for the columns
        //Must match the headings
        string[] sortOptions = new[] { "Make", "Model", "Year", "Daily Rate" };

        //Add the Count
        ViewData["Filtering"] = "btn-outline-secondary";
        int numberFilters = 0; //Start the variable at zero

        var vehicles = _context.Vehicles
            .Include(v => v.VehicleLocations)
            .ThenInclude(vl => vl.Location)
            .AsNoTracking();
            //.ToListAsync(); //Remove soon

        //Suply the DropDowns
        ViewData["Make"] = new SelectList(
            await _context.Vehicles
                .Select(v => v.Make)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync());

        ViewData["Model"] = new SelectList(
            await _context.Vehicles
                .Select(v => v.Model)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync());

        ViewData["Year"] = new SelectList(
            await _context.Vehicles
                .Select(v => v.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync());

        //These provide functionality for the drop downs
        if (!string.IsNullOrEmpty(Make))
        {
            vehicles = vehicles.Where(v => v.Make == Make);
            numberFilters++;
    }
        if (!string.IsNullOrEmpty(Model))
        {
            vehicles = vehicles.Where(v => v.Model == Model);
            numberFilters++;
        }
        if (Year.HasValue)
        {
            vehicles = vehicles.Where(v => v.Year == Year);
            numberFilters++;
        }
        //Search Feature
        if (!String.IsNullOrEmpty(SearchString))
        {
            vehicles = vehicles.Where(v => v.LicensePlate.ToUpper().Contains(SearchString.ToUpper()));
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
        //Before we sort, see if we have called for a change of filtering or sorting THE CODE WONT WORK WITHOUT THIS
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

        if (sortField == "Make")
        {
            if (sortDirection == "asc")
            {
                vehicles = vehicles
                    .OrderBy(v => v.Make)
                    .ThenBy(v => v.Model);
            }
            else
            {
                vehicles = vehicles
                    .OrderByDescending(v => v.Make)
                    .ThenByDescending(v => v.Model);
            }
        }
        if (sortField == "Model")
        {
            if (sortDirection == "asc")
        {
                vehicles = vehicles
                    .OrderBy(v => v.Model)
                    .ThenBy(v => v.Make);
            }
            else
            {
                vehicles = vehicles
                    .OrderByDescending(v => v.Model)
                    .ThenByDescending(v => v.Make);
            }
        }
        if (sortField == "Year")
        {
            if (sortDirection == "asc")
            {
                vehicles = vehicles
                    .OrderBy(v => v.Year);
            }
            else
                {
                vehicles = vehicles
                    .OrderByDescending(v => v.Year);
            }
        }
        if (sortField == "Daily Rate")
        {
            if (sortDirection == "asc")
            {
                vehicles = vehicles
                    .OrderBy(v => v.DailyRate);
}
            else
    {
                vehicles = vehicles
                    .OrderByDescending(v => v.DailyRate);
            }
        }

        //Remeber the sort for the next request
        ViewData["sortField"] = sortField;
        ViewData["sortDirection"] = sortDirection;

        //Handle Paging
        int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
        ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

        var pagedData = await PaginatedList<Vehicle>.CreateAsync(vehicles.AsNoTracking(), page ?? 1, pageSize); //Use 1 by defualt,

        return View(pagedData);
    }

    // GET: VEHICLES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleLocations)
            .ThenInclude(vl => vl.Location)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.ID == id);
        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    // GET: VEHICLES/Create
    public IActionResult Create()
    {
        PopulateAssignedLocationData(); //Method we created at the top of the page is called, and retrieves values for locations
        return View();
    }

    // POST: VEHICLES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Make,Model,Year,LicensePlate,DailyRate")] Vehicle vehicle, string[] selectedLocations)
    {
        if (ModelState.IsValid) //Sister Step asp-validation-summary="ModelOnly" class="text-danger" Vehicle.Create.cshtml
        {
            try
            {
                foreach (var locationID in selectedLocations)  //Sister steps The ICollection inside of Vehicle will be collecting the IDs of the selectedd locations based on what the user selects
                {
                    vehicle.VehicleLocations.Add(new VehicleLocation
                    {
                        LocationID = int.Parse(locationID)
                    });
                }
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { vehicle.ID});
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save the vehicle. Please Try Again");
            }
        }
        PopulateAssignedLocationData(); //Incase the validation fails
        return View(vehicle);
    }


    // GET: VEHICLES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleLocations) //Exsisting locations
            .FirstOrDefaultAsync(v => v.ID == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        PopulateAssignedLocationData(); //
        return View(vehicle); //opens the edit page
    }

    // POST: VEHICLES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("ID,Make,Model,Year,LicensePlate,DailyRate")] Vehicle vehicle, string[] selectedLocations)
    {
        if (id != vehicle.ID)
        {
            return NotFound();
        }

        var vehicleToUpdate = await _context.Vehicles
            .Include(v => v.VehicleLocations)
            .FirstOrDefaultAsync(v => v.ID == id);

        if (vehicleToUpdate == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (await TryUpdateModelAsync<Vehicle>(
                    vehicleToUpdate,
                    "", v => v.Make, v => v.Model, v => v.Year, v => v.LicensePlate, v => v.DailyRate))
                {
                    await UpdateVehicleLocationsAsync(selectedLocations, vehicleToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { vehicleToUpdate.ID }); //This line got changed
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(vehicle.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        PopulateAssignedLocationData();
        return View(vehicle);
    }

    // GET: VEHICLES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleLocations)
            .ThenInclude(vl => vl.Location)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.ID == id);


        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    // POST: VEHICLES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
        }

        await _context.SaveChangesAsync();
        var returnURL = ViewData["returnURL"]?.ToString();
        if (string.IsNullOrEmpty(returnURL))
        {
            return RedirectToAction(nameof(Index)); //Elephant falls asleep
        }

        return Redirect(returnURL);
    }

    private bool VehicleExists(int? id)
    {
        return _context.Vehicles.Any(e => e.ID == id);
    }

    //Methods to be used
    private void PopulateAssignedLocationData()
    {
        var allLocations = _context.Locations
            .OrderBy(l => l.Name)
            .ToList();

        ViewData["Locations"] = allLocations;
    }

    //Helper methods
    private Task UpdateVehicleLocationsAsync(
    string[] selectedLocations,
    Vehicle vehicleToUpdate)
    {
        var selectedIds = selectedLocations.Select(int.Parse).ToHashSet();

        var currentIds = vehicleToUpdate.VehicleLocations
            .Select(vl => vl.LocationID).ToHashSet();

        //method to remove duplicates
        foreach (var vehicleLocation in vehicleToUpdate.VehicleLocations.ToList())
        {
            if (!selectedIds.Contains(vehicleLocation.LocationID))
            {
                _context.VehicleLocations.Remove(vehicleLocation);
            }
        }

        // Add newly checked locations
        foreach (var locationID in selectedIds)
        {
            if (!currentIds.Contains(locationID))
            {
                vehicleToUpdate.VehicleLocations.Add(new VehicleLocation
                {
                    VehicleID = vehicleToUpdate.ID,
                    LocationID = locationID
                });
            }
        }
        return Task.CompletedTask;
    }
}
