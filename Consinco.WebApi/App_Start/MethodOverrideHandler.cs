using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System;

namespace Consinco.WebApi
{
    // Esse classe atende a diretriz de arquitetura para que a Web Api seja
    // executa por clients que ainda fazem uso de protocolo HTTP 1.0
    public class MethodOverrideHandler : DelegatingHandler
    {
        readonly string[] _methods = { "DELETE", "HEAD", "PUT", "PATCH" };
        const string _header = "X-HTTP-Method-Override";

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Checa se na requisição HTTP POST possui a variável X-HTTP-Method-Override no header da request.
            if (request.Method == HttpMethod.Post && request.Headers.Contains(_header))
            {
                // Checa se o valor passado no header é um dos métodos da lista _methods.
                var method = request.Headers.GetValues(_header).FirstOrDefault();
                if (_methods.Contains(method, StringComparer.InvariantCultureIgnoreCase))
                {
                    // Faz a troca para o método corresponde da Web Api
                    request.Method = new HttpMethod(method);
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}