using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VtorProektRSWEB.Data;
using VtorProektRSWEB.Models;
using GemBox.Document;

namespace VtorProektRSWEB.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlbumsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Album
                .Include(a => a.ArtistName)
                .AsNoTracking(); 

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.ArtistName)
                .Include(a => a.AlbumTracks)
                .AsNoTracking() 
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "Id", "Name");
            return View();
        }

        // POST: Albums/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,YearPublished,Label,AlbumCover,ArtistId,priceVinyl")] Album album, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        album.AlbumCover = "data:image/jpeg;base64," + Convert.ToBase64String(memoryStream.ToArray());
                    }
                }

                album.Id = Guid.NewGuid();
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "Id", "Name", album.ArtistId);
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "Id", "Name", album.ArtistId);
            return View(album);
        }

        // POST: Albums/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,YearPublished,Label,AlbumCover,ArtistId,priceVinyl")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
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
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "Id", "Name", album.ArtistId);
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.ArtistName)
                .AsNoTracking() 
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var album = await _context.Album
                .Include(a => a.AlbumTracks)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            if (album.AlbumTracks != null && album.AlbumTracks.Any())
            {
                foreach (var track in album.AlbumTracks)
                {
                    track.AlbumId = null;
                }

                _context.Track.UpdateRange(album.AlbumTracks);
            }

            _context.Album.Remove(album);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Albums/CreateInvoice
        public async Task<FileContentResult> CreateInvoice(Guid albumId)
        {
            var album = await _context.Album
                .Include(a => a.ArtistName)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == albumId);

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{AlbumId}}", album.Id.ToString());
            document.Content.Replace("{{AlbumTitle}}", album.Title);
            document.Content.Replace("{{ArtistName}}", album.ArtistName?.Name ?? "Unknown Artist");
            document.Content.Replace("{{priceVinyl}}", album.priceVinyl.ToString("F2") + "$");

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "AlbumInvoice.pdf");
        }

        private bool AlbumExists(Guid id)
        {
            return _context.Album.Any(e => e.Id == id);
        }
    }
}
