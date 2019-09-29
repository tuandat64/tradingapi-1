using System;
using System.Threading.Tasks;
using Trading.Foundation.Protocol;

namespace Gateway.ApiAccess
{
    public interface IApiAuthorizeService
    {
        Task<IResponse<ApiAccessRequest>> AuthApiAccessAsync(IRequest<ApiAccessRequest> request);
    }

    public class ApiAccessRequest
    {
        public string ApiKey { get; set; }
        public string ApiPassphrase { get; set; }
        public string ApiSignature { get; set; }
        public long ApiNonce { get; set; }
        public string ApiMethod { get; set; }
        public string ApiPath { get; set; }
    }

    public class AuthApiAccessResponse
    {
        public Guid CustomerId { get; set; }
    }
}
