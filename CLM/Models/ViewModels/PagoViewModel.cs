using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models.ViewModels
{
    public class PagoViewModel
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        public string Fecha { get; set; }

        public string AvailableDates { get; set; }

        [Display(Name = "Oficina")]
        public int OficinaId { get; set; }
        [Display(Name = "Oficina")]
        public IEnumerable<SelectListItem> OficinasList { get; set; }

        [Display(Name ="Vales Caja para fecha seleccionada")]
        public int ValeId { get; set; }
        public IEnumerable<SelectListItem> ValesList { get; set; }

        public int ClienteId { get; set; }

        [Display(Name = "Vales Caja para fecha seleccionada")]
        public Medio Medio { get; set; }
        public IEnumerable<SelectListItem> MediosList { get; set; }

        public string Monto { get; set; }

        public string Saldo { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        [Display(Name = "Fecha de Transferencia")]
        public string FechaTransferencia { get; set; }

        [Display(Name = "RUT de Transferencia")]
        public string RUT { get; set; }
    }
}
