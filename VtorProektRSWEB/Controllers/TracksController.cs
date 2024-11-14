using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VtorProektRSWEB.Data;
using VtorProektRSWEB.Models;

namespace VtorProektRSWEB.Controllers
{
    public class TracksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TracksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adding track to user's playlist
        public async Task<IActionResult> AddToPlaylist(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var track = await _context.Track
                .Include(t => t.ArtistTracks)
                .ThenInclude(at => at.Artist)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null)
            {
                return NotFound();
            }

            var userPlaylists = await _context.Playlist
                .Where(p => p.AppUserId == userId)
                .ToListAsync();

            // If no playlists exist, redirect to create a new one
            if (userPlaylists == null || !userPlaylists.Any())
            {
                TempData["CreatePlaylistMessage"] = "You must create a playlist before adding tracks.";
                return RedirectToAction("Create", "Playlists");
            }

            // Pass the list of playlists to the view for selection
            var selectList = new SelectList(userPlaylists, "Id", "Name");
            return View("SelectPlaylist", Tuple.Create(id, selectList));
        }


        [HttpPost]
        public async Task<IActionResult> AddTrackToSelectedPlaylist(Guid trackId, Guid playlistId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            // Fetch the selected playlist
            var selectedPlaylist = await _context.Playlist
                .FirstOrDefaultAsync(p => p.Id == playlistId && p.AppUserId == userId);

            if (selectedPlaylist == null)
            {
                return NotFound();
            }

            // Check if the track is already in the playlist
            var existingPlaylistTrack = await _context.PlaylistTrack
                .FirstOrDefaultAsync(pt => pt.PlaylistId == playlistId && pt.TrackId == trackId);

            if (existingPlaylistTrack != null)
            {
                TempData["Message"] = "This track is already in the selected playlist.";
                return RedirectToAction("Index", "Tracks");
            }

            // Add the track to the selected playlist
            var playlistTrack = new PlaylistTrack
            {
                Id = Guid.NewGuid(),
                PlaylistId = playlistId,
                TrackId = trackId,
                DateAdded = DateTime.Now
            };

            _context.PlaylistTrack.Add(playlistTrack);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Track has been successfully added to the playlist!";
            return RedirectToAction(nameof(Index));
        }


        // GET: Tracks/Index
        public async Task<IActionResult> Index(string Name, Guid? ArtistId, Guid? GenreId)
        {
            var tracks = _context.Track.AsQueryable();

            if (!string.IsNullOrEmpty(Name))
            {
                tracks = tracks.Where(t => t.Name.Contains(Name));
            }

            if (ArtistId.HasValue)
            {
                tracks = tracks.Where(t => t.ArtistTracks.Any(at => at.ArtistId == ArtistId));
            }

            if (GenreId.HasValue)
            {
                tracks = tracks.Where(t => t.TrackGenres.Any(tg => tg.GenreId == GenreId));
            }

            tracks = tracks
                .Include(t => t.ArtistTracks)
                .ThenInclude(at => at.Artist)
                .Include(t => t.TrackGenres)
                .ThenInclude(tg => tg.Genre);

            ViewData["Genres"] = new SelectList(await _context.Genre.ToListAsync(), "Id", "GenreName", GenreId);
            ViewData["Artists"] = new SelectList(await _context.Artist.ToListAsync(), "Id", "Name", ArtistId);

            return View(await tracks.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Track
                .Include(t => t.AlbumName)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // GET: Tracks/Create
        //Authorize(Roles = "Admin")
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Title");
            ViewData["GenreNames"] = new SelectList(_context.Genre, "Id", "GenreName");
            ViewData["ArtistNames"] = new SelectList(_context.Artist, "Id", "Name");
            return View();
        }

        // POST: Tracks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create([Bind("Id,Name,Duration,AlbumId")] Track track, List<Guid> ArtistId, List<Guid> GenreId)
        {
            if (ModelState.IsValid)
            {
                // Fetch and validate album and artist
                if (track.AlbumId.HasValue)
                {
                    var selectedAlbum = await _context.Album
                        .Include(a => a.ArtistName) 
                        .FirstOrDefaultAsync(a => a.Id == track.AlbumId);

                    if (selectedAlbum == null || selectedAlbum.ArtistName == null)
                    {
                        ModelState.AddModelError("AlbumId", "Selected album is invalid or has no associated artist.");
                        PopulateSelectLists(track.AlbumId, ArtistId, GenreId);
                        return View(track);
                    }

                    // Check if at least one selected artist matches the album artist
                    var matchingArtist = ArtistId.Any(a => a == selectedAlbum.ArtistName.Id);
                    if (!matchingArtist)
                    {
                        ModelState.AddModelError("", "At least one of the track's artists must match the selected album's artist.");
                        PopulateSelectLists(track.AlbumId, ArtistId, GenreId);
                        return View(track);
                    }
                }

                // Add ArtistTracks and TrackGenres
                track.ArtistTracks = new List<ArtistTracks>();
                track.TrackGenres = new List<TrackGenres>();

                // Add selected artists to the track
                ArtistId.ForEach(a =>
                {
                    var artist = _context.Artist.Find(a);
                    track.ArtistTracks.Add(new ArtistTracks { Track = track, Artist = artist, ArtistId = a });
                });

                // Add selected genres to the track
                GenreId.ForEach(g =>
                {
                    var genre = _context.Genre.Find(g);
                    track.TrackGenres.Add(new TrackGenres { Track = track, Genre = genre, GenreId = g });
                });

                // Add the track to the context and save
                _context.Add(track);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If we get here, something failed, redisplay the form
            PopulateSelectLists(track.AlbumId, ArtistId, GenreId);
            return View(track);
        }


        private void PopulateSelectLists(Guid? albumId, List<Guid> artistIds, List<Guid> genreIds)
        {
            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Title", albumId);
            ViewData["GenreNames"] = new SelectList(_context.Genre, "Id", "GenreName", genreIds);
            ViewData["ArtistNames"] = new SelectList(_context.Artist, "Id", "Name", artistIds);
        }

        // GET: Tracks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Track.FindAsync(id);
            if (track == null)
            {
                return NotFound();
            }

            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Title", track.AlbumId);
            return View(track);
        }

        // POST: Tracks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Duration,AlbumId")] Track track, List<Guid> ArtistId, List<Guid> GenreId)
        {
            if (id != track.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(track);

                    // Remove existing artist and genre associations
                    _context.ArtistTracks.RemoveRange(_context.ArtistTracks.Where(at => at.TrackId == id));
                    _context.TrackGenres.RemoveRange(_context.TrackGenres.Where(tg => tg.TrackId == id));

                    // Reassign new artist and genre associations
                    track.ArtistTracks = new List<ArtistTracks>();
                    track.TrackGenres = new List<TrackGenres>();

                    ArtistId.ForEach(a =>
                    {
                        track.ArtistTracks.Add(new ArtistTracks { TrackId = id, ArtistId = a });
                    });

                    GenreId.ForEach(g =>
                    {
                        track.TrackGenres.Add(new TrackGenres { TrackId = id, GenreId = g });
                    });

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackExists(track.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(track.AlbumId, ArtistId, GenreId);
            return View(track);
        }

        private bool TrackExists(Guid? id)
        {
            throw new NotImplementedException();
        }

        // GET: Tracks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Track
                .Include(t => t.AlbumName)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var track = await _context.Track.FindAsync(id);
            _context.Track.Remove(track);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackExists(Guid id)
        {
            return _context.Track.Any(e => e.Id == id);
        }
    }
}
