using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLM.Data;
using CLM.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CLM.Services;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using CLM.Models.ViewModels;
using CLM.Extensions;

namespace CLM.Controllers
{
    [Authorize(Policy = "Clientes")]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientesController(
            UserManager<ApplicationUser> userManager, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(int? pg, int? rpp, string srt,
            bool? asc, bool? saldo, string val)
        {
            if (pg == null) pg = 1;
            if (rpp == null) rpp = 20;
            if (String.IsNullOrEmpty(srt)) srt = "Id";
            if (asc == null) asc = true;
            if (saldo == null) saldo = false;

            bool _asc = asc.Value;

            var pos = saldo.Value ? 1 : 0;

            var pre = _context.Cliente.Pre(val);
            var sort = _context.Cliente.FilterSort(srt);
            ViewData = _context.Cliente.ViewData(pre, pg, rpp, srt, asc, val);
            var Filters = ViewData["Filters"] as IDictionary<string, List<string>>;

            var applicationDbContext = _asc ?
                pre
                .Where(c => c.SaldosPendientes >= pos)
                .OrderBy(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.Comuna)
                .Include(c => c.Vales)
                .Include(c => c.GirosCliente)
                .ThenInclude(g => g.Giro) :
                pre
                .Where(c => c.SaldosPendientes >= pos)
                .OrderByDescending(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.Comuna)
                .Include(c => c.Vales)
                .Include(c => c.GirosCliente)
                .ThenInclude(g => g.Giro);

            ViewData["saldo"] = saldo.Value.ToString();
            ViewData[nameof(Tipo)] = Tipo.juridica.Enum2Select(Filters, "Name");

            ViewData["Comuna"] = new MultiSelectList(
                from Comuna c in _context.Cliente.Select(v => v.Comuna).GroupBy(c => c.Id).Select(c => c.First())
                select new
                {
                    c.Id,
                    c.Name
                },
                "Id", "Name",
                Filters.ContainsKey("Comuna") ? Filters["Comuna"] : null);

            ViewData["FechaIngreso"] = string.Format("'{0}'",
                string.Join("','", _context.Cliente.Select(v => v.FechaIngreso.Date.ToString("yyyy-M-d")).Distinct().ToList()));

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var model = new HistorialPagoVM();

            if (id.HasValue)
            {
                model.ClienteId = id.Value;
                model.ClientesList = new SelectList(
                    from c in _context.Cliente
                    select new {
                        c.Id,
                        Name = string.Join(" / ", c.NoCliente, c.Name, string.Format(c.Id.ToString("{0:U}", new InterceptProvider())))
                    }, "Id", "Name", id.Value);

                var cliente = await _context.Cliente
                    .Include(c => c.Comuna)
                    .Include(c => c.GirosCliente)
                    .Include(c => c.Vales)
                    .ThenInclude(c => c.Pagos)
                    .SingleOrDefaultAsync(m => m.Id == id);

                if (cliente == null)
                {
                    return NotFound();
                }
                model.Cliente = cliente;

            }
            else
            {
                model.ClientesList = new SelectList(
                    from c in _context.Cliente
                    select new { c.Id, Name = String.Join(" / ", c.NoCliente, c.Name, String.Format(new InterceptProvider(), "{0:U}", c.Id) ) }, "Id", "Name");
            }

            return View(model);
        }

        // GET: Clientes/Create
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            var model = new ClienteViewModel
            {
                ComunasList = new SelectList(_context.Comuna, "Id", "Name"),
                GirosList = new SelectList(_context.Set<Giro>(), "Id", "Name"),
                TiposList = new SelectList(from Tipo t in Enum.GetValues(typeof(Tipo))
                       select new { Id = t, t.GetType().GetMember(t.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).Name }, "Id", "Name")
            };
            return View(model);
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("RUT,ComunaId,GirosId,Name,Mensuales,Laborales,Renta,Retencion,SaldoInicial,NoCliente,Phone,Email,Tipo,Address,ClaveFE,ClaveSII")] ClienteViewModel cliente)
        {
            if (ModelState.IsValid)
            {
                var rut = Regex.Replace(cliente.RUT, "[^0-9]", "");
                var client = new Cliente { };
                //{
                client.Id = Convert.ToInt32(rut.Remove(rut.Length - 1));
                client.ComunaId = cliente.ComunaId;
                client.Phone = cliente.Phone;
                client.Email = cliente.Email;
                client.Tipo = cliente.Tipo;
                client.Address = cliente.Address;
                client.ClaveFE = cliente.ClaveFE;
                client.ClaveSII = cliente.ClaveSII;
                client.FechaIngreso = DateTime.Now;
                client.Name = cliente.Name;
                client.ApplicationUserId = (await _userManager.GetUserAsync(User)).Id;
                client.SaldosPendientes = cliente.SaldoInicial != null ? Convert.ToInt32(Regex.Replace(cliente.SaldoInicial, "[^0-9]", "")) : 0;
                //};

                if(cliente.Mensuales != null)
                    client.Mensuales = Convert.ToInt32(Regex.Replace(cliente.Mensuales, "[^0-9]", ""));

                if (cliente.Laborales != null)
                    client.Laborales = Convert.ToInt32(Regex.Replace(cliente.Laborales, "[^0-9]", ""));

                if (cliente.Renta != null)
                    client.Renta = Convert.ToInt32(Regex.Replace(cliente.Renta, "[^0-9]", ""));

                if (cliente.Retencion != null)
                    client.Retencion = Convert.ToInt32(Regex.Replace(cliente.Retencion, "[^0-9]", ""));

                if (cliente.NoCliente != null)
                    client.NoCliente = Convert.ToInt32(Regex.Replace(cliente.NoCliente, "[^0-9]", ""));

                _context.Cliente.Add(client);

                await _context.SaveChangesAsync();
                client.GirosCliente = new List<GirosCliente> { };

                if (cliente.SaldoInicial != null)
                {
                    var valeInicial = new Vale
                    {
                        ClienteId = client.Id,
                        ApplicationUserId = client.ApplicationUserId,
                        Date = client.FechaIngreso,
                        SubTotal = Convert.ToInt32(Regex.Replace(cliente.SaldoInicial, "[^0-9]", "")),
                        State = State.Activo,
                        Cobros = new List<Cobro> { }
                    };
                    var cobroInicial = new Cobro { Honorario = Honorario.SaldoInicial, Monto = valeInicial.SubTotal };

                    valeInicial.Cobros.Add(cobroInicial);

                    _context.Vale.Add(valeInicial);
                }

                foreach (var giro in cliente.GirosId)
                {
                    var giroCliente = new GirosCliente
                    {
                        Giro = _context.Giro.FirstOrDefault(g => g.Id == giro),
                        Cliente = client,
                        ClienteId = client.Id
                    };
                    giroCliente.GiroId = giroCliente.Giro.Id;
                    client.GirosCliente.Add(giroCliente);
                }
                _context.Cliente.Update(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ComunaId"] = new SelectList(_context.Comuna, "Id", "Name", cliente.ComunaId);
            ViewData["GiroId"] = new SelectList(_context.Set<Giro>(), "Id", "Name");
            var tipo = from Tipo t in Enum.GetValues(typeof(Tipo))
                       select new { Id = t, t.GetType().GetMember(t.ToString()).FirstOrDefault().GetCustomAttribute<DisplayAttribute>(false).Name };
            ViewData["Tipo"] = new SelectList(tipo, "Id", "Name");
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cliente = await _context.Cliente
                .Include(c => c.GirosCliente)
                .Include(c => c.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            var girosId = cliente.GirosCliente.Select(g => g.GiroId).ToList();

            var clientVM = new ClienteViewModel
            {
                RUT = string.Format(new InterceptProvider(), "{0:U}", cliente.Id),
                ComunaId = cliente.ComunaId,
                ComunasList = new SelectList(_context.Comuna, "Id", "Name", cliente.ComunaId),
                GirosList = new SelectList(_context.Set<Giro>(), "Id", "Name", girosId ),
                GirosId = girosId,
                TiposList = new SelectList(from Tipo t in Enum.GetValues(typeof(Tipo))
                                           select new { Id = t, t.GetType()
                                           .GetMember(t.ToString()).FirstOrDefault()
                                           .GetCustomAttribute<DisplayAttribute>(false).Name }, "Id", "Name", cliente.Tipo),
                Name = cliente.Name,
                Mensuales = cliente.Mensuales.HasValue ? cliente.Mensuales.Value.ToString("C", CultureInfo.CreateSpecificCulture("es-CL")).Replace("$","") : "",
                Laborales = cliente.Laborales.HasValue ? cliente.Laborales.Value.ToString("C", CultureInfo.CreateSpecificCulture("es-CL")).Replace("$", "") : "",
                Renta = cliente.Renta.HasValue ? cliente.Renta.Value.ToString("C", CultureInfo.CreateSpecificCulture("es-CL")).Replace("$", "") : "",
                Retencion = cliente.Retencion.HasValue ? cliente.Retencion.Value.ToString("C", CultureInfo.CreateSpecificCulture("es-CL")).Replace("$", "") : "",
                NoCliente = cliente.NoCliente.HasValue ? cliente.NoCliente.Value.ToString("C", CultureInfo.CreateSpecificCulture("es-CL")).Replace("$", "") : "",
                FechaIngreso = cliente.FechaIngreso,
                Phone = cliente.Phone,
                Email = cliente.Email,
                Tipo = cliente.Tipo,
                Address = cliente.Address,
                ClaveFE = cliente.ClaveFE,
                ClaveSII = cliente.ClaveSII,
                Editor = cliente.ApplicationUser.UserName
            };

            try
            {
                var valeInicial = cliente.Vales.SingleOrDefault(v => v.Cobros.Any(c => c.Honorario == Honorario.SaldoInicial));
                clientVM.SaldoInicial = valeInicial != null ? valeInicial.Cobros.First().Monto.ToString("C", CultureInfo.CreateSpecificCulture("es-CL")).Replace("$", "") : "";
            }
            catch
            {
                clientVM.SaldoInicial = "0";
            }

            return View(clientVM);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("RUT,ComunaId,GirosId,Name,Mensuales,Laborales,Renta,Retencion,SaldoInicial,NoCliente,Phone,Email,Tipo,Address,ClaveFE,ClaveSII")] ClienteViewModel cliente)
        {
            var rut = Convert.ToInt32(string.Format(new InterceptProvider(), "{0:I}", cliente.RUT));

            if (id != rut)
            {
                return RedirectToPage("KnownError","Id desconocido");
            }

            var model = _context.Cliente
                .Include(c => c.GirosCliente)
                .Include(c => c.Vales)
                .ThenInclude(c => c.Cobros)
                .Single(c => c.Id == id);

            if (ModelState.IsValid)
            {
                model.ComunaId = cliente.ComunaId;
                model.Name = cliente.Name;

                if (cliente.Mensuales != null)
                    model.Mensuales = Convert.ToInt32(Regex.Replace(cliente.Mensuales, "[^0-9]", ""));

                if (cliente.Laborales != null)
                    model.Laborales = Convert.ToInt32(Regex.Replace(cliente.Laborales, "[^0-9]", ""));

                if (cliente.Renta != null)
                    model.Renta = Convert.ToInt32(Regex.Replace(cliente.Renta, "[^0-9]", ""));

                if (cliente.Retencion != null)
                    model.Retencion = Convert.ToInt32(Regex.Replace(cliente.Retencion, "[^0-9]", ""));

                if (cliente.NoCliente != null)
                    model.NoCliente = Convert.ToInt32(Regex.Replace(cliente.NoCliente, "[^0-9]", ""));

                var valeInicial = model.Vales.SingleOrDefault(v => v.Cobros.Any(c => c.Honorario == Honorario.SaldoInicial));
                if(valeInicial != null)
                {
                    var saldoInicial = valeInicial.Cobros.First().Monto;
                    var nuevo = Convert.ToInt32(Regex.Replace(cliente.SaldoInicial, "[^0-9]", ""));
                    if (cliente.SaldoInicial != null && nuevo != saldoInicial)
                    {
                        valeInicial.State = State.Anulado;

                        _context.Vale.Update(valeInicial);

                        var valeInicialNew = new Vale
                        {
                            ClienteId = model.Id,
                            ApplicationUserId = model.ApplicationUserId,
                            Date = model.FechaIngreso,
                            SubTotal = nuevo,
                            State = State.Activo,
                        };
                        var cobroInicial = new Cobro { Honorario = Honorario.SaldoInicial, Monto = valeInicial.SubTotal };

                        valeInicial.Cobros.Add(cobroInicial);

                        _context.Vale.Add(valeInicial);
                    }
                }

                model.Phone = cliente.Phone;
                model.Email = model.Email;
                model.Tipo = model.Tipo;
                model.ClaveFE = model.ClaveFE;
                model.ClaveSII = model.ClaveSII;

                foreach (var giroId in cliente.GirosId)
                {
                    if (!model.GirosCliente.Any(g => g.GiroId == giroId))
                    {
                        model.GirosCliente.Add(new GirosCliente { ClienteId = id, GiroId = giroId });
                    }
                }

                foreach (var girosCliente in model.GirosCliente)
                {
                    if (!cliente.GirosId.Contains(girosCliente.GiroId))
                    {
                        model.GirosCliente.Remove(girosCliente);
                    }
                }

                try
                {
                    _context.Cliente.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(id))
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

            return View(cliente);
        }

        // GET: Clientes/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .Include(c => c.Comuna)
                .Include(c => c.GirosCliente)
                .Include(c => c.Pagos)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Cliente
                .Include(c => c.GirosCliente)
                .SingleOrDefaultAsync(m => m.Id == id);
            foreach (var giro in cliente.GirosCliente)
            {
                _context.GirosCliente.Remove(giro);
            }
            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult ClientExists(int id)
        {
            var response = ClienteExists(id).ToString();
            return Json(new { response });
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(e => e.Id == id);
        }
    }
}
