﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CLM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CLM.Data;

namespace CLM.Controllers
{
    [Authorize(Policy = "Usuarios", Roles = "Administrador")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger _logger;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            model = _userManager.Users.Select(u => new UserListViewModel
            {
                ID = u.Id,
                Email = u.Email,
                ProfileImageUrl = u.ProfileImageUrl,
                RoleName = u.Roles.Any() ? _context.Roles.Where(r => r.Id == u.Roles.FirstOrDefault().RoleId).SingleOrDefault().Name : "Estándar"
            }).ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult AddUser()
        {
            UserViewModel model = new UserViewModel
            {
                UserClaims = ClaimData.UserClaims.Select(c => new SelectListItem
                {
                    Text = c,
                    Value = c
                }).ToList(),
                ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                }).ToList()
            };
            return PartialView("_AddUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                ApplicationUser applicationUser = await _userManager.FindByEmailAsync(user.Email);
                List<SelectListItem> userClaims = model.UserClaims.Where(c => c.Selected).ToList();
                foreach (var claim in userClaims)
                {
                    applicationUser.Claims.Add(new IdentityUserClaim<string>
                    {
                        ClaimType = claim.Value,
                        ClaimValue = claim.Value
                    });
                }
                if (result.Succeeded)
                {
                    ApplicationRole applicationRole = await _roleManager.FindByIdAsync(model.ApplicationRoleID);
                    if(applicationRole != null)
                    {
                        IdentityResult identityResult = await _userManager.AddToRoleAsync(user, applicationRole.Name);
                        if(identityResult.Succeeded)
                        {
                            _logger.LogInformation("Se ha añadido un nuevo usuario exitosamente.");
                            return RedirectToAction("Index");
                        }
                    }
                }
                AddErrors(result);
            }
            return RedirectToAction("_AddUser");
            //return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
//        [Authorize(Policy = "Usuarios")]
        [HttpGet]
        [Authorize(Roles = "Administrador,Editor")]
        public async Task<IActionResult> EditUser(string id)
        {
            EditUserViewModel model = new EditUserViewModel()
            {
                ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                }).ToList()
            };
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    model.Email = applicationUser.Email;
                    var claims = await _userManager.GetClaimsAsync(applicationUser);
                    model.UserClaims = ClaimData.UserClaims.Select(c => new SelectListItem
                    {
                        Text = c,
                        Value = c,
                        Selected = claims.Any(x => x.Value == c)
                    }).ToList();
                    try
                    {
                        string role = _userManager.GetRolesAsync(applicationUser).Result.Single();
                        if (!String.IsNullOrEmpty(role))
                        {
                            string roleID = _roleManager.Roles.Single(r => r.Name == role).Id;
                            if (!String.IsNullOrEmpty(roleID))
                            {
                                model.ApplicationRoleID = roleID;
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    model.UserClaims = ClaimData.UserClaims.Select(c => new SelectListItem
                    {
                        Text = c,
                        Value = c
                    }).ToList();
                }
            }
            return PartialView("_EditUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Editor")]
        public async Task<IActionResult> EditUser(string id, EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);
                if (applicationUser != null && applicationUser.Email != "adminmit@bibliomit.cl")
                {
                    applicationUser.Email = model.Email;
                    List<SelectListItem> userClaims = new List<SelectListItem>();
                    try
                    {
                        IList<Claim> claims = await _userManager.GetClaimsAsync(applicationUser);
                        userClaims = model
                                        .UserClaims
                                        .Where(c => c.Selected && !claims
                                            .Any(u => u.Value == c.Value))
                                            .ToList();
                        List<Claim> userRemoveClaims = claims
                            .Where(c => model
                                        .UserClaims
                                        .Any(u => u.Value == c.Value && !u.Selected))
                            .ToList();
                        foreach (Claim claim in userRemoveClaims)
                        {
                            await _userManager.RemoveClaimAsync(applicationUser, claim);
                        }
                    }
                    catch
                    {
                        userClaims = model.UserClaims.Where(u => u.Selected).ToList();
                    }
                    foreach (var claim in userClaims)
                    {
                        applicationUser.Claims.Add(new IdentityUserClaim<string>
                        {
                            ClaimType = claim.Value,
                            ClaimValue = claim.Value
                        });
                    }
                    IdentityResult result = await _userManager.UpdateAsync(applicationUser);
                    if (result.Succeeded)
                    {
                        try
                        {
                            string existingRole = _userManager.GetRolesAsync(applicationUser).Result.Single();
                            string existingRoleId = _roleManager.Roles.Single(r => r.Name == existingRole).Id;
                            if (existingRoleId != model.ApplicationRoleID)
                            {
                                IdentityResult roleResult = await _userManager.RemoveFromRoleAsync(applicationUser, existingRole);
                                if (roleResult.Succeeded)
                                {
                                    ApplicationRole applicationRole = await _roleManager.FindByIdAsync(model.ApplicationRoleID);
                                    if (applicationRole != null)
                                    {
                                        IdentityResult newRoleResult = await _userManager.AddToRoleAsync(applicationUser, applicationRole.Name);
                                        if (newRoleResult.Succeeded)
                                        {
                                            return RedirectToAction("Index");
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            ApplicationRole applicationRole = await _roleManager.FindByIdAsync(model.ApplicationRoleID);
                            if (applicationRole != null)
                            {
                                IdentityResult newRoleResult = await _userManager.AddToRoleAsync(applicationUser, applicationRole.Name);
                                if (newRoleResult.Succeeded)
                                {
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUser(string id)
        {            
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    name = applicationUser.Email;
                }
            }
            return PartialView("_DeleteUser", name);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUser(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);
                if (applicationUser != null && applicationUser.Email != "adminmit@bibliomit.cl")
                {
                    IdentityResult result = await _userManager.DeleteAsync(applicationUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
    }
}