using System.Net.Http;
using System.Web.Http;

namespace App.Web.Utilities
{
    public class ApiBaseController : ApiController {

        protected HttpResponseMessage CreateResponse<T>(App.Web.Models.MessageResponseDto<T> message) where T : new() {
            HttpResponseMessage result = null;
            switch (message.StatusCode) {
                case Models.StatusCode.success:
                    result = Request.CreateResponse(System.Net.HttpStatusCode.OK, message);
                    break;
                case Models.StatusCode.failed:
                    result = Request.CreateResponse(System.Net.HttpStatusCode.NoContent, message);
                    break;
                case Models.StatusCode.error:
                    result = Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, message);
                    break;
                default:
                    result = Request.CreateResponse(System.Net.HttpStatusCode.NoContent, message);
                    break;
            }

            return result;
        }

    }
}