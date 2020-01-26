using System.Collections.Generic;

namespace CLM.Models
{
    public class RoleData
    {
        public static List<string> ApplicationRoles { get; set; } = new List<string>
                                                            {
                                                                "Administrador",
                                                                "Caja",
                                                                "Cobros",
                                                                "Funcionario",
                                                                "PartTime"
                                                            };
    }
}
