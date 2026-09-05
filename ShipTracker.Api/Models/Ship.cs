namespace ShipTracker.Api.Models;

public class Ship
{
    public int Id { get; set; }
    public required string Mmsi { get; set; }
    public string? Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? SpeedOverGround { get; set; }
    public double? CourseOverGround { get; set; }
    public string? Destination { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}