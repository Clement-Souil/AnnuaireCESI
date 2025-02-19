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
        [Authorize(Roles = "Admin")]  // Restreint aux administrateurs
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
        // Accessible uniquement par un administrateur
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // Restreint aux administrateurs
        public async Task<IActionResult> PutSite(int id, SiteDTO siteDTO)
        {
            if (id != siteDTO.Id)
            {
                // Vérification de l'intégrité des données
                return BadRequest(new { message = "L'ID du site ne correspond pas" });
            }

            // Recherche du site par son ID
            var site = await _context.Sites.FindAsync(id);
            if (site == null)
            {
                // Retourne une erreur 404 si le site n'existe pas
                return NotFound(new { message = "Site non trouvé" });
            }

            // Mise à jour du nom du site
            site.Ville = siteDTO.Ville;

            // Marquage de l'entité comme modifiée
            _context.Entry(site).State = EntityState.Modified;

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

        // DELETE : api/site/5
        // Accessible uniquement par un administrateur
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Restreint aux administrateurs
        public async Task<IActionResult> DeleteSite(int id)
        {
            // Recherche du site par son ID
            var site = await _context.Sites.FindAsync(id);
            if (site == null)
            {
                // Erreur 404 si le site n'existe pas
                return NotFound(new { message = "Site non trouvé" });
            }

            // Suppression du site
            _context.Sites.Remove(site);
            await _context.SaveChangesAsync(); // Sauvegarde des modifications

            // Retourne un succès sans contenu (204 No Content)
            return NoContent();
        }
    }
}

