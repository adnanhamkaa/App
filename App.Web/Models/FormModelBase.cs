using App.DataAccess;
using App.Web.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace App.Web.Models
{
    public class FormModelBase {
        [DttField(hidden:true)]
        public string Id { get; set; }
        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }

        [DttField(renderFunction: @"datatableDate('DD MMMM YYYY HH:mm')")]
        [Display(Name = "Created at")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated by")]
        public string UpdatedBy { get; set; }
        [DttField(renderFunction: @"datatableDate('DD MMMM YYYY HH:mm')")]
        [Display(Name = "Updated at")]
        public DateTime? UpdatedDate { get; set; }

        [DttField(hidden: true)]
        public bool IsDeleted { get; set; }
        [DttField(hidden: true)]
        public bool IsDraft { get; set; }

        public FormModelBase() { }
        
    }
}