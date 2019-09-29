using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Domain.Quote
{
    public interface IFxQuoteService
    {
        Task<IResponse<FxQuoteResponse>> Create(IRequest<FxQuoteRequest> model);
        Task<IResponse<FxExecuteQuoteResponse>> Execute(IRequest<FxExecuteQuoteRequest> model);
        Task<IResponse<FxExecuteQuoteResponse>> Track(IRequest<TrackOrder> request);
    }
}
