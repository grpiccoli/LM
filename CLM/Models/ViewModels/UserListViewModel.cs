using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class UserListViewModel
    {
        public string ID { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Permisos")]
        public List<string> UserClaims { get; set; }

        [Display(Name = "Rol")]
        public string RoleName { get; set; }

        [Display(Name = "Puestos de trabajo")]
        public List<string> PuestosTrabajoList { get; set; }

        [Display(Name = "Foto")]
        public string ProfileImageUrl { get; set; }

        [Display(Name = "Fecha Ingreso")]
        [DisplayFormat(DataFormatString = "{0:dd/MMMM/yyyy}")]
        public DateTime MemberSince { get; set; }
    }
}
