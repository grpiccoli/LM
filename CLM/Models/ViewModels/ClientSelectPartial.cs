using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class ClientSelectPartial
    {
        public int ClienteId { get; set; }

        public IEnumerable<SelectListItem> ClientesList { get; set; }
    }
}
