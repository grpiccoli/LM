using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class TableFilterVM
    {
        public string Id { get; set; }

        public string Val { get; set; }

        public string Name { get; set; }

        public int MaxOptions { get; set; }

        public string Srt { get; set; }

        public bool Asc { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Rpp { get; set; }

        public MultiSelectList List { get; set; }

        public bool LiveSearch { get; set; } = false;

        public FilterType Type { get; set; } = FilterType.sort;
    }

    public enum FilterType
    {
        select,date,checkbox,sort
    }
}
