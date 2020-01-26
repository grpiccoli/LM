using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CLM.Models;
using Microsoft.AspNetCore.Identity;

namespace CLM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationRole>()
                .HasMany(e => e.Users)
                .WithOne()
                .HasForeignKey(e => e.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cliente>()
                .HasMany(c => c.Pagos)
                .WithOne()
                .HasForeignKey(p => p.ClienteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Oficina>()
                .HasMany(o => o.Pagos)
                .WithOne()
                .HasForeignKey(p => p.OficinaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ValePago>()
                .HasKey(c => new { c.ValeId, c.PagoId });

            builder.Entity<PuestoTrabajo>()
                .HasKey(c => new { c.ApplicationUserId, c.OficinaId });

            builder.Entity<GirosCliente>()
                .HasKey(c => new { c.ClienteId, c.GiroId });
        }

        public DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }
        public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }

        public DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<ApplicationRole> ApplicationRole { get; set; }

        public DbSet<Region> Region { get; set; }

        public DbSet<Provincia> Provincia { get; set; }

        public DbSet<Comuna> Comuna { get; set; }

        public DbSet<Cliente> Cliente { get; set; }

        public DbSet<GirosCliente> GirosCliente { get; set; }

        public DbSet<Giro> Giro { get; set; }

        public DbSet<Vale> Vale { get; set; }

        public DbSet<Cobro> Cobro { get; set; }

        public DbSet<Pago> Pago { get; set; }

        public DbSet<Oficina> Oficina { get; set; }

        public DbSet<Retiro> Retiro { get; set; }

        public DbSet<Arqueo> Arqueo { get; set; }

        public DbSet<ValePago> ValePagos { get; set; }
    }
}
