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
    [Authorize]
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
                    Service = e.Service.Nom,
                    Site = e.Site.Ville
                })
                .ToListAsync();

            return Ok(employes);
        }

        //  Ajouter un employé (ADMIN uniquement)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeDTO>> PostEmploye(EmployeDTO employeDTO)
        {
            if (string.IsNullOrWhiteSpace(employeDTO.Nom) || string.IsNullOrWhiteSpace(employeDTO.Prenom))
                return BadRequest("Nom et prénom sont obligatoires");

            var service = await _context.Services.FirstOrDefaultAsync(s => s.Nom == employeDTO.Service);
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Ville == employeDTO.Site);

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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("Tentative de suppression sans être connecté.");
                return Unauthorized("Vous devez être connecté.");
            }

            var roles = await _userManager.GetRolesAsync(user) ?? new List<string>(); // Empêche le null (je comprends rien punaise)

            if (roles.Count == 0)
            {
                Console.WriteLine("Accès refusé : L'utilisateur n'a aucun rôle attribué.");
                return Forbid("Accès refusé : Vous n'avez aucun rôle attribué.");
            }

            if (!roles.Contains("Admin"))
            {
                Console.WriteLine("Accès refusé : Vous n'êtes pas administrateur.");
                return Forbid("Accès refusé : action réservée aux administrateurs.");
            }

            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
            {
                Console.WriteLine("Employé introuvable !");
                return NotFound("Employé non trouvé.");
            }

            _context.Employes.Remove(employe);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Suppression réussie de l'employé {id} par {user.Email}");
            return NoContent();
        }




    }
}
