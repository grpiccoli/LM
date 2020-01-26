using System;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models
{
    public class Retiro
    {
        public int Id { get; set; }

        [Display(Name ="Retirado por")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "Oficina")]
        public int OficinaId { get; set; }
        public virtual Oficina Oficina { get; set; }

        [Display(Name ="Fecha de retiro")]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public int Monto { get; set; }

        public Forma Forma { get; set; }

        public string Detalles { get; set; }
    }

    public enum Forma
    {
        Efectivo,
        Cheque
    }

}
