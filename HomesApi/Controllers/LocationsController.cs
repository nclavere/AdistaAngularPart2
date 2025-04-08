using HomesApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomesApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LocationsController : ControllerBase
{
    // GET: api/Locations
    [HttpGet]
    public IEnumerable<HousingLocation> Get()
    {
        return HousingLocationService.HousingLocationList;
    }

    // GET api/Locations/5
    [HttpGet("{id}")]
    public HousingLocation? Get(int id)
    {
        return HousingLocationService.HousingLocationList.Find(item => item.Id == id);
    }
}
