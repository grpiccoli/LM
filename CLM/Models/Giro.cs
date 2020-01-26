using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class Giro
    {
        [Display(Name = "Código de Giro")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "Giro")]
        public string Name { get; set; }

        public virtual ICollection<GirosCliente> GirosClientes { get; set; }
    }
}
