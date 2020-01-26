using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class RetiroVM
    {
        [Required]
        [RegularExpression("^[0-9\\.]+$")]
        public string Monto { get; set; }

        public string Detalles { get; set; }

        [Required]
        public Forma? Forma { get; set; }

        public IEnumerable<SelectListItem> FormaList { get; set; }

        [Display(Name = "Oficina")]
        [Required]
        public int OficinaId { get; set; }
        public IEnumerable<SelectListItem> OficinaList { get; set; }

        public IList<DisponibleVM> DisponibleVMs { get; set; }
    }
}
