
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Data;
using Microsoft.Net.Http.Headers;

public class VehicleController : Controller
{
    private readonly CarRentalContext _context;

    public VehicleController(CarRentalContext context)
    {
        _context = context;
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


    // GET: VEHICLES
    public async Task<IActionResult> Index()    
    {
        //return View(await _context.Vehicles.ToListAsync()); old code

        var vehicles = await _context.Vehicles
        .Include(v => v.VehicleLocations)
        .ThenInclude(vl => vl.Location)
        .AsNoTracking()
        .ToListAsync();

        return View(vehicles);
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
                return RedirectToAction(nameof(Index));
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
                    return RedirectToAction(nameof(Index));
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
        return RedirectToAction(nameof(Index));
    }

    private bool VehicleExists(int? id)
    {
        return _context.Vehicles.Any(e => e.ID == id);
    }
}
