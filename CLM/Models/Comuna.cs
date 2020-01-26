using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLM.Models
{
    public class Comuna
    {
        [Display(Name = "Código de Comuna")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ProvinciaId { get; set; }

        public virtual Provincia Provincia { get; set; }

        [Display(Name = "Nombre de Comuna")]
        public string Name { get; set; }

        [Display(Name = "Distrito Electoral")]
        public int DE { get; set; }

        [Display(Name = "Circunscripción Senatorial")]
        public int CS { get; set; }

        public ICollection<Cliente> Clientes { get; set; }
    }
}
