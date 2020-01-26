using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CLM.Models;
using CLM.Services;
using CLM.Models.ProfileViewModels;
using Microsoft.AspNetCore.Http;
//using Amazon.S3;
//using Amazon.S3.Model;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace CLM.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        //private readonly IUpload _uploadService;

        public ProfilesController(
            UserManager<ApplicationUser> userManager,
            IApplicationUser userService
            //,IUpload uploadService
            )
        {
            _userManager = userManager;
            _userService = userService;
            //_uploadService = uploadService;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            var profiles = _userService.GetAll()
                .Select(u => new ProfileModel
                {
                    Email = u.Email,
                    UserName = u.UserName,
                    ProfileImageUrl = u.ProfileImageUrl
                });

            var model = new ProfileListModel
            {
                Profiles = profiles
            };

            return View(model);
        }

        public IActionResult Details(string id)
        {
            var user = _userService.GetById(id);
            var userRoles = _userManager.GetRolesAsync(user).Result;

            var model = new ProfileModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                IsAdmin = userRoles.Contains("Administrador")
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UploadProfileImage(IFormFile file)
        {
            var userId = _userManager.GetUserId(User);

            //var accessKey = "AKIAISMYGSV5LKLHP25A";
            //var secretKey = "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";

            //var client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.SAEast1);

            //var filePath = Path.GetTempFileName();

            //var stream = new FileStream(filePath, FileMode.Create);

            //var request = new PutObjectRequest
            //{
            //    BucketName = "bucketmit",
            //    Key = userId,
            //    FilePath = filePath
            //};

            //var url = "https://" + request.BucketName + ".s3.amazonaws.com/" + request.Key;

            //var response = client.PutObjectAsync(request).GetAwaiter().GetResult();

            //_userService.SetProfileImage(userId, new Uri(url));

            return RedirectToAction("Details","Profiles",new { id = userId });
        }
    }
}