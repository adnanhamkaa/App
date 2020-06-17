using DA = System.ComponentModel.DataAnnotations;

namespace App.Web.Utilities
{
    public class AppRequiredAttribute : DA.RequiredAttribute
    {
        public AppRequiredAttribute(): base() {
            ErrorMessage = "Field {0} tidak boleh kosong";
        }
    }
}