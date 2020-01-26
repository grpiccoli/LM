using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models.ViewModels
{
    public class UsersViewModel
    {
        public string Name { get; set; }

        public string[] Claims { get; set; }

        public string[] Roles { get; set; }

        public string Email { get; set; }

        public string Key { get; set; }

        public string[] Places { get; set; }
    }
}
