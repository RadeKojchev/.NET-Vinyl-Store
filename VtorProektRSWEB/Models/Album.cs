using System.ComponentModel.DataAnnotations;

namespace VtorProektRSWEB.Models
{
    public class Album
    {
            [Key]
            public Guid Id { get; set; }
            [StringLength(100)]
            [Required]
            public string Title { get; set; }
            public int YearPublished { get; set; }
            public string? Label { get; set; }
            public string? DownloadURL { get; set; }
            public string? AlbumCover { get; set; }
            public double priceVinyl { get; set; }
            
            [Required]
            public Guid ArtistId { get; set; }
            public Artist? ArtistName { get; set; }
            public ICollection<Track>? AlbumTracks { get; set; }

        }
    }



