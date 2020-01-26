using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class HistorialPagoVM
    {
        public int? ClienteId { get; set; }

        public Cliente Cliente { get; set; }

        public IEnumerable<SelectListItem> ClientesList { get; set; }

        public IEnumerable<Pago> Pagos { get; set; }

        public IEnumerable<Honorario> Honorarios { get; set; }
    }
}
