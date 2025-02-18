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

        public SiteController(AnnuaireContext context)
        {
            _context = context;
        }

        // GET : api/site
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteDTO>>> GetSites()
        {
            var sites = await _context.Sites
                .Select(s => new SiteDTO
                {
                    Id = s.Id,
                    Ville = s.Ville
                })
                .ToListAsync();

            return Ok(sites);
        }

        // GET : api/site/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SiteDTO>> GetSite(int id)
        {
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
                return NotFound();
            }

            return Ok(site);
        }

        // POST : api/site
        [HttpPost]
        public async Task<ActionResult<SiteDTO>> PostSite(SiteDTO siteDTO)
        {
            var site = new SiteDAO
            {
                Ville = siteDTO.Ville
            };

            _context.Sites.Add(site);
            await _context.SaveChangesAsync();

            siteDTO.Id = site.Id;
            return CreatedAtAction(nameof(GetSite), new { id = siteDTO.Id }, siteDTO);
        }

        // PUT : api/site/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSite(int id, SiteDTO siteDTO)
        {
            if (id != siteDTO.Id)
            {
                return BadRequest();
            }

            var site = await _context.Sites.FindAsync(id);
            if (site == null)
            {
                return NotFound();
            }

            site.Ville = siteDTO.Ville;

            _context.Entry(site).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE : api/site/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSite(int id)
        {
            var site = await _context.Sites.FindAsync(id);
            if (site == null)
            {
                return NotFound();
            }

            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
