using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VtorProektRSWEB.Models;

namespace VtorProektRSWEB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<VtorProektRSWEB.Models.Album> Album { get; set; } = default!;
        public DbSet<VtorProektRSWEB.Models.Artist> Artist { get; set; } = default!;
        public DbSet<VtorProektRSWEB.Models.Genre> Genre { get; set; } = default!;
        public DbSet<VtorProektRSWEB.Models.Playlist> Playlist { get; set; } = default!;
        public DbSet<   VtorProektRSWEB.Models.Track> Track { get; set; } = default!;
        public DbSet<VtorProektRSWEB.Models.PlaylistTrack> PlaylistTrack { get; set; }
        public DbSet<VtorProektRSWEB.Models.ArtistTracks> ArtistTracks { get; set; }
        public DbSet<VtorProektRSWEB.Models.TrackGenres> TrackGenres { get; set; }

    }
}
