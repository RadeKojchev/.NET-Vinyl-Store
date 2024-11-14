namespace VtorProektRSWEB.Models
{
    public class ArtistTracks
    {
        public Guid Id { get; set; }
        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; }
        public Guid TrackId { get; set; }
        public Track Track { get; set; }

    }
}
