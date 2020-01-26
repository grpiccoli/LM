using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLM.Models;
using CLM.Data;

namespace CLM.Services
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.ApplicationUser;
        }

        public ApplicationUser GetById(string id)
        {
            return GetAll().FirstOrDefault(
                u => u.Id == id);
        }

        public async Task UpdateUserRating(string id, System.Type type)
        {
            var user = GetById(id);
            await _context.SaveChangesAsync();
        }

        private int CalculateUserRating(System.Type type, int userRating)
        {
            var inc = 0;
            //if (type == typeof(Post))
            //    inc = 1;

            //if (type == typeof(PostReply))
            //    inc = 3;

            return userRating + inc;
        }

        public async Task SetProfileImage(string id, Uri uri)
        {
            var user = GetById(id);
            user.ProfileImageUrl = uri.AbsoluteUri;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
