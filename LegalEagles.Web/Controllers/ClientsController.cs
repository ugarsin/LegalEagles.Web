using LegalEagles.Web.Data;
using LegalEagles.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalEagles.Web.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 8;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageSize, int page = 1)
        {
            if (pageSize <= 5) 
            {
                pageSize = PageSize;
            }

            var totalClients = await _context.Clients.CountAsync();

            var clients = await _context.Clients
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalClients / (double)pageSize);

            return View(clients);
        }
        
        // ===============================
        // GET: Clients/Create
        // ===============================
        public IActionResult Create()
        {
            return View();
        }

        // ===============================
        // POST: Clients/Create
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // ===============================
        // GET: Clients/Edit/5
        // ===============================
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        // ===============================
        // POST: Clients/Edit/5
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(client);

            _context.Update(client);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // POST: Clients/Delete/5
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
