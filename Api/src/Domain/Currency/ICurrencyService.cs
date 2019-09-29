using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Domain.Currency
{
    public interface ICurrencyService
    {
        Task<IResponse<List<CurrencyPair>>> GetCurrencyPairs(IRequest<NullPayload> request);
        
        Task<IResponse<List<FxIndicativeExchangeRate>>> GetIndicativeFxExchangeRates(IRequest<NullPayload> request);
    }
}
