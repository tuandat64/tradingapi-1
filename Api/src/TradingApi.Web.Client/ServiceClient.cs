using Newtonsoft.Json;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace TradingApi.Web.Client
{
    using RequestOfNullPayload = IRequest<NullPayload>;
    using RequestOfTrackOrder = IRequest<TrackOrder>;
    using RequestOfFxBestExecutionRequest = IRequest<FxBestExecutionRequest>;
    using RequestOfFxQuoteRequest = IRequest<FxQuoteRequest>;
    using RequestOfFxExecuteQuoteRequest = IRequest<FxExecuteQuoteRequest>;

    public partial class ServiceClient
    {
        public string JsonResponse { get; set; }

        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
        {
            JsonResponse = "";

            JsonResponse = response.Content.ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult();
        }

        public T Response<T>()
        {
            return JsonConvert.DeserializeObject<T>(JsonResponse);
        }
    }
}
