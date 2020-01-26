using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [RegularExpression(@"^.*$")]
        public string Name { get; set; }
        public string Last { get; set; }
        public DateTime MemberSince { get; set; }
        public string ProfileImageUrl { get; set; }
        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();
        
        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        public virtual ICollection<Arqueo> Arqueos { get; set; }
        public virtual ICollection<Retiro> Retiros { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
        public virtual ICollection<Vale> Vales { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
        public virtual ICollection<PuestoTrabajo> PuestosTrabajo { get; set; }
    }
}
