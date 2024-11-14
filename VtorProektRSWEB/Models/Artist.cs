using System.ComponentModel.DataAnnotations;

namespace VtorProektRSWEB.Models
{
    public class Artist
    {

        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string? Country { get; set; }
        ICollection<ArtistTracks>? ArtistTracks { get; set; }
        ICollection<Album>? Albums { get; set; }

    }
}
