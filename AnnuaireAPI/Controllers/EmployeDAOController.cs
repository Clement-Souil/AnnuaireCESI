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
    public class EmployeController : ControllerBase
    {
        private readonly AnnuaireContext _context;

        public EmployeController(AnnuaireContext context)
        {
            _context = context;
        }

        // 🔹 GET : api/employe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> GetEmployes()
        {
            var employes = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .Select(e => new EmployeDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    TelephoneFixe = e.TelephoneFixe,
                    TelephonePortable = e.TelephonePortable,
                    Email = e.Email,
                    Service = e.Service.Nom,  // 🔹 On retourne le nom du Service, pas l'ID
                    Site = e.Site.Ville       // 🔹 On retourne la ville du Site, pas l'ID
                })
                .ToListAsync();

            return Ok(employes);
        }

        // 🔹 GET : api/employe/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeDTO>> GetEmploye(int id)
        {
            var employe = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .Where(e => e.Id == id)
                .Select(e => new EmployeDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    TelephoneFixe = e.TelephoneFixe,
                    TelephonePortable = e.TelephonePortable,
                    Email = e.Email,
                    Service = e.Service.Nom,
                    Site = e.Site.Ville
                })
                .FirstOrDefaultAsync();

            if (employe == null)
            {
                return NotFound();
            }

            return Ok(employe);
        }

        // 🔹 POST : api/employe
        [HttpPost]
        public async Task<ActionResult<EmployeDTO>> PostEmploye(EmployeDTO employeDTO)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Nom == employeDTO.Service);
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Ville == employeDTO.Site);

            if (service == null || site == null)
            {
                return BadRequest("Le service ou le site n'existe pas.");
            }

            var employe = new EmployeDAO
            {
                Nom = employeDTO.Nom,
                Prenom = employeDTO.Prenom,
                TelephoneFixe = employeDTO.TelephoneFixe,
                TelephonePortable = employeDTO.TelephonePortable,
                Email = employeDTO.Email,
                ServiceId = service.Id,
                SiteId = site.Id
            };

            _context.Employes.Add(employe);
            await _context.SaveChangesAsync();

            employeDTO.Id = employe.Id;
            return CreatedAtAction(nameof(GetEmploye), new { id = employeDTO.Id }, employeDTO);
        }

        // 🔹 PUT : api/employe/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmploye(int id, EmployeDTO employeDTO)
        {
            if (id != employeDTO.Id)
            {
                return BadRequest();
            }

            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FirstOrDefaultAsync(s => s.Nom == employeDTO.Service);
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Ville == employeDTO.Site);

            if (service == null || site == null)
            {
                return BadRequest("Le service ou le site n'existe pas.");
            }

            employe.Nom = employeDTO.Nom;
            employe.Prenom = employeDTO.Prenom;
            employe.TelephoneFixe = employeDTO.TelephoneFixe;
            employe.TelephonePortable = employeDTO.TelephonePortable;
            employe.Email = employeDTO.Email;
            employe.ServiceId = service.Id;
            employe.SiteId = site.Id;

            _context.Entry(employe).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔹 DELETE : api/employe/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmploye(int id)
        {
            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
            {
                return NotFound();
            }

            _context.Employes.Remove(employe);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
