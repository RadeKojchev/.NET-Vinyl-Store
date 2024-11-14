using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VtorProektRSWEB.Data;
using VtorProektRSWEB.Models;

namespace VtorProektRSWEB.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlaylistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Playlists
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var playlists = await _context.Playlist
                .Include(p => p.PlaylistTracks)
                .Where(p => p.AppUserId == userId)
                .ToListAsync();

            return View(playlists);
        }



        // GET: Playlists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Playlists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Playlist playlist)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                playlist.Id = Guid.NewGuid();
                playlist.AppUserId = userId;
                _context.Playlist.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(playlist);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist
                .Include(p => p.AppUser)
                .Include(p => p.PlaylistTracks) 
                    .ThenInclude(pt => pt.Track) 
                    .ThenInclude(pt=>pt.ArtistTracks)
                    .ThenInclude(pt=>pt.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // GET: Playlists/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // POST: Playlists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] Playlist playlist)
        {
            if (id != playlist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.Id))
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
            return View(playlist);
        }

        // POST: Playlists/RemoveTrack
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTrack(Guid playlistId, Guid trackId)
        {
            var playlistTrack = await _context.PlaylistTrack
                .FirstOrDefaultAsync(pt => pt.PlaylistId == playlistId && pt.TrackId == trackId);

            if (playlistTrack != null)
            {
                _context.PlaylistTrack.Remove(playlistTrack);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new { id = playlistId });
        }

        private bool PlaylistExists(Guid id)
        {
            return _context.Playlist.Any(e => e.Id == id);
        }
        public async Task<FileContentResult> CreatePlaylistInvoice(Guid playlistId)
        {
            var playlist = await _context.Playlist
                .Include(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .ThenInclude(t => t.ArtistTracks)
                .ThenInclude(at => at.Artist)
                .FirstOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null)
            {
                return null; // Handle the case where the playlist is not found.
            }

            // Load the invoice template
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "PlaylistInvoice.docx");
            var document = DocumentModel.Load(templatePath);

            // Calculate total price for the playlist (3$ per track)
            var totalPrice = playlist.NumberOfTracks * 3;

            // Track details formatting (each track detail on a new line)
            var trackDetails = new StringBuilder();
            foreach (var playlistTrack in playlist.PlaylistTracks)
            {
                var trackName = playlistTrack.Track?.Name ?? "Unknown Track";
                var artistName = playlistTrack.Track?.ArtistTracks?.FirstOrDefault()?.Artist?.Name ?? "Unknown Artist";
                var duration = playlistTrack.Track?.Duration != null ? ((double)playlistTrack.Track.Duration / 100).ToString("F2"): "Unknown Duration";


                trackDetails.AppendLine($"Track: {trackName}, Artist: {artistName}, Duration: {duration}");
            }

            // Replace placeholders in the template
            document.Content.Replace("{{OrderId}}", playlist.Id.ToString());
            document.Content.Replace("{{PlaylistName}}", playlist.Name);
            document.Content.Replace("{{NumberOfTracks}}", playlist.NumberOfTracks.ToString());
            document.Content.Replace("{{TrackDetails}}", trackDetails.ToString()); // New line-separated track details
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString("F2") + "$");

            // Save the document to a PDF
            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "PlaylistInvoice.pdf");
        }
        // GET: Playlists/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist
                .Include(p => p.PlaylistTracks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }
        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var playlist = await _context.Playlist
                .Include(p => p.PlaylistTracks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist != null)
            {
                _context.Playlist.Remove(playlist);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
