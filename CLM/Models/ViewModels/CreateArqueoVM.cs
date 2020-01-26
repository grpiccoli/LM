using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models.ViewModels
{
    public class CreateArqueoVM
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string Fecha { get; set; }

        [Display(Name = "Oficina")]
        public int OficinaId { get; set; }
        [Display(Name = "Oficina")]
        public IEnumerable<SelectListItem> OficinasList { get; set; }

        public string Efectivo { get; set; }

        public string Cheque { get; set; }

        public string Transferencia { get; set; }

        public string FechaList { get; set; }
    }
}
