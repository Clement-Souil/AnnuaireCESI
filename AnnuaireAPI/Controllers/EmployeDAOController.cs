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

        // Injection de dépendance du contexte de base de données
        public EmployeController(AnnuaireContext context)
        {
            _context = context;
        }

        // GET : api/employe
        // Accessible par tout le monde (visiteur + admin)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> GetEmployes()
        {
            // Récupération de tous les employés avec leurs services et sites
            var employes = await _context.Employes
                .Include(e => e.Service) // Jointure avec la table Service
                .Include(e => e.Site)    //Jointure avec la table Site
                .Select(e => new EmployeDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    TelephoneFixe = e.TelephoneFixe,
                    TelephonePortable = e.TelephonePortable,
                    Email = e.Email,
                    Service = e.Service.Nom, // On retourne le nom du service, pas l'ID
                    Site = e.Site.Ville       // On retourne la ville du site, pas l'ID
                })
                .ToListAsync();

            return Ok(employes); // Retourne la liste des employés au format JSON
        }

        // GET : api/employe/5
        // Accessible par tout le monde (visiteur + admin)
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EmployeDTO>> GetEmploye(int id)
        {
            // Recherche de l'employé par son ID
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
                return NotFound(); // Si l'employé n'existe pas, retourne une erreur 404
            }

            return Ok(employe); // Retourne l'employé trouvé
        }
       
        // GET : api/employe/search/nom/{query}
        // Rechercher les employés par nom ou prénom
        [HttpGet("search/nom/{query}")]
        [AllowAnonymous]  // Ouvert à tout le monde pour la consultation
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> SearchByName(string query)
        {
            var employes = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
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

        // GET : api/employe/search/site/{site}
        // Rechercher les employés par site (Ville)
        [HttpGet("search/site/{site}")]
        [AllowAnonymous]  // Ouvert à tout le monde pour la consultation
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> SearchBySite(string site)
        {
            var employes = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .Where(e => e.Site.Ville.Contains(site))  // Filtre sur la ville du site
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

        // GET : api/employe/search/service/{service}
        // Rechercher les employés par service (Nom du service)
        [HttpGet("search/service/{service}")]
        [AllowAnonymous]  // Ouvert à tout le monde pour la consultation
        public async Task<ActionResult<IEnumerable<EmployeDTO>>> SearchByService(string service)
        {
            var employes = await _context.Employes
                .Include(e => e.Service)
                .Include(e => e.Site)
                .Where(e => e.Service.Nom.Contains(service))  // Filtre sur le nom du service
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


        // POST : api/employe
        // Accessible uniquement par un administrateur
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeDTO>> PostEmploye(EmployeDTO employeDTO)
        {
            // Vérification que le service et le site existent en base de données
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Nom == employeDTO.Service);
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Ville == employeDTO.Site);

            if (service == null || site == null)
            {
                return BadRequest("Le service ou le site n'existe pas."); // Erreur si l'un des deux est introuvable
            }

            // 🛠Création d'un nouvel employé à partir du DTO
            var employe = new EmployeDAO
            {
                Nom = employeDTO.Nom,
                Prenom = employeDTO.Prenom,
                TelephoneFixe = employeDTO.TelephoneFixe,
                TelephonePortable = employeDTO.TelephonePortable,
                Email = employeDTO.Email,
                ServiceId = service.Id, // On utilise l'ID du service existant
                SiteId = site.Id         // On utilise l'ID du site existant
            };

            _context.Employes.Add(employe); // Ajout de l'employé en base de données
            await _context.SaveChangesAsync(); // Sauvegarde des modifications

            employeDTO.Id = employe.Id; // Mise à jour du DTO avec l'ID généré

            // Retourne l'employé créé avec un lien vers sa ressource
            return CreatedAtAction(nameof(GetEmploye), new { id = employeDTO.Id }, employeDTO);
        }

        // DELETE : api/employe/5
        // Accessible uniquement par un administrateur
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmploye(int id)
        {
            // Recherche de l'employé par son ID
            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
            {
                return NotFound(); // Erreur 404 si l'employé n'existe pas
            }

            _context.Employes.Remove(employe); // Suppression de l'employé
            await _context.SaveChangesAsync(); // Sauvegarde des modifications

            return NoContent(); // Retourne un succès sans contenu (204 No Content)
        }
    }
}

