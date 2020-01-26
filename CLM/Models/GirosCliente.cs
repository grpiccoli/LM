using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class GirosCliente
    {
        public int GiroId { get; set; }
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Giro Giro { get; set; }
    }
}
