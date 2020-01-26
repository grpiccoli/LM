using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class ApplicationRoleViewModel
    {
        public string ID { get; set; }

        [Display(Name = "Rol")]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
