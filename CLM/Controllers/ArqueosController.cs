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
using CLM.Models.ViewModels;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using CLM.Extensions;
using System.Text.RegularExpressions;

namespace CLM.Controllers
{
    [Authorize(Policy = "Arqueo")]
    public class ArqueosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArqueosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Arqueos
        public IActionResult Index(int? pg, int? rpp, string srt,
            bool? asc, string val)
        {
            if (pg == null) pg = 1;
            if (rpp == null) rpp = 20;
            if (String.IsNullOrEmpty(srt)) srt = "Date";
            if (asc == null) asc = false;

            bool _asc = asc.Value;

            var pre = _context.Arqueo.Pre(val);
            var sort = _context.Arqueo.FilterSort(srt);
            ViewData = _context.Arqueo.ViewData(pre, pg, rpp, srt, asc, val);
            var Filters = ViewData["Filters"] as IDictionary<string, List<string>>;

            var arqueos = _asc ?
                pre
                .OrderBy(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.ApplicationUser) :
                pre
                .OrderByDescending(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.ApplicationUser);

            var model = from Arqueo g in arqueos
                        select new ArqueoVM
                        {
                            OficinaId = g.OficinaId,
                            OfficeName = g.Oficina.Name,
                            ApplicationUserId = g.ApplicationUserId,
                            ApplicationUserName = g.ApplicationUser.UserName,
                            Fecha = g.Date.Date,
                            PagosEfectivo = _context.Pago.Where(p => p.Date.Date == g.Date.Date && p.Medio == Medio.Efectivo && p.OficinaId == g.OficinaId).Select(p => p.Monto).Sum(),
                            PagosCheque = _context.Pago.Where(p => p.Date.Date == g.Date.Date && p.Medio == Medio.Cheque && p.OficinaId == g.OficinaId).Select(p => p.Monto).Sum(),
                            PagosTransferencia = _context.Pago.Where(p => p.Date.Date == g.Date.Date && p.Medio == Medio.Transferencia && p.OficinaId == g.OficinaId).Select(p => p.Monto).Sum(),
                            PagosTotal = _context.Pago.Where(p => p.Date.Date == g.Date.Date && p.OficinaId == g.OficinaId).Select(p => p.Monto).Sum(),
                            RetiroEfectivo = _context.Retiro.Where(p => p.Date.Date == g.Date.Date && p.Forma == Forma.Efectivo && p.OficinaId == g.OficinaId).Select(p => p.Monto).Sum(),
                            RetiroCheque = _context.Retiro.Where(p => p.Date.Date == g.Date.Date && p.Forma == Forma.Cheque && p.OficinaId == g.OficinaId).Select(p => p.Monto).Sum(),
                            RetiroTotal = _context.Retiro.Where(p => p.Date.Date == g.Date.Date && p.OficinaId == g.OficinaId).Select(p => p.Monto).Sum(),
                            SaldoAnterior = _context.Arqueo.Where(a => a.Date < g.Date.Date && a.OficinaId == g.OficinaId).OrderByDescending(t => t.Date).FirstOrDefault().Saldo,
                            ArqueoCheques = g.Cheque,
                            ArqueoEfectivo = g.Efectivo,
                            ArqueoTransferencias = g.Transferencia,
                            ArqueoTotal = g.ResultadoEjercicio
                        };

            foreach (ArqueoVM m in model as List<ArqueoVM>)
            {
                m.DiferenciaEfectivo = m.ArqueoEfectivo - (m.SaldoAnterior + m.PagosEfectivo - m.RetiroEfectivo);
                m.DiferenciaCheques = m.ArqueoCheques - (m.PagosCheque - m.RetiroCheque);
                m.DiferenciaTransferencias = m.ArqueoTransferencias - m.PagosTransferencia;
                m.DiferenciaTotal = m.DiferenciaEfectivo + m.DiferenciaCheques + m.DiferenciaTransferencias;
            }

            var contextUser = _context.ApplicationUser
                .Include(u => u.PuestosTrabajo)
                .ThenInclude(p => p.Oficina)
                .SingleOrDefault(u => u.UserName == User.Identity.Name);

            ViewData["Oficinas"] = contextUser.PuestosTrabajo.Select(o => o.Oficina.Name).ToList();

            ViewData["ApplicationUserId"] = new MultiSelectList(
                from ApplicationUser c in _context.Arqueo
                .Select(v => v.ApplicationUser).GroupBy(c => c.Id)
                .Select(c => c.First())
                select new
                { c.Id, Name = c.UserName },
                "Id", "Name", Filters.ContainsKey("ApplicationUserId") ?
                Filters["ApplicationUserId"] : null);

            ViewData["Date"] = String.Format("'{0}'",
                String.Join("','", _context.Arqueo.Select(v => v.Date.Date
                .ToString("yyyy-M-d")).Distinct().ToList()));

            return View(model);
        }

