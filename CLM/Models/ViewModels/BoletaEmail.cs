using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class BoletaEmail : Detalle
    {
        public int Id { get; set; }

        public string Fecha { get; set; }
    }
}
