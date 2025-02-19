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

        // Injection de dépendance du contexte de base de données
        public ServiceController(AnnuaireContext context)
        {
            _context = context;
        }

        // GET : api/service
        // Accessible par tout le monde (visiteur + admin)
        [HttpGet]
        [AllowAnonymous]  // Ouvert à tout le monde pour la consultation
        public async Task<ActionResult<IEnumerable<ServiceDTO>>> GetServices()
        {
            // Récupération de tous les services en base de données
            var services = await _context.Services
                .Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    Nom = s.Nom
                })
                .ToListAsync();

            // Retourne la liste des services au format JSON
            return Ok(services);
        }

        // GET : api/service/5
        // Accessible par tout le monde (visiteur + admin)
        [HttpGet("{id}")]
        [AllowAnonymous]  // Ouvert à tout le monde pour la consultation
        public async Task<ActionResult<ServiceDTO>> GetService(int id)
        {
            // Recherche du service par son ID
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
                // Retourne une erreur 404 si le service n'existe pas
                return NotFound(new { message = "Service non trouvé" });
            }

            // Retourne le service trouvé au format JSON
            return Ok(service);
        }

        // POST : api/service
        // Accessible uniquement par un administrateur
        [HttpPost]
        [Authorize(Roles = "Admin")]  // Restreint aux administrateurs
        public async Task<ActionResult<ServiceDTO>> PostService(ServiceDTO serviceDTO)
        {
            // Vérification que le nom du service est unique
            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => s.Nom == serviceDTO.Nom);

            if (existingService != null)
            {
                // Retourne une erreur si le service existe déjà
                return Conflict(new { message = "Ce nom de service existe déjà" });
            }

            // Création d'un nouveau service en base de données
            var service = new ServiceDAO
            {
                Nom = serviceDTO.Nom
            };

            _context.Services.Add(service); // Ajout du service en base de données
            await _context.SaveChangesAsync(); // Sauvegarde des modifications

            // Mise à jour du DTO avec l'ID généré par la base de données
            serviceDTO.Id = service.Id;

            // Retourne le service créé avec un lien vers sa ressource
            return CreatedAtAction(nameof(GetService), new { id = serviceDTO.Id }, serviceDTO);
        }

        // PUT : api/service/5
        // Accessible uniquement par un administrateur
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // Restreint aux administrateurs
        public async Task<IActionResult> PutService(int id, ServiceDTO serviceDTO)
        {
            if (id != serviceDTO.Id)
            {
                // Vérification de l'intégrité des données
                return BadRequest(new { message = "L'ID du service ne correspond pas" });
            }

            // Recherche du service par son ID
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                // Retourne une erreur 404 si le service n'existe pas
                return NotFound(new { message = "Service non trouvé" });
            }

            // Mise à jour du nom du service
            service.Nom = serviceDTO.Nom;

            // Marquage de l'entité comme modifiée
            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); // Sauvegarde des modifications
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour" });
            }

            // Retourne un succès sans contenu (204 No Content)
            return NoContent();
        }

        // DELETE : api/service/5
        // Accessible uniquement par un administrateur
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Restreint aux administrateurs
        public async Task<IActionResult> DeleteService(int id)
        {
            // Recherche du service par son ID
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                // Erreur 404 si le service n'existe pas
                return NotFound(new { message = "Service non trouvé" });
            }

            // Suppression du service
            _context.Services.Remove(service);
            await _context.SaveChangesAsync(); // Sauvegarde des modifications

            // Retourne un succès sans contenu (204 No Content)
            return NoContent();
        }
    }
}
