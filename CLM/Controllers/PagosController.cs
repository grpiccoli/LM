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
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using CLM.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Globalization;
using CLM.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CLM.Controllers
{
    [Authorize(Policy = "CajaPago")]
    public class PagosController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PagosController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IServiceProvider serviceProvider,
            ITempDataProvider tempDataProvider,
            IRazorViewEngine viewEngine)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        // GET: Cajas
        public async Task<IActionResult> Index(int? pg, int? rpp, string srt,
            bool? asc, string val)
        {
            if (pg == null) pg = 1;
            if (rpp == null) rpp = 20;
            if (string.IsNullOrEmpty(srt)) srt = "Id";
            if (asc == null) asc = true;

            bool _asc = asc.Value;

            var pre = _context.Pago.Pre(val);
            var sort = _context.Pago.FilterSort(srt);
            ViewData = _context.Pago.ViewData(pre, pg, rpp, srt, asc, val);
            var Filters = ViewData["Filters"] as IDictionary<string, List<string>>;

            var applicationDbContext = _asc ? 
                pre
                .OrderBy(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Vales)
                .Include(c => c.Oficina)
                .Include(p => p.Cliente) : 
                pre
                .OrderByDescending(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Vales)
                .Include(c => c.Oficina)
                .Include(p => p.Cliente);

            ViewData[nameof(Medio)] = Medio.Cheque.Enum2Select(Filters);

            var clientSelect = new List<SelectListItem> { };
            foreach (var c in _context.Pago.Select(v => v.ClienteId))
            {
                var cliente = await _context.Cliente.SingleOrDefaultAsync(i => i.Id == c);
                var item = new SelectListItem
                {
                    Value = c.ToString(),
                    Text = string.Join(" / ",
                    cliente.NoCliente, cliente.Name,
                    string.Format(new InterceptProvider(), "{0:U}", c))
                };
                clientSelect.Add(item);
            }

            ViewData["ClienteId"] = new MultiSelectList(clientSelect, "Value", "Text");
            //ViewData["ClienteId"] = new MultiSelectList(
            //    from Cliente c in _context.Pago.Select(v => v.Cliente)
            //    .GroupBy(c => c.Id).Select(c => c.First())
            //    select new
            //    {
            //        c.Id,
            //        Name = String.Join(" / ",
            //        c.NoCliente, c.Name,
            //        String.Format(new InterceptProvider(), "{0:U}", c.Id))
            //    },
            //    "Id", "Name", Filters.ContainsKey("ClienteId") ?
            //    Filters["ClienteId"] : null);

            var oficinaSelect = new List<SelectListItem> { };
            foreach (var o in _context.Pago.Select(v => v.OficinaId).Distinct())
            {
                var oficina = await _context.Oficina.SingleOrDefaultAsync(i => i.Id == o);
                var item = new SelectListItem
                {
                    Value = o.ToString(),
                    Text = oficina.Name
                };
                oficinaSelect.Add(item);
            }

            ViewData["OficinaId"] = new MultiSelectList(oficinaSelect, "Value", "Text");

            ViewData["ApplicationUserId"] = new MultiSelectList(
                from ApplicationUser c in _context.Pago
                .Select(v => v.ApplicationUser).GroupBy(c => c.Id)
                .Select(c => c.First())
                select new
                { c.Id, Name = c.UserName },
                "Id", "Name", Filters.ContainsKey("ApplicationUserId") ?
                Filters["ApplicationUserId"] : null);

            ViewData["Date"] = string.Format("'{0}'",
                string.Join("','", _context.Pago.Select(v => v.Date.Date
                .ToString("yyyy-M-d")).Distinct().ToList()));

            return View(await applicationDbContext.ToListAsync());
        }

        //public async Task<IActionResult> Historial(int? id, int? pg, int? rpp, string srt, bool? asc)
        //{
        //    if (pg == null) pg = 1;
        //    if (rpp == null) rpp = 20;
        //    if (String.IsNullOrEmpty(srt)) srt = "Id";
        //    if (asc == null) asc = true;

        //    PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Pago)).Find(srt, false);
        //    if (rpp.HasValue) ViewData["last"] = (Math.Ceiling((double)(_context.Pago.Count() / rpp.Value))).ToString();

        //    var pre =
        //        id == null ?
        //        _context.Pago.Where(v => v.ClienteId == 0) :
        //        _context.Pago
        //        .Where(v => v.ClienteId == id.Value);

        //    var applicationDbContext = 
        //        asc.Value ?
        //        pre
        //        .OrderBy(x => prop.GetValue(x))
        //        .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
        //        .Include(c => c.ApplicationUser)
        //        .Include(c => c.Vales) : 
        //        pre
        //        .OrderByDescending(x => prop.GetValue(x))
        //        .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
        //        .Include(c => c.ApplicationUser)
        //        .Include(c => c.Vales);

        //    ViewData["srt"] = srt;
        //    ViewData["asc"] = asc.Value.ToString();
        //    ViewData["pg"] = Convert.ToString(pg.Value);
        //    ViewData["rpp"] = rpp.Value.ToString();

        //    var model = new HistorialPagoVM
        //    {
        //        Pagos = await applicationDbContext.ToListAsync(),
        //        ClienteId = id,
        //        ClientesList = new SelectList(
        //            from c in _context.Cliente
        //            select new { c.Id,
        //                Name = String.Join(" / ", c.NoCliente, c.Name, String.Format(new InterceptProvider(), "{0:U}", c.Id)) },
        //            "Id", "Name", id ?? 0)
        //    };

        //    if (id != null)
        //    {
        //        var honorarios = _context.Cobro
        //        .Where(v => v.Vale.ClienteId == id.Value && v.Vale.State == State.Pagado).Select(c => c.Honorario.ToString());
        //    }

        //    return View(model);
        //}

        // GET: Cajas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pago
                .Include(c => c.Vales)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Cajas/Create
        public IActionResult Create()
        {
            var model = new PagoViewModel { };
            model.Fecha = DateTime.Today.Date.ToString("dd-MM-yyyy");
            model.FechaTransferencia = model.Fecha;
            model.MediosList = new SelectList(from Medio m in Enum.GetValues(typeof(Medio))
                       select new { Id = m, Name = m.ToString() }, "Id", "Name");
            var oficinas = _context.ApplicationUser
                            .Include(u => u.PuestosTrabajo)
                            .ThenInclude(p => p.Oficina)
                            .Single(u => u.UserName == User.Identity.Name)
                            .PuestosTrabajo.Select(u => u.Oficina);
            model.OficinasList = new SelectList(from Oficina o in oficinas
                                                select new {
                                                    o.Id, o.Name
                                                }, "Id", "Name",
                                                oficinas.Count() == 1 ? 
                                                oficinas.First().Id.ToString() : null);
            model.AvailableDates = String.Format("'{0}'",
                String.Join("','",
                _context.Vale
                .Where(v => v.State == State.Activo || v.State == State.Pendiente)
                .Select(v => v.Date.Date.ToString("yyyy-M-d")).Distinct().ToList()));
            return View(model);
        }

        // POST: Cajas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fecha,ValeId,Medio,Monto,FechaTransferencia,RUT,ClienteId,Saldo,OficinaId")] PagoViewModel pago)
        {
            if (ModelState.IsValid && !String.IsNullOrEmpty(pago.Monto))
            {
                var user = await _userManager.GetUserAsync(User);

                var plata = Convert.ToInt32(Regex.Replace(pago.Monto, "[^0-9]", ""));

                var cliente = _context.Cliente
                    .Include(c => c.Vales)
                    .ThenInclude(v => v.Cobros)
                    .Single(c => c.Id == pago.ClienteId);

                var pagado = new Pago
                {
                    Date = DateTime.Parse(pago.Fecha).Date,
                    ApplicationUserId = user.Id,
                    ClienteId = pago.ClienteId,
                    Medio = pago.Medio,
                    Monto = plata,
                    Cliente = cliente,
                    OficinaId = pago.OficinaId,
                    Oficina = await _context.Oficina.SingleOrDefaultAsync(o => o.Id == pago.OficinaId)
                };

                if(pago.Medio == Medio.Transferencia)
                {
                    pagado.FechaTransferencia = DateTime.Parse(pago.FechaTransferencia).Date;
                    pagado.RutTransferencia = Convert.ToInt32(String.Format(new InterceptProvider(), "{0:I}", pago.RUT));
                }

                _context.Pago.Add(pagado);

                var vales = cliente.Vales.Where(v => v.State == State.Activo || v.State == State.Pendiente).OrderBy(v => v.Date);

                foreach (var v in vales)
                {
                    var valePago = new ValePago
                    {
                        PagoId = pagado.Id,
                        ValeId = v.Id
                    };
                    _context.ValePagos.Add(valePago);

                    var remanente = v.SubTotal - v.Pagado;
                    if(plata > remanente)
                    {
                        plata -= remanente;
                        v.Pagado = v.SubTotal;
                        v.State = State.Pagado;
                        _context.Vale.Update(v);
                    }
                    else if(plata == remanente)
                    {
                        v.Pagado = v.SubTotal;
                        v.State = State.Pagado;
                        _context.Vale.Update(v);
                        break;
                    }
                    else if(plata < remanente)
                    {
                        v.Pagado += plata;
                        v.State = State.Pendiente;
                        _context.Vale.Update(v);
                        break;
                    }
                }

                var saldo = Convert.ToDouble(Regex.Replace(pago.Saldo, "[^0-9-]", ""));

                var vale = _context.Vale.Single(v => v.Id == pago.ValeId);

                _context.Vale.Update(vale);

                if (saldo == cliente.SaldosPendientes - pagado.Monto)
                {
                    cliente.SaldosPendientes = Convert.ToInt32(Regex.Replace(pago.Saldo, "[^0-9-]", ""));
                    _context.Cliente.Update(cliente);
                    await _context.SaveChangesAsync();

                    var cobros = vales
                        .SelectMany(v => v.Cobros)
                        .Select(c => $@"<tr><td>{typeof(Honorario)
                            .GetMember(c.Honorario.ToString())
                            .FirstOrDefault()
                            .GetCustomAttribute<DisplayAttribute>(false)
                            .Name}</td><td><i class=""fas fa-dollar-sign""></i></td><td style=""text-align:right"">" +
                            c.Monto.ToString("C", CultureInfo.GetCultureInfo("es-CL")) + "</td></tr>");

                    //var cobrosList = new List<string> { };

                    //foreach(var c in cobros)
                    //{
                    //    cobrosList.Add($@"<tr><td>{typeof(Honorario)
                    //                  .GetMember(c.Honorario.ToString())
                    //                  .FirstOrDefault()
                    //                  .GetCustomAttribute<DisplayAttribute>(false)
                    //                  .Name}</td><td><i class=""fas fa-dollar-sign""></i></td><td style=""text-align:right"">" +
                    //                  c.Monto.ToString("C", CultureInfo.GetCultureInfo("es-CL")) + "</td></tr>");
                    //}

                    var dlEmail = new BoletaEmail
                    {
                        Id = pagado.Id,
                        Fecha = pagado.Date.ToString("dd-MM-yyyy"),
                        Cobros = String.Join("",cobros),
                        Emitidos = "",
                        Saldos = "",
                        Total = pagado.Monto.ToString("C", CultureInfo.GetCultureInfo("es-CL")),
                    };

                    var renderer = new RazorViewToStringRenderer(_viewEngine, _tempDataProvider, _serviceProvider);

                    var result = renderer.RenderViewToString("_BoletaEmail", dlEmail).GetAwaiter().GetResult();

                    await _emailSender.SendEmailAsync(user.Email, $@"BOLETA {pagado.Id}", result);

                    return RedirectToAction(nameof(Index));
                }

                return NotFound();
            }
            var model = new PagoViewModel { };
            model.Fecha = DateTime.Today.Date.ToString("dd-MM-yyyy");
            model.FechaTransferencia = model.Fecha;
            model.MediosList = new SelectList(from Medio m in Enum.GetValues(typeof(Medio))
                                              select new { Id = m, Name = m.ToString() }, "Id", "Name");
            model.AvailableDates = String.Format("'{0}'",
                String.Join("','",
                _context.Vale
                .Where(v => v.State == State.Activo || v.State == State.Pendiente)
                .Select(v => v.Date.Date.ToString("yyyy-M-d")).Distinct().ToList()));
            return View(model);
        }

        //// GET: Cajas/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var pago = await _context.Pago.SingleOrDefaultAsync(m => m.Id == id);
        //    if (pago == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ValeId"] = new SelectList(_context.Vale, "Id", "Id", pago.ValeId);
        //    return View(pago);
        //}

        //// POST: Cajas/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,ValeId,ApplicationUserId,Pago,Medio,FechaTransferencia,RutTransferencia")] Pago pago)
        //{
        //    if (id != pago.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(pago);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CajaExists(pago.Id))
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
        //    ViewData["ValeId"] = new SelectList(_context.Vale, "Id", "Id", pago.ValeId);
        //    return View(pago);
        //}

        public IActionResult Boleta()
        {
            return PartialView("_BoletaEmail",
                new BoletaEmail
                {
                    Cobros = $@"<tr><td>Impuestos renta</td><td><i class=""fas fa-dollar-sign""></i></td><td style=""text-align:right"">$10.000</td></tr>",
                    Emitidos ="",
                    Fecha ="25-05-2018",
                    Id =1,
                    Saldos ="0",
                    Total ="$10.000"
                });
        }

        public ActionResult GetValesByDate(string date)
        {
            var dt = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var vales = _context.Vale
                .Include(v => v.Cliente)
                .Include(v => v.ApplicationUser)
                .Where(v => v.Date.Date == dt.Date && (v.State == State.Activo || v.State == State.Pendiente));
            var select = new SelectList(
                from Vale v in vales
                select new { v.Id, Name = String.Join(" / ", v.Id, "CLTE:"+v.Cliente.NoCliente+'-'+v.Cliente.Name + '-' + String.Format(new InterceptProvider(), "{0:U}", v.Cliente.Id), "EMISOR:"+v.ApplicationUser.UserName) },
                "Id", "Name");
            return Json(select);
        }

        public async Task<ActionResult> GetValeById(int id)
        {
            var vale = await _context.Vale
                .Include(v => v.Cliente)
                .Include(v => v.Cobros)
                .SingleAsync(v => v.Id == id);

            var encolados = _context.Vale
                .Where(v => v.ClienteId == vale.ClienteId && v.State == State.Pendiente);

            var model = new
            {
                Cobros = (from Cobro c in vale.Cobros
                          select new
                          {
                              Id = typeof(Honorario).GetMember(c.Honorario.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).Name,
                              Name = c.Monto
                          }).ToDictionary(key => key.Id, value => value.Name),
                vale.SubTotal,
                vale.ClienteId,
                SaldosPendientes = vale.Cliente.SaldosPendientes - vale.SubTotal,
                Total = vale.Cliente.SaldosPendientes,
                ValesEmitidos = (from Vale v in encolados
                                 select new
                                 {
                                     v.Id
                                 }).ToList()
            };
            return Json(model);
        }

        //// GET: Cajas/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var pago = await _context.Pago
        //        .Include(c => c.Vales)
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (pago == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(pago);
        //}

        //// POST: Cajas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var pago = await _context.Pago.SingleOrDefaultAsync(m => m.Id == id);
        //    _context.Pago.Remove(pago);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CajaExists(int id)
        //{
        //    return _context.Pago.Any(e => e.Id == id);
        //}
    }
}
