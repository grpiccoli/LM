using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLM.Data;
using CLM.Models;
using CLM.Services;
using CLM.Extensions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CLM.Controllers
{
    [Authorize(Policy = "Vale")]
    public class ValesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public ValesController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Vales
        public async Task<IActionResult> Index(int? pg, int? rpp, string srt,
            bool? asc, string val)
        {
            if (pg == null) pg = 1;
            if (rpp == null) rpp = 20;
            if (String.IsNullOrEmpty(srt)) srt = "Date";
            if (asc == null) asc = false;

            bool _asc = asc.Value;

            var pre = _context.Vale.Pre(val);
            var sort = _context.Vale.FilterSort(srt);
            ViewData = _context.Vale.ViewData(pre, pg, rpp, srt, asc, val);
            var Filters = ViewData["Filters"] as IDictionary<string, List<string>>;

            var applicationDbContext = _asc ?
                pre
                .OrderBy(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Pagos)
                .Include(c => c.Cliente) : 
                pre
                .OrderByDescending(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Pagos)
                .Include(g => g.Cliente);
            
            ViewData[nameof(State)] = State.Activo.Enum2Select(Filters);

            ViewData["ClienteId"] = new MultiSelectList(
                from Cliente c in _context.Vale.Select(v => v.Cliente).GroupBy(c => c.Id).Select(c => c.First())
                select new
                {
                    c.Id,
                    Name = String.Join(" / ", c.NoCliente, c.Name, String.Format(new InterceptProvider(), "{0:U}", c.Id))
                },
                "Id", "Name", Filters.ContainsKey("ClienteId") ? Filters["ClienteId"] : null);

            ViewData["ApplicationUserId"] = new MultiSelectList(
                from ApplicationUser c in _context.Vale.Select(v => v.ApplicationUser).GroupBy(c => c.Id).Select(c => c.First())
                select new
                { c.Id, Name = c.UserName },
                "Id", "Name", Filters.ContainsKey("ApplicationUserId") ? Filters["ApplicationUserId"] : null);

            ViewData["Date"] = String.Format("'{0}'", 
                String.Join("','", _context.Vale.Select(v => v.Date.Date.ToString("yyyy-M-d")).Distinct().ToList()));

            return View(await applicationDbContext.ToListAsync());
        }

        //public async Task<IActionResult> Historial(int? id, int? pg, int? rpp, string srt, bool? asc)
        //{
        //    if (pg == null) pg = 1;
        //    if (rpp == null) rpp = 20;
        //    if (String.IsNullOrEmpty(srt)) srt = "Id";
        //    if (asc == null) asc = true;

        //    PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Vale)).Find(srt, false);
        //    if (rpp.HasValue) ViewData["last"] = (Math.Ceiling((double)(_context.Vale.Count() / rpp.Value))).ToString();

        //    var pre = 
        //        id == null ?
        //        _context.Vale :
        //        _context.Vale
        //        .Where(v => v.ClienteId == id.Value);

        //    var applicationDbContext = 
        //        asc.Value ? 
        //        pre
        //        .OrderBy(x => prop.GetValue(x))
        //        .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
        //        .Include(c => c.ApplicationUser)
        //        .Include(c => c.Pagos)
        //        .Include(c => c.Cliente) : 
        //        pre
        //        .OrderByDescending(x => prop.GetValue(x))
        //        .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
        //        .Include(c => c.ApplicationUser)
        //        .Include(c => c.Pagos)
        //        .Include(g => g.Cliente);

        //    ViewData["srt"] = srt;
        //    ViewData["asc"] = asc.Value.ToString();
        //    ViewData["pg"] = Convert.ToString(pg.Value);
        //    ViewData["rpp"] = rpp.Value.ToString();

        //    var model = new HistorialValeVM
        //    {
        //        Vales = await applicationDbContext.ToListAsync(),
        //        ClienteId = id,
        //        ClientesList = new SelectList(
        //            from c in _context.Cliente
        //            select new { c.Id,
        //                Name = String.Join(" / ", c.NoCliente, c.Name, String.Format(new InterceptProvider(), "{0:U}", c.Id)) }, 
        //            "Id", "Name", id ?? 0)
        //    };

        //    return View(model);
        //}

        // GET: Vales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vale = await _context.Vale
                .Include(v => v.Cliente)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vale == null)
            {
                return NotFound();
            }

            return View(vale);
        }

        // GET: Vales/Create
        public IActionResult Create()
        {
            var clientes = from Cliente c in _context.Cliente
                       select new { c.Id, Name = String.Join(" / ",c.NoCliente,c.Name, String.Format(new InterceptProvider(), "{0:U}", c.Id))};
            var honorario = from Honorario h in Enum.GetValues(typeof(Honorario)).Cast<Honorario>().Where(h => h != Honorario.SaldoInicial)
                       select new {
                           Id = h,
                           h.GetType().GetMember(h.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).Name,
                           h.GetType().GetMember(h.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).GroupName
                       };
            //https://www.google.com/url?q=https://stackoverflow.com/questions/36343620/selectlist-with-selectlistgroup&sa=D&ust=1517165157558000&usg=AFQjCNEno9tPOiWA4l7nPN0afQZRgOwagg
            var initialModel = new ValeViewModel {
                ClientesList = new SelectList(clientes, "Id", "Name"),
                HonorariosList = new List<SelectListItem> { }
            };

            foreach(var group in honorario.GroupBy(h => h.GroupName))
            {
                var optionGroup = new SelectListGroup() { Name = group.Key };
                foreach(var hon in group)
                {
                    initialModel.HonorariosList.Add(new SelectListItem() { Value = hon.Id.ToString(), Text = hon.Name, Group = optionGroup });
                }
            }
            return View(initialModel);
        }

        // POST: Vales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,Cobros,SaldosPendientes,SubTotal,Total")] ValeViewModel vale)
        {
            if (ModelState.IsValid)
            {
                var model = new Vale
                {
                    ClienteId = vale.ClienteId,
                    Date= DateTime.Now,
                    SubTotal = Convert.ToInt32(Regex.Replace(vale.SubTotal.ToString(), "[^0-9]", "")),
                    ApplicationUserId = _userManager.GetUserId(User),
                    Cobros = new List<Cobro> { }
                };

                foreach (var cobro in vale.Cobros)
                {
                    if (String.IsNullOrEmpty(cobro.Monto)) continue;
                    var monto = Convert.ToInt32(Regex.Replace(cobro.Monto, "[^0-9]", ""));
                    //var hon = (Honorario)Enum.Parse(typeof(Honorario), cobro.Honorario);
                    //model.Cobros.Add(new Cobro { Monto = monto, Honorario = hon });
                    model.Cobros.Add(
                        new Cobro
                        {
                            Monto = monto,
                            Honorario = cobro.Honorario
                        });
                }

                var cliente = await _context.Cliente
                    .Include(c => c.Vales)
                    .SingleAsync(c => c.Id == model.ClienteId);
                cliente.SaldosPendientes += model.SubTotal;

                var last = cliente.Vales.SingleOrDefault(v => v.State == State.Activo);

                if (last != null)
                {
                    last.State = State.Pendiente;
                    _context.Vale.Update(last);
                }

                model.State = State.Activo;

                _context.Vale.Add(model);

                _context.Cliente.Update(cliente);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            var clientes = from Cliente c in _context.Cliente
                           select new { c.Id, Name = String.Join(" / ", c.NoCliente, c.Name, String.Format(new InterceptProvider(), "{0:U}", c.Id)) };
            var honorario = from Honorario h in Enum.GetValues(typeof(Honorario)).Cast<Honorario>().Where(h => h != Honorario.SaldoInicial)
                            select new
                            {
                                Id = h,
                                h.GetType().GetMember(h.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).Name,
                                h.GetType().GetMember(h.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).GroupName
                            };
            var initialModel = new ValeViewModel
            {
                ClientesList = new SelectList(clientes, "Id", "Name"),
                HonorariosList = new List<SelectListItem> { }
            };

            foreach (var group in honorario.GroupBy(h => h.GroupName))
            {
                var optionGroup = new SelectListGroup() { Name = group.Key };
                foreach (var hon in group)
                {
                    initialModel.HonorariosList.Add(new SelectListItem() { Value = hon.Id.ToString(), Text = hon.Name, Group = optionGroup });
                }
            }

            return View(initialModel);
        }

        public async Task<IActionResult> ViewCobros(int id)
        {
            var model = await _context.Vale
                .Include(v => v.Cobros)
                .Include(v => v.Cliente)
                .Include(v => v.ApplicationUser)
                .SingleAsync(v => v.Id == id);
            return PartialView("_ViewCobros", model);
        }

        public async Task<ActionResult> GetPagosCliente(int id, string name)
        {
            var model = await _context.Cliente.SingleAsync(c => c.Id == id);
            return Json(new
            {
                monto = typeof(Cliente).GetProperty(name).GetValue(model)
            });
        }

        [HttpPost]
        public async Task<IActionResult> ValeCliente(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var clientes = from Cliente c in _context.Cliente
                           select new { c.Id, Name = String.Join(" / ", c.NoCliente, c.Name, String.Format(new InterceptProvider(), "{0:U}", c.Id)) };

            var honorario = from Honorario h in Enum.GetValues(typeof(Honorario)).Cast<Honorario>().Where(h => h != Honorario.SaldoInicial)
                            select new
                            {
                                Id = h,
                                h.GetType().GetMember(h.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).Name,
                                h.GetType().GetMember(h.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).GroupName
                            };

            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Id == id);

            var model = new ValeViewModel
            {
                ClienteId = id.Value,
                ClientesList = new SelectList(clientes, "Id", "Name"),
                //SaldosList = new SelectList(_context.Cliente, "Id", "SaldosPendientes"),
                //HonorariosList = new SelectList(honorario, "Id", "Name"),
                HonorariosId = new List<int>
                {
                    (int)Honorario.Mensuales,
                    (int)Honorario.Laborales,
                    (int)Honorario.Renta,
                    (int)Honorario.Retención
                }
            };
            return View("Create", model);
        }

        [HttpGet]
        public IActionResult Exito(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = _context.Vale
                .Include(v => v.Cliente)
                .Include(v => v.ApplicationUser)
                .FirstOrDefault(v => v.Id == id);
            return PartialView("_Exito", model);
        }

        //// GET: Vales/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var vale = await _context.Vale.SingleOrDefaultAsync(m => m.Id == id);
        //    if (vale == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Id", vale.ClienteId);
        //    return View(vale);
        //}

        //// POST: Vales/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId")] Vale vale)
        //{
        //    if (id != vale.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(vale);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ValeExists(vale.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Id", vale.ClienteId);
        //    return View(vale);
        //}

        // GET: Vales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vale = await _context.Vale
                .Include(v => v.Cliente)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vale == null || vale.Pagos != null )
            {
                return NotFound();
            }

            return PartialView("_Delete",vale);
        }

        // POST: Vales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vale = await _context.Vale.SingleAsync(m => m.Id == id);

            var userId = _userManager.GetUserId(User);

            if (vale.State == State.Pagado || userId != vale.ApplicationUserId)
            {
                return NotFound();
            }

            var anterior = await _context.Vale
                .Where(v => v.State == State.Pendiente)
                .OrderByDescending(v => v.Date).FirstOrDefaultAsync();

            if(anterior != null)
            {
                anterior.State = State.Activo;
                _context.Vale.Update(anterior);
            }

            vale.State = State.Anulado;

            var cliente = await _context.Cliente.SingleAsync(c => c.Id == vale.ClienteId);
            cliente.SaldosPendientes -= vale.SubTotal;
            _context.Vale.Update(vale);
            _context.Cliente.Update(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ValeExists(int id)
        {
            return _context.Vale.Any(e => e.Id == id);
        }
    }
}
