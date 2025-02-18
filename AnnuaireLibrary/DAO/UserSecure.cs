using Microsoft.AspNetCore.Identity;

namespace AnnuaireLibrary.Models
{
    public class UserSecure : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
