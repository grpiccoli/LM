using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class DisponibleVM
    {
        [Display(Name = "Efectivo Disponible")]
        public string Efectivo { get; set; }

        [Display(Name = "Cheques Disponibles")]
        public string Cheque { get; set; }

        public int OficinaId { get; set; }
    }
}
