using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnnuaireLibrary.Data;
using AnnuaireLibrary.DTO;
using AnnuaireLibrary.DAO;
using Microsoft.AspNetCore.Authorization;

namespace AnnuaireAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SiteController : ControllerBase
    {
        private readonly AnnuaireContext _context;

        // Injection de dépendance du contexte de base de données
        public SiteController(AnnuaireContext context)
        {
            _context = context;
        }

        // GET : api/site
        // Accessible par tout le monde (visiteur + admin)
        [HttpGet]
        [AllowAnonymous]  // Ouvert à tout le monde pour la consultation
        public async Task<ActionResult<IEnumerable<SiteDTO>>> GetSites()
        {
            // Récupération de tous les sites en base de données
            var sites = await _context.Sites
                .Select(s => new SiteDTO
                {
                    Id = s.Id,
                    Ville = s.Ville
                })
                .ToListAsync();

            // Retourne la liste des sites au format JSON
            return Ok(sites);
        }

        // GET : api/site/5
        // Accessible par tout le monde (visiteur + admin)
        [HttpGet("{id}")]
        [AllowAnonymous]  // Ouvert à tout le monde pour la consultation
        public async Task<ActionResult<SiteDTO>> GetSite(int id)
        {
            // Recherche du site par son ID
            var site = await _context.Sites
                .Where(s => s.Id == id)
                .Select(s => new SiteDTO
                {
                    Id = s.Id,
                    Ville = s.Ville
                })
                .FirstOrDefaultAsync();

            if (site == null)
            {
                // Retourne une erreur 404 si le site n'existe pas
                return NotFound(new { message = "Site non trouvé" });
            }

            // Retourne le site trouvé au format JSON
            return Ok(site);
        }

        // POST : api/site
        // Accessible uniquement par un administrateur
        [HttpPost]
        //[Authorize(Roles = "Admin")]  // Restreint aux administrateurs
        public async Task<ActionResult<SiteDTO>> PostSite(SiteDTO siteDTO)
        {
            // Vérification que le nom du site est unique
            var existingSite = await _context.Sites
                .FirstOrDefaultAsync(s => s.Ville == siteDTO.Ville);

            if (existingSite != null)
            {
                // Retourne une erreur si le site existe déjà
                return Conflict(new { message = "Ce nom de site existe déjà" });
            }

            // Création d'un nouveau site en base de données
            var site = new SiteDAO
            {
                Ville = siteDTO.Ville
            };

            _context.Sites.Add(site); // Ajout du site en base de données
            await _context.SaveChangesAsync(); // Sauvegarde des modifications

            // Mise à jour du DTO avec l'ID généré par la base de données
            siteDTO.Id = site.Id;

            // Retourne le site créé avec un lien vers sa ressource
            return CreatedAtAction(nameof(GetSite), new { id = siteDTO.Id }, siteDTO);
        }

        // PUT : api/site/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSite(int id, [FromBody] SiteDTO siteDTO)
        {
            if (id != siteDTO.Id)
            {
                return BadRequest("L'ID du site dans l'URL ne correspond pas à l'ID du JSON.");
            }

            var existingSite = await _context.Sites.FindAsync(id);
            if (existingSite == null)
            {
                return NotFound("Le site n'existe pas.");
            }

            // Met à jour les champs
            existingSite.Ville = siteDTO.Ville;

            try
            {
                await _context.SaveChangesAsync(); // Enregistre en base
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }

            return NoContent(); // Réponse 204 OK si tout va bien
        }


        // DELETE : api/site/5
        // Accessible uniquement par un administrateur
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSite(int id)
        {
            var site = await _context.Sites.FindAsync(id);
            if (site == null)
                return NotFound();

            // Vérifier s'il y a des employés attachés à ce site
            bool hasEmployes = await _context.Employes.AnyAsync(e => e.SiteId == id);
            if (hasEmployes)
                return BadRequest("Impossible de supprimer ce site car des employés y sont rattachés.");

            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}