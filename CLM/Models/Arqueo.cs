using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class Arqueo
    {
        public int Id { get; set; }

        [Display(Name ="Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name ="Responsable")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int OficinaId { get; set; }
        public virtual Oficina Oficina { get; set; }

        public int ResultadoEjercicio { get; set; }

        public int Efectivo { get; set; }

        public int Cheque { get; set; }

        public int Transferencia { get; set; }

        public int Saldo { get; set; }
    }
}
