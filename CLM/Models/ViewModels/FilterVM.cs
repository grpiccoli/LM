using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class FilterVM
    {
        public string Rpp { get; set; }

        public bool Asc { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Srt { get; set; }

        public string Val { get; set; }
    }
}
