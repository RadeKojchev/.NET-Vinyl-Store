using System.ComponentModel.DataAnnotations;
using VtorProektRSWEB.Models;

namespace VtorProektRSWEB.Models
{
    public class Track
    {
        [Key]
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public float? Duration { get; set; }
        public Album? AlbumName { get; set; }
        public Guid? AlbumId { get; set; }
        //[Required] ne maaj dalgi so required i ?...
        public ICollection<ArtistTracks>? ArtistTracks { get; set; }
        public ICollection<TrackGenres>? TrackGenres { get; set; }
        public ICollection<PlaylistTrack>? PlaylistTracks { get; set; }

    }
}
