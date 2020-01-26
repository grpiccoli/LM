using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLM.Data;
using CLM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using CLM.Extensions;
using CLM.Models.ViewModels;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CLM.Controllers
{
    [Authorize(Policy = "Retiros")]
    public class RetirosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RetirosController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Retiros
        public async Task<IActionResult> Index(int? pg, int? rpp, string srt,
            bool? asc, string val)
        {
            if (pg == null) pg = 1;
            if (rpp == null) rpp = 20;
            if (String.IsNullOrEmpty(srt)) srt = "Id";
            if (asc == null) asc = true;

            bool _asc = asc.Value;

            var pre = _context.Retiro.Pre(val);
            var sort = _context.Retiro.FilterSort(srt);
            ViewData = _context.Retiro.ViewData(pre, pg, rpp, srt, asc, val);
            var Filters = ViewData["Filters"] as IDictionary<string, List<string>>;

            var applicationDbContext =
                _asc ?
                pre
                .OrderBy(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Oficina):
                pre
                .OrderByDescending(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Oficina);

            ViewData[nameof(Forma)] = Forma.Cheque.Enum2Select(Filters);

            ViewData["OficinaId"] = new MultiSelectList(
                from Oficina o in _context.Oficina
                select new
                { o.Id, o.Name },
                "Id", "Name", Filters.ContainsKey("OficinaId") ? Filters["OficinaId"] : null);

            ViewData["ApplicationUserId"] = new MultiSelectList(
                from ApplicationUser c in _context.Pago
                .Select(v => v.ApplicationUser).GroupBy(c => c.Id).Select(c => c.First())
                select new
                { c.Id, Name = c.UserName },
                "Id", "Name", Filters.ContainsKey("ApplicationUserId") ? Filters["ApplicationUserId"] : null);

            ViewData["Date"] = String.Format("'{0}'",
                String.Join("','", _context.Pago.Select(v => v.Date.Date.ToString("yyyy-M-d")).Distinct().ToList()));

            return View(await _context.Retiro.ToListAsync());
        }

        // GET: Retiros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retiro = await _context.Retiro
                .SingleOrDefaultAsync(m => m.Id == id);
            if (retiro == null)
            {
                return NotFound();
            }

            return View(retiro);
        }

        // GET: Retiros/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            var model = new RetiroVM { DisponibleVMs = new List<DisponibleVM> { } };
            //{
            foreach(int id in _context.Oficina.Select(o => o.Id))
            {
                var disponible = new DisponibleVM {
                    OficinaId = id,
                    Cheque = (_context.Pago
            .Where(p => p.Date.Date == DateTime.Now.Date &&
            p.Medio == Medio.Cheque)
            .Select(p => p.Monto).Sum() -
            _context.Retiro.Where(p => p.Date.Date == DateTime.Now.Date &&
            p.Forma == Forma.Cheque)
            .Select(p => p.Monto).Sum()).ToString("C", CultureInfo.CreateSpecificCulture("es-CL")),
                    Efectivo = (_context.Pago
            .Where(p => p.Date.Date == DateTime.Now.Date &&
            p.Medio == Medio.Efectivo)
            .Select(p => p.Monto).Sum() -
            _context.Retiro.Where(p => p.Date.Date == DateTime.Now.Date &&
            p.Forma == Forma.Efectivo)
            .Select(p => p.Monto).Sum()).ToString("C", CultureInfo.CreateSpecificCulture("es-CL"))
                };

                model.DisponibleVMs.Add(disponible);
            }

            model.FormaList = Forma.Cheque.Enum2Select(null);

            model.Forma = null;
            //};
            model.OficinaList = new MultiSelectList(
                from Oficina o in _context.Oficina
                select new
                { o.Id, o.Name },
                "Id", "Name", null);

            return View(model);
        }

        // POST: Retiros/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Monto,Detalles,Forma,OficinaId")] RetiroVM retiro)
        {
            if (ModelState.IsValid)
            {
                var model = new Retiro
                {
                    Date = DateTime.Now,
                    ApplicationUserId = (await _userManager.GetUserAsync(User)).Id,
                    Monto = Convert.ToInt32(Regex.Replace(retiro.Monto, "[^0-9]", "")),
                    Forma = retiro.Forma.Value,
                    Detalles = retiro.Detalles,
                    OficinaId = retiro.OficinaId
                };
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(retiro);
        }

        // GET: Retiros/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retiro = await _context.Retiro.SingleOrDefaultAsync(m => m.Id == id);
            if (retiro == null)
            {
                return NotFound();
            }
            return View(retiro);
        }

        // POST: Retiros/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Date,Monto,Detalles")] Retiro retiro)
        {
            if (id != retiro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(retiro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RetiroExists(retiro.Id))
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
            return View(retiro);
        }

        // GET: Retiros/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retiro = await _context.Retiro
                .SingleOrDefaultAsync(m => m.Id == id);
            if (retiro == null)
            {
                return NotFound();
            }

            return View(retiro);
        }

        // POST: Retiros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var retiro = await _context.Retiro.SingleOrDefaultAsync(m => m.Id == id);
            _context.Retiro.Remove(retiro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RetiroExists(int id)
        {
            return _context.Retiro.Any(e => e.Id == id);
        }
    }
}
