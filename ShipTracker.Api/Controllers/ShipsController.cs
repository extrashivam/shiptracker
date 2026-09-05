using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipTracker.Api.Data;
using ShipTracker.Api.Models;

namespace ShipTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ShipsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IEnumerable<Ship>> Get()
        => await _db.Ships.OrderBy(s => s.Name).ToListAsync();

    [HttpGet("{mmsi}")]
    public async Task<ActionResult<Ship>> GetByMmsi(string mmsi)
    {
        var ship = await _db.Ships.FirstOrDefaultAsync(s => s.Mmsi == mmsi);
        return ship is null ? NotFound() : ship;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _db.Ships.AnyAsync())
            return Ok("Already seeded.");

        _db.Ships.AddRange(
            new Ship { Mmsi = "367123456", Name = "Ever Given", Latitude = 30.0131, Longitude = 32.5498, SpeedOverGround = 0.2, CourseOverGround = 45.0, LastUpdatedUtc = DateTime.UtcNow },
            new Ship { Mmsi = "367987654", Name = "Maersk Alabama", Latitude = 4.6096, Longitude = -74.0817, SpeedOverGround = 18.4, CourseOverGround = 210.5, LastUpdatedUtc = DateTime.UtcNow },
            new Ship { Mmsi = "367555111", Name = "Cosco Shipping", Latitude = 1.2903, Longitude = 103.8519, SpeedOverGround = 12.1, CourseOverGround = 90.0, LastUpdatedUtc = DateTime.UtcNow }
        );

        await _db.SaveChangesAsync();
        return Ok("Seeded 3 ships.");
    }
}