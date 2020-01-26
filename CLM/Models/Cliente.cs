using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLM.Models
{
    public class Cliente
    {
        [Display(Name = "RUT")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayFormat(DataFormatString = "{0,9:N0}")]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name ="Comuna")]
        public int ComunaId { get; set; }
        public virtual Comuna Comuna { get; set; }

        [Display(Name ="Giro")]
        public virtual ICollection<GirosCliente> GirosCliente { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(GroupName ="Honorarios")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int? Mensuales { get; set; }

        [Display(GroupName = "Honorarios")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int? Laborales { get; set; }

        [Display(GroupName = "Honorarios")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int? Renta { get; set; }

        [Display(GroupName = "Honorarios")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int? Retencion { get; set; }

        [Display(Name = "Saldos pendientes")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double SaldosPendientes { get; set; }

        [Display(Name = "Número de Cliente")]
        public int? NoCliente { get; set; }

        [Display(Name = "Fecha de Ingreso")]     
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaIngreso { get; set; }
        
        [Display(Name = "Teléfono")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public Tipo Tipo { get; set; }

        [Display(Name = "Dirección")]
        public string Address { get; set; }

        public string ClaveFE { get; set; }
        
        public string ClaveSII { get; set; }

        public virtual ICollection<Vale> Vales { get; set; }

        public virtual ICollection<Pago> Pagos { get; set; }
    }

    public enum Tipo
    {
        [Display(Name ="Persona Natural")]
        natural,
        [Display(Name = "Persona Jurídica")]
        juridica
    }

}
