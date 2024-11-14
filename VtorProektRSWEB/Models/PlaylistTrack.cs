using VtorProektRSWEB.Models;

namespace VtorProektRSWEB.Models
{
    public class PlaylistTrack
    {
        public Guid Id { get; set; }
        public Guid PlaylistId { get; set; } // Foreign key to Playlist
        public Playlist Playlist { get; set; } // Navigation property for Playlist

        public Guid TrackId { get; set; } // Foreign key to Track
        public Track Track { get; set; } // Navigation property for Track

        // Additional properties can be added here
        public DateTime DateAdded { get; set; } // Example: When the track was added to the playlist
    }
}
