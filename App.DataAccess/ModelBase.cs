using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataAccess {
    public class ModelBase : IModelBase {

        public ModelBase() {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
        }
        
        public void Init() {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
        }

        public void Delete(string userName) {
            this.IsDeleted = true;
            this.UpdatedBy = userName;
            this.UpdatedDate = DateTime.Now;
        }

        [Key]
        [Required]
        [MaxLength(128)]
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        //[Index("Deleted")]
        //[Index("DraftAndDelete",1)]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        //[Index("Draft")]
        //[Index("DraftAndDelete", 2)]
        [DefaultValue(false)]
        public bool IsDraft { get; set; }
        
    }
}
