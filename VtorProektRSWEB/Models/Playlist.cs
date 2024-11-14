using VtorProektRSWEB.Models;

namespace VtorProektRSWEB.Models
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? AppUserId { get; set; } // Nadvoreshen kluc do user za da se povrzat
        public virtual AppUser? AppUser { get; set; } // Navigacija do korisnikot

        public virtual ICollection<PlaylistTrack>? PlaylistTracks { get; set; }

        // Calculated property for number of tracks
        public int NumberOfTracks => PlaylistTracks?.Count ?? 0;

    }
}
