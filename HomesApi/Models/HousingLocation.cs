namespace HomesApi.Models;

public class HousingLocation
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Photo { get; set; } = null!;
    public int AvailableUnits { get; set; }
    public bool Wifi { get; set; }
    public bool Laundry { get; set; }

}
