using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class ArqueoVM
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        public int PagosEfectivo { get; set; }

        public int PagosCheque { get; set; }

        public int PagosTransferencia { get; set; }

        public int PagosTotal { get; set; }

        public int RetiroEfectivo { get; set; }

        public int RetiroCheque { get; set; }

        public int RetiroTotal { get; set; }

        public int ArqueoEfectivo { get; set; }

        public int ArqueoCheques { get; set; }

        public int ArqueoTransferencias { get; set; }

        public int ArqueoTotal { get; set; }

        public int DiferenciaEfectivo { get; set; }

        public int DiferenciaCheques { get; set; }

        public int DiferenciaTransferencias { get; set; }

        public int DiferenciaTotal { get; set; }

        public int SaldoAnterior { get; set; }

        [Display(Name = "Responsable")]
        public string ApplicationUserId { get; set; }

        public string ApplicationUserName { get; set; }

        [Display(Name = "Oficina")]
        public int OficinaId { get; set; }

        public string OfficeName { get; set; }
    }
}
