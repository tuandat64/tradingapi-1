using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;

namespace Domain.FxExposure
{
    public class FxExposureService : ServiceBase, IFxExposureService
    {
        public FxExposureService(ILogger<FxExposureService> logger)
            : base(logger)
        {

        }

        public async Task<IResponse<FxExposureResponse>> IsTradeable(IRequest<FxExposureRequest> request)
        {
            await Task.Yield();

            if (request.ValidateEnvelope().HasError)
            {
                return InvalidRequest<FxExposureResponse, FxExposureRequest>(request);
            }

            var payload = new FxExposureResponse
            {
                CanTrade = true
            };

            return ProcessResponse(request.CreateResponse(payload));
        }
    }
}
