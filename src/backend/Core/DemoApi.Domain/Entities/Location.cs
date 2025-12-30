namespace DemoApi.Domain.Entities;

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
