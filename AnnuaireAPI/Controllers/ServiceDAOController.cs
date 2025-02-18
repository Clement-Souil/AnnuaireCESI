using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnnuaireLibrary.Data;
using AnnuaireLibrary.DTO;
using AnnuaireLibrary.DAO;
using Microsoft.AspNetCore.Authorization;

namespace AnnuaireAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly AnnuaireContext _context;

        public ServiceController(AnnuaireContext context)
        {
            _context = context;
        }

        // GET : api/service
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDTO>>> GetServices()
        {
            var services = await _context.Services
                .Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    Nom = s.Nom
                })
                .ToListAsync();

            return Ok(services);
        }

        // GET : api/service/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDTO>> GetService(int id)
        {
            var service = await _context.Services
                .Where(s => s.Id == id)
                .Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    Nom = s.Nom
                })
                .FirstOrDefaultAsync();

            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }

        // POST : api/service
        [HttpPost]
        public async Task<ActionResult<ServiceDTO>> PostService(ServiceDTO serviceDTO)
        {
            var service = new ServiceDAO
            {
                Nom = serviceDTO.Nom
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            serviceDTO.Id = service.Id;
            return CreatedAtAction(nameof(GetService), new { id = serviceDTO.Id }, serviceDTO);
        }

        // PUT : api/service/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(int id, ServiceDTO serviceDTO)
        {
            if (id != serviceDTO.Id)
            {
                return BadRequest();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            service.Nom = serviceDTO.Nom;

            _context.Entry(service).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE : api/service/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
