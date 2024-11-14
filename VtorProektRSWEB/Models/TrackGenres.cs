namespace VtorProektRSWEB.Models
{
    public class TrackGenres
    {
        public Guid Id { get; set; }
        public Guid TrackId { get; set; }
        public Track Track { get; set; }
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; }

    }
}
