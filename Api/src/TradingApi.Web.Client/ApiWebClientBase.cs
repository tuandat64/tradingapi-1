using NLog;
using System;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;

namespace TradingApi.Web.Client
{
    public class ApiWebClientBase
    {
        public static Guid CustomerId = Guid.Parse("0726DEF5-3E84-4FA7-93C0-18B4EFD379CD");

        public static string VsUrl => "https://localhost:44300";

        public static string LocalUrl => "http://local-api-trading.bitcoinsuisse.com";

        public static string QaSandboxUrl => "https://qa-api-trading-sandbox.bitcoinsuisse.com";

        public static string QaUrl => "https://qa-api-trading.bitcoinsuisse.com";

        public ServiceClient Client { get; }
        protected ILogger Logger { get; }

        public ApiWebClientBase()
        {
            //Logger = output.GetNLogLogger();
            Client = new ServiceClient(LocalUrl);
        }

        public ApiWebClientBase(string url)
        {
            //Logger = output.GetNLogLogger();
            Client = new ServiceClient(url);
        }

        protected void Log(string m)
        {
            //Logger.Info(m);
        }

        protected void AddClientInfo<T>(Request<T> request)
        {
            request.AddValue("test client", "local");
        }

        protected Request<T> Configure<T>(Request<T> request)
        {
            request.AddCustomerId(CustomerId);
            AddClientInfo(request);
            return request;
        }

        protected Request<NullPayload> ConfiguredNullRequest
        {
            get
            {
                var request = Configure((Request<NullPayload>)Requests.NullRequest);
                request.AddCustomerId(CustomerId);
                AddClientInfo(request);
                return request;
            }
        }

        protected Response<T> GetResponse<T>()
        {
            return Client.Response<Response<T>>();
        }
    }
}
