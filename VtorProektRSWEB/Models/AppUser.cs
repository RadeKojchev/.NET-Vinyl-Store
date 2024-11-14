using Microsoft.AspNetCore.Identity;
using VtorProektRSWEB.Models;

namespace VtorProektRSWEB.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Playlist>? UserPlaylists { get; set; }
    }
}
