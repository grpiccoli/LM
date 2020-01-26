using CLM.Data;
using CLM.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class UserInitializer
    {
        public static Task Initialize(ApplicationDbContext context)
        {
            var roleStore = new RoleStore<ApplicationRole>(context);
            var userStore = new UserStore<ApplicationUser>(context);

            if (!context.ApplicationUserRole.Any())
            {
                if (!context.Users.Any())
                {
                    if (!context.ApplicationRole.Any())
                    {
                        var applicationRoles = new List<ApplicationRole> { };
                        foreach (var item in RoleData.ApplicationRoles)
                        {
                            applicationRoles.Add(
                                new ApplicationRole
                                {
                                    CreatedDate = DateTime.Now,
                                    Name = item,
                                    Description = "",
                                    NormalizedName = item.ToLower()
                                });
                        };

                        foreach (var role in applicationRoles)
                        {
                            context.ApplicationRole.Add(role);
                        }
                        context.SaveChanges();
                    }

                    var users = new UsersViewModel[]
                    {
                        new UsersViewModel
                        {
                            Name = "WebMaster",
                            Places = new string[]{ "Matriz", "Sucursal" },
                            Roles = new string[]{ "Administrador" },
                            Claims = new string[]{ "Clientes",
                                                    "Historial",
                                                    "CajaPago",
                                                    "Retiros",
                                                    "Arqueo",
                                                    "Vale",
                                                    "Usuarios" },
                            Email = "guille.arp@gmail.com",
                            Key = "g5cNs5s<"
                        },
                        new UsersViewModel
                        {
                            Name = "Erwin Soto",
                            Places = new string[]{ "Matriz", "Sucursal" },
                            Roles = new string[]{ "Administrador" },
                            Claims = new string[]{ "Clientes",
                                                    "Historial",
                                                    "CajaPago",
                                                    "Retiros",
                                                    "Arqueo",
                                                    "Vale",
                                                    "Usuarios" },
                            Email = "contadorlosmuermos@gmail.com",
                            Key = "uatd9Idj"
                        },
                        new UsersViewModel
                        {
                            Name = "Priscila",
                            Places = new string[]{ "Matriz", "Sucursal" },
                            Roles = new string[]{ "Administrador" },
                            Claims = new string[]{ "Clientes",
                                                    "Historial",
                                                    "CajaPago",
                                                    "Retiros",
                                                    "Arqueo",
                                                    "Vale",
                                                    "Usuarios" },
                            Email = "prixhy@hotmail.com",
                            Key = "3ovZt<Jn"
                        },
                        new UsersViewModel
                        {
                            Name = "Luz Soto",
                            Places = new string[]{ "Sucursal" },
                            Roles = new string[]{ "Caja" },
                            Claims = new string[]{ "CajaPago",
                                                    "Arqueo",
                                                    "Vale" },
                            Email = "contabilidadlosmuermos1@gmail.com",
                            Key = "zkdr3Rqa"
                        },
                        new UsersViewModel
                        {
                            Name = "Natalia Díaz",
                            Places = new string[]{ "Matriz" },
                            Roles = new string[]{ "Cobros" },
                            Claims = new string[]{ "Clientes",
                                                    "Historial",
                                                    "CajaPago",
                                                    "Arqueo",
                                                    "Vale" },
                            Email = "contabilidadlosmuermos2@gmail.com",
                            Key = "tvQLg1UG"
                        },
                        new UsersViewModel
                        {
                            Name = "Macarena Sanchez",
                            Places = new string[]{ "Sucursal" },
                            Roles = new string[]{ "Cobros" },
                            Claims = new string[]{ "Clientes",
                                                    "Historial",
                                                    "Vale" },
                            Email = "contabilidadlosmuermos3@gmail.com",
                            Key = ">?Kj;DId"
                        },
                        new UsersViewModel
                        {
                            Name = "Mirso Miranda",
                            Places = new string[]{ "Sucursal" },
                            Roles = new string[]{ "Funcionario" },
                            Claims = new string[]{ "Vale" },
                            Email = "contabilidadlosmuermos4@gmail.com",
                            Key = "mqunZeKW"
                        },
                        new UsersViewModel
                        {
                            Name = "Karen Godoy",
                            Places = new string[]{ "Sucursal" },
                            Roles = new string[]{ "Funcionario" },
                            Claims = new string[]{ "Vale" },
                            Email = "contabilidadlosmuermos5@gmail.com",
                            Key = "ptt0neXN"
                        },
                        new UsersViewModel
                        {
                            Name = "Fernanda Díaz",
                            Places = new string[]{ "Sucursal", "Matriz" },
                            Roles = new string[]{ "Funcionario" },
                            Claims = new string[]{ "Vale" },
                            Email = "contabilidadlosmuermos6@gmail.com",
                            Key = "vgGc0xMY"
                        },
                        new UsersViewModel
                        {
                            Name = "Part Time 1",
                            Places = new string[]{ "Matriz" },
                            Roles = new string[]{ "PartTime" },
                            Claims = new string[]{ "Vale" },
                            Email = "contabilidadlosmuermos8@gmail.com",
                            Key = "3pQ866Ov"
                        },
                        new UsersViewModel
                        {
                            Name = "Part Time 2",
                            Places = new string[]{ "Matriz" },
                            Roles = new string[]{ "PartTime" },
                            Claims = new string[]{ "Vale" },
                            Email = "contabilidadlosmuermos9@gmail.com",
                            Key = "tSgyPLBo"
                        }
                    };

                    var iniDate = DateTime.Now;

                    foreach (var item in users)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = item.Name,
                            NormalizedUserName = item.Name.ToLower(),
                            Email = item.Email,
                            NormalizedEmail = item.Email.ToLower(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            ProfileImageUrl = "/images/ico/icono.svg",
                            PuestosTrabajo = new List<PuestoTrabajo> { },
                            MemberSince = iniDate
                        };

                        foreach (var place in item.Places)
                        {
                            var oficina = context.Oficina.FirstOrDefault(o => o.Name == place);
                            user.PuestosTrabajo.Add(new PuestoTrabajo
                            {
                                Oficina = oficina,
                                OficinaId = oficina.Id,
                                ApplicationUser = user,
                                ApplicationUserId = user.Id
                            });
                        }

                        var hasher = new PasswordHasher<ApplicationUser>();
                        var hashedPassword = hasher.HashPassword(user, item.Key);
                        user.PasswordHash = hashedPassword;

                        foreach (var claim in item.Claims)
                        {
                            user.Claims.Add(new IdentityUserClaim<string>
                            {
                                ClaimType = claim,
                                ClaimValue = claim
                            });
                        }

                        context.Users.Add(user);
                        context.SaveChanges();

                        foreach (var role in item.Roles)
                        {
                            var rol = context.ApplicationRole.SingleOrDefault(m => m.Name == role);

                            var usuario = context.Users.SingleOrDefault(m => m.UserName == item.Name);

                            var userRole = new ApplicationUserRole
                            {
                                UserId = usuario.Id,
                                RoleId = rol.Id
                            };

                            context.ApplicationUserRole.Add(userRole);
                        }

                        context.SaveChanges();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
