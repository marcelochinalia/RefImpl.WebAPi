using Microsoft.Web.Http.Versioning;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Consinco.WebApi.Controllers
{
    public class ConsincoBaseController : ApiController
    {
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            IApiVersionReader headerApiVersion = new HeaderApiVersionReader("api-version");
            string versao = headerApiVersion.Read(controllerContext.Request);

            var ret = base.ExecuteAsync(controllerContext, cancellationToken);

            //ApiVersion apiVersion = ApiVersionReader();

            ret.Result.Headers.Add("Api-version", new string[] { "api-version:" + versao });

            return ret;
        }
    }

    
}
