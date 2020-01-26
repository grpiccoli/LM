using System.Collections.Generic;

namespace CLM.Models
{
    public class ClaimData
    {
        public static List<string> UserClaims { get; set; } = new List<string>
                                                            {
                                                                "Clientes",
                                                                "Historial",
                                                                "CajaPago",
                                                                "Retiros",
                                                                "Arqueo",
                                                                "Vale",
                                                                "Usuarios"
                                                            };
    }
}
