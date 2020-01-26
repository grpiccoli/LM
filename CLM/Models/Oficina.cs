using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class Oficina
    {
        public int Id { get; set; }

        [Display(Name = "Tipo")]
        public string Name { get; set; }

        [Display(Name ="Dirección")]
        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public virtual ICollection<PuestoTrabajo> PuestosTrabajo { get; set; }

        public virtual ICollection<Pago> Pagos { get; set; }

        public virtual ICollection<Arqueo> Arqueos { get; set; }
    }
}
