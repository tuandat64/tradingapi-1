using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Domain.BestExecution
{
    public interface IFxBestExecutionService
    {
        Task<IResponse<FxBestExecutionResponse>> Create(IRequest<FxBestExecutionRequest> model);
        Task<IResponse<FxBestExecutionResponse>> Track(IRequest<TrackOrder> request);
    }
}
