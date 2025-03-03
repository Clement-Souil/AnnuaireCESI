using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnnuaireLibrary.Data;
using AnnuaireLibrary.DTO;
using AnnuaireLibrary.DAO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AnnuaireLibrary.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AnnuaireAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeController : ControllerBase
    {
        private readonly AnnuaireContext _context;
        private readonly UserManager<UserSecure> _userManager;
        private readonly ILogger<EmployeController> _logger;

        public EmployeController(AnnuaireContext context, UserManager<UserSecure> userManager, ILogger<EmployeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        //  Lister tous les employés (accessible à tous)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> GetEmployes()
        {
            var employes = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .AsNoTracking() // Optimisation
                .Select(e => new EmployeDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    TelephoneFixe = e.TelephoneFixe,
                    TelephonePortable = e.TelephonePortable,
                    Email = e.Email,
                    ServiceId = e.Service.Id,  // Ajout de l'ID
                    SiteId = e.Site.Id,
                    Service = e.Service.Nom,
                    Site = e.Site.Ville
                })
                .ToListAsync();

            return Ok(employes);
        }

        //  Récupérer un employé par ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EmployeDTO>> GetEmploye(int id)
        {
            var employe = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EmployeDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    TelephoneFixe = e.TelephoneFixe,
                    TelephonePortable = e.TelephonePortable,
                    Email = e.Email,
                    ServiceId = e.Service.Id,  // Ajout de l'ID
                    SiteId = e.Site.Id,
                    Service = e.Service.Nom,
                    Site = e.Site.Ville
                })
                .FirstOrDefaultAsync();

            return employe == null ? NotFound("Employé non trouvé") : Ok(employe);
        }

        //  Recherche par nom/prénom
        [HttpGet("search/nom/{query}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> SearchByName(string query)
        {
            var employes = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .AsNoTracking()
                .Where(e => e.Nom.Contains(query) || e.Prenom.Contains(query))
                .Select(e => new EmployeDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    TelephoneFixe = e.TelephoneFixe,
                    TelephonePortable = e.TelephonePortable,
                    Email = e.Email,
                    ServiceId = e.Service.Id,  // Ajout de l'ID
                    SiteId = e.Site.Id,
                    Service = e.Service.Nom,
                    Site = e.Site.Ville
                })
                .ToListAsync();

            return Ok(employes);
        }

        //  Recherche par site (ville)
        [HttpGet("search/site/{site}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> SearchBySite(string site)
        {
            var employes = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .AsNoTracking()
                .Where(e => e.Site.Ville.Contains(site))
                .Select(e => new EmployeDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    TelephoneFixe = e.TelephoneFixe,
                    TelephonePortable = e.TelephonePortable,
                    Email = e.Email,
                    ServiceId = e.Service.Id,  // Ajout de l'ID
                    SiteId = e.Site.Id,
                    Service = e.Service.Nom,
                    Site = e.Site.Ville
                })
                .ToListAsync();

            return Ok(employes);
        }

        //  Recherche par service
        [HttpGet("search/service/{service}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> SearchByService(string service)
        {
            var employes = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .AsNoTracking()
                .Where(e => e.Service.Nom.Contains(service))
                .Select(e => new EmployeDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    TelephoneFixe = e.TelephoneFixe,
                    TelephonePortable = e.TelephonePortable,
                    Email = e.Email,
                    ServiceId = e.Service.Id,  // Ajout de l'ID
                    SiteId = e.Site.Id,
                    Service = e.Service.Nom,
                    Site = e.Site.Ville
                })
                .ToListAsync();

            return Ok(employes);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmploye(int id, EmployeDTO employeDTO)
        {
            if (id != employeDTO.Id)
                return BadRequest("L'ID de l'employé ne correspond pas.");

            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
                return NotFound();

            employe.Nom = employeDTO.Nom;
            employe.Prenom = employeDTO.Prenom;
            employe.TelephoneFixe = employeDTO.TelephoneFixe;
            employe.TelephonePortable = employeDTO.TelephonePortable;
            employe.Email = employeDTO.Email;
            employe.ServiceId = employeDTO.ServiceId;
            employe.SiteId = employeDTO.SiteId;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        //  Ajouter un employé (ADMIN uniquement)
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeDTO>> PostEmploye(EmployeDTO employeDTO)
        {
            if (string.IsNullOrWhiteSpace(employeDTO.Nom) || string.IsNullOrWhiteSpace(employeDTO.Prenom))
                return BadRequest("Nom et prénom sont obligatoires");

            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == employeDTO.ServiceId);
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Id == employeDTO.SiteId);

            if (service == null || site == null)
                return BadRequest("Le service ou le site n'existe pas.");

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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmploye(int id)
        {
            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
                return NotFound();

            _context.Employes.Remove(employe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("exists/{siteId}")]
        public async Task<IActionResult> CheckIfSiteIsUsed(int siteId)
        {
            bool isUsed = await _context.Employes.AnyAsync(e => e.SiteId == siteId);
            return Ok(isUsed);
        }

    }
}
