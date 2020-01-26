
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models
{
    public class Vale
    {
        [Display(Name = "Vale")]
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        [Display(Name = "Atendido Por")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public int SubTotal { get; set; }

        public virtual ICollection<Cobro> Cobros { get; set; }

        public virtual ICollection<ValePago> Pagos { get; set; }

        [Display(Name = "Estado")]
        public State State { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public int Pagado { get; set; }
    }

    public enum State
    {
        Activo,
        Pendiente,
        Anulado,
        Pagado
    }
}
