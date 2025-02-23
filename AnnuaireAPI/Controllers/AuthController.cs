using AnnuaireLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<UserSecure> _userManager;
    private readonly SignInManager<UserSecure> _signInManager;

    public AuthController(UserManager<UserSecure> userManager, SignInManager<UserSecure> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    //  Connexion de l'utilisateur (avec session et cookies)
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized("Utilisateur non trouvé");

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);
        if (!result.Succeeded) return Unauthorized("Mot de passe incorrect");

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { Message = "Connexion réussie", Email = user.Email, Roles = roles });
    }

    //  Déconnexion de l'utilisateur
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { Message = "Déconnexion réussie" });
    }

    //  Vérifier qui est connecté
    [HttpGet("whoami")]
    [Authorize]
    public async Task<IActionResult> WhoAmI()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized("Aucun utilisateur connecté");

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { Email = user.Email, Roles = roles });
    }
}

// Modèle pour la requête de connexion
public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
