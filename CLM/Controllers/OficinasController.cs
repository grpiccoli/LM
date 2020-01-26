using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CLM.Data;
using CLM.Models;
using Microsoft.AspNetCore.Authorization;

namespace CLM.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class OficinasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OficinasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Oficinas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Oficina.ToListAsync());
        }

        // GET: Oficinas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oficina = await _context.Oficina
                .SingleOrDefaultAsync(m => m.Id == id);
            if (oficina == null)
            {
                return NotFound();
            }

            return View(oficina);
        }

        // GET: Oficinas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Oficinas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address")] Oficina oficina)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oficina);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(oficina);
        }

        // GET: Oficinas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oficina = await _context.Oficina.SingleOrDefaultAsync(m => m.Id == id);
            if (oficina == null)
            {
                return NotFound();
            }
            return View(oficina);
        }

        // POST: Oficinas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Oficina oficina)
        {
            if (id != oficina.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oficina);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OficinaExists(oficina.Id))
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
            return View(oficina);
        }

        // GET: Oficinas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oficina = await _context.Oficina
                .SingleOrDefaultAsync(m => m.Id == id);
            if (oficina == null)
            {
                return NotFound();
            }

            return View(oficina);
        }

        // POST: Oficinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oficina = await _context.Oficina.SingleOrDefaultAsync(m => m.Id == id);
            _context.Oficina.Remove(oficina);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OficinaExists(int id)
        {
            return _context.Oficina.Any(e => e.Id == id);
        }
    }
}
