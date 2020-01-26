﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLM.Models
{
    public class UserViewModel
    {
        public string ID { get; set; }

        [Required(ErrorMessage = "Se requiere una contraseña")]
        [StringLength(100, ErrorMessage = "La {0} debe tener entre {2} y {1} caracteres de largo.", MinimumLength = 6)]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Se requiere confirmar la contraseña")]
        [Display(Name = "Confirmar Contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Permisos de usuario")]
        public List<SelectListItem> UserClaims { get; set; }

        public List<SelectListItem> ApplicationRoles { get; set; }

        [Display(Name = "Rol")]
        public string ApplicationRoleID { get; set; }
    }
}
