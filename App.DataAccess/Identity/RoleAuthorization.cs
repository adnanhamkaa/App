using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataAccess.Identity {
    public class ApplicationRole : IdentityRole, IModelBase {
        public ICollection<ActionAuthorization> Actions { get; set; }

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

        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }

    }

    public class ActionAuthorization : ModelBase {
        public string ActionName { get; set; }

        [JsonIgnore]
        public virtual ICollection<ApplicationRole> Roles { get; set; }
        
    }

}
