using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Domain.Account
{
    public interface IAccountService
    {
        Task<IResponse<List<TradeableAccount>>> GetTradeableAccounts(IRequest<NullPayload> request);
        Task<TradeableAccount> FromCurrency(string currency, Guid customerId);
        Task<TradeableAccount> FromAccountNumber(string accountNumber, Guid customerId);
        Task<TradeableAccount> FromAccountId(Guid accountId, Guid customerId);
    }
}