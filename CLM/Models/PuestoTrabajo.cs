using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class PuestoTrabajo
    {
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int OficinaId { get; set; }
        public virtual Oficina Oficina { get; set; }
    }
}
