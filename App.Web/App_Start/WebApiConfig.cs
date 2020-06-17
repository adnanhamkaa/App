
using System.Net.Http.Formatting;
using System.Web.Http;

namespace App.Web {

    public class WebApiConfig {
        public static void Register(HttpConfiguration configuration) {
            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });

            //configuration.MapHttpAttributeRoutes();

            configuration.Formatters.Add(new JsonMediaTypeFormatter());
            configuration.Formatters.Add(new XmlMediaTypeFormatter());

        }
    }
}