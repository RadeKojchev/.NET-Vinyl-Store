using System.ComponentModel.DataAnnotations;

namespace VtorProektRSWEB.Models
{
    public class Genre
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string? GenreName { get; set; }
        public ICollection<TrackGenres>? TrackGenres { get; set; }

    }
}
