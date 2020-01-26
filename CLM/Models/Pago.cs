using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models
{
    public class Pago
    {
        [Display(Name = "Pago")]
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public virtual ICollection<ValePago> Vales { get; set; }

        [Display(Name = "Atendido Por")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "Oficina")]
        public int OficinaId { get; set; }
        public virtual Oficina Oficina { get; set; }

        [Display(Name = "Monto cancelado")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int Monto { get; set; }

        [Display(Name = "Medio de pago")]
        public Medio Medio { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaTransferencia { get; set; }

        [Display(Name = "RUT")]
        public int RutTransferencia { get; set; }
    }

    public enum Medio
    {
        Efectivo, 
        Transferencia,
        Cheque
    }
}
