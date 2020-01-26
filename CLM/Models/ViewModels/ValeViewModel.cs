using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models
{
    public class ValeViewModel
    {
        [Required]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        public string SubTotal { get; set; }

        public string Total { get; set; }

        [Display(Name = "Cobros")]
        public List<int> HonorariosId { get; set; }

        public List<SelectListItem> HonorariosList { get; set; }

        [Display(Name ="Saldos Pendientes")]
        public string SaldosPendientes { get; set; }

        public IEnumerable<SelectListItem> ClientesList { get; set; }

        public List<CobroViewModel> Cobros { get; set; }
    }
}