        // GET: Arqueos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arqueo = await _context.Arqueo
                .SingleOrDefaultAsync(m => m.Id == id);
            if (arqueo == null)
            {
                return NotFound();
            }

            return View(arqueo);
        }

        // GET: Arqueos/Create
        public IActionResult Create()
        {
            var model = new CreateArqueoVM { };
            model.Fecha = DateTime.Today.Date.ToString("dd-MM-yyyy");
            var oficinas = _context.ApplicationUser
                        .Include(u => u.PuestosTrabajo)
                        .ThenInclude(p => p.Oficina)
                        .Single(u => u.UserName == User.Identity.Name)
                        .PuestosTrabajo.Select(u => u.Oficina);
            model.OficinasList = new SelectList(
                from Oficina o in oficinas
                select new
                {
                    o.Id,
                    o.Name
                }, "Id", "Name",
                oficinas.Count() == 1 ?
                oficinas.First().Id.ToString() : null);
            model.FechaList = String.Format("'{0}'",
                String.Join("','",
                _context.Arqueo
                .Select(v => v.Date.Date.ToString("yyyy-M-d")).Distinct().ToList()));

            return View(model);
        }

        // POST: Arqueos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fecha,OficinaId,Efectivo,Transferencia,Cheque")] CreateArqueoVM arqueo)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var date = DateTime.Parse(arqueo.Fecha).Date;
                var model = new Arqueo
                {
                    ResultadoEjercicio = 
                    _context.Pago
                    .Where(p => p.Date.Date == date)
                    .Select(p => p.Monto).Sum()
                    - _context.Retiro.Where(p => p.Date.Date == date).Select(p => p.Monto).Sum(),
                    ApplicationUserId = user.Id,
                    Cheque = Convert.ToInt32(Regex.Replace(arqueo.Cheque, "[^0-9]", "")),
                    Efectivo = Convert.ToInt32(Regex.Replace(arqueo.Efectivo, "[^0-9]", "")),
                    Date = date,
                    OficinaId = arqueo.OficinaId,
                    Transferencia = Convert.ToInt32(Regex.Replace(arqueo.Transferencia, "[^0-9]", ""))
                };
                model.Saldo = model.Transferencia + model.Efectivo + model.Cheque;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(arqueo);
        }

        // GET: Arqueos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arqueo = await _context.Arqueo.SingleOrDefaultAsync(m => m.Id == id);
            if (arqueo == null)
            {
                return NotFound();
            }
            return View(arqueo);
        }

        // POST: Arqueos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,ApplicationUserId,DifEfectivo,DifTransferencia,DifCheque")] Arqueo arqueo)
        {
            if (id != arqueo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arqueo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArqueoExists(arqueo.Id))
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
            return View(arqueo);
        }

        // GET: Arqueos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arqueo = await _context.Arqueo
                .SingleOrDefaultAsync(m => m.Id == id);
            if (arqueo == null)
            {
                return NotFound();
            }

            return View(arqueo);
        }

        // POST: Arqueos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var arqueo = await _context.Arqueo.SingleOrDefaultAsync(m => m.Id == id);
            _context.Arqueo.Remove(arqueo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArqueoExists(int id)
        {
            return _context.Arqueo.Any(e => e.Id == id);
        }
    }
}
