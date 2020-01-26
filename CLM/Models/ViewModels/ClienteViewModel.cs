using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models
{
    public class ClienteViewModel
    {
        [Display(Name = "RUT")]
        [DisplayFormat(DataFormatString = "{0:###'.'###'.'###}")]
        [RegularExpression(@"^(([0-9]{0,3})\.{0,1}){0,4}-[0-9Kk]$", ErrorMessage = "Ingrese número de RUT y dígito verificador")]
        [Required(ErrorMessage = "El RUT es requerido")]
        public string RUT { get; set; }

        public string Editor { get; set; }

        [Required(ErrorMessage = "Por favor seleccione una comuna")]
        [Display(Name ="Comuna")]
        public int ComunaId { get; set; }

        public IEnumerable<SelectListItem> ComunasList { get; set; }

        public IEnumerable<SelectListItem> TiposList { get; set; }

        [Display(Name = "Giro")]
        public IEnumerable<int> GirosId { get; set; }

        public IEnumerable<SelectListItem> GirosList { get; set; }

        [Display(Name = "Razón Social")]
        public string Name { get; set; }

        [Display(GroupName ="Honorarios")]
        public string Mensuales { get; set; }

        [Display(GroupName = "Honorarios")]
        public string Laborales { get; set; }

        [Display(GroupName = "Honorarios")]
        public string Renta { get; set; }

        [Display(GroupName = "Honorarios")]
        public string Retencion { get; set; }

        [Display(Name ="Saldo inicial")]
        public string SaldoInicial { get; set; }

        [Display(Name = "Número de Cliente")]
        public string NoCliente { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        [DisplayFormat(DataFormatString = "{0:dd/MMMM/yyyy}")]
        public DateTime FechaIngreso { get; set; }
        
        [Display(Name = "Teléfono")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+[0-9\s]{14}$", ErrorMessage = "Debe ingresar solo 11 números")]
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public Tipo Tipo { get; set; }

        [Display(Name = "Dirección")]
        public string Address { get; set; }

        public string ClaveFE { get; set; }
        
        public string ClaveSII { get; set; }
    }
}
