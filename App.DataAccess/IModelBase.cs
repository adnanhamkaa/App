using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataAccess {
    public interface IModelBase {
        [Key]
        [Required]
        [MaxLength(128)]
        string Id { get; set; }
        string CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        [DefaultValue(false)]
        bool IsDeleted { get; set; }
        [DefaultValue(false)]
        bool IsDraft { get; set; }
        void Init();
        void Delete(string userName);

    }
}
