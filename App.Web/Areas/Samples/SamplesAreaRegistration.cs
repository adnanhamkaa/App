using App.Web.Utilities;
using System.Web.Mvc;

namespace App.Web.Areas.Samples
{
    public class SamplesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Samples";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            this.RegisterDefaultAreaRoute(context);
        }
    }
}