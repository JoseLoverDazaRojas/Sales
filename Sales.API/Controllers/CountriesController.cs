using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    [ApiController]
    [Route("/api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Countries.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetId(int id)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Id == id);
            if (country ==null)
            { 
                return NotFound();
            }
            return Ok(country);
        }
        
        [HttpPost]
        public async Task<ActionResult> Post(Country country)
        {
            _context.Add(country);
            await _context.SaveChangesAsync();
            return Ok(country);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, Country country)
        {
            var currentCountry = await _context.Countries.FirstOrDefaultAsync(x => x.Id == id);
            if (currentCountry == null)
            {
                return NotFound();
            }
            currentCountry.Name = country.Name;
            _context.Update(currentCountry);
            await _context.SaveChangesAsync();
            return Ok(currentCountry);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Id == id);
            if (country == null)
            {
                return NotFound();
            }           
            _context.Remove(country);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}