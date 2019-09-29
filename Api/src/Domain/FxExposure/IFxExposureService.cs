using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Domain.FxExposure
{
    public interface IFxExposureService
    {
        Task<IResponse<FxExposureResponse>> IsTradeable(IRequest<FxExposureRequest> request);
    }
}
