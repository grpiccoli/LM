using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class ValePago
    {
        public int ValeId { get; set; }
        public int PagoId { get; set; }
        public virtual Vale Vale { get; set; }
        public virtual Pago Pago { get; set; }
    }
}
