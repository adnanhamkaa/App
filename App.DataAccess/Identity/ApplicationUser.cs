using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.DataAccess.Identity
{
    public class ApplicationUser : IdentityUser,IModelBase {

        public string FullName { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        [DefaultValue(false)]
        public bool IsDraft { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }


        public void Init() {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
        }

        public void Delete(string userName) {
            this.IsDeleted = true;
            this.UpdatedBy = userName;
            this.UpdatedDate = DateTime.Now;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
