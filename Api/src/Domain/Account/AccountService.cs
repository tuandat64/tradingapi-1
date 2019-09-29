using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;
using PFS = BTCS.Bank.Services.Core;

namespace Domain.Account
{
    public class AccountService : ServiceBase, IAccountService
    {
        readonly PFS.Interfaces.IAccountService _pfsAccountService;

        public AccountService(PFS.Interfaces.IAccountService pfsAccountService, ILogger<AccountService> logger)
            : base(logger)
        {
            _pfsAccountService = pfsAccountService;
        }

        public async Task<IResponse<TradeableAccount>> GetAccountFromAccountNumber(string accountNumber, Guid customerId)
        {
            var result = await ResolveTradeableAccounts(customerId);

            var account = from acc in result
                          where acc.AccountNumber.Equals(accountNumber)
                          select acc;

            var found = account.First();

            if (found != null)
            {
                return new Response<TradeableAccount> { Payload = found };
            }

            var err = new Response<TradeableAccount>();
            err.AddError($"No account found with account number ${accountNumber}");
            return err;
        }

        public async Task<IResponse<TradeableAccount>> GetAccountFromCurrency(string currency, Guid customerId)
        {
            var result = await ResolveTradeableAccounts(customerId);

            var account = from acc in result
                          where acc.Currency.Equals(currency)
                          select acc;

            var found = account.First();

            if (found != null)
            {
                return new Response<TradeableAccount> { Payload = found };
            }

            var err = new Response<TradeableAccount>();
            err.AddError($"No account found with currency ${currency}");
            return err;
        }

        public async Task<IResponse<TradeableAccount>> GetAccountFromAccountId(Guid accountId, Guid customerId)
        {
            var result = await ResolveTradeableAccounts(customerId);

            var account = from acc in result
                          where acc.AccountId.Equals(accountId)
                          select acc;

            var found = account.First();

            if (found != null)
            {
                return new Response<TradeableAccount> {Payload = found};
            }

            var err = new Response<TradeableAccount>();
            err.AddError($"No account found with id ${accountId}");
            return err;
        }

        private async Task<List<TradeableAccount>> ResolveTradeableAccounts(Guid customerId)
        {
            var result = await _pfsAccountService.GetByCustomerAsync(customerId, (int)PFS.Model.Enums.AccountTypeEnum.TRADING);
            return result.Accounts.Select(a => a.TradeableAccount()).ToList();
        }

        public async Task<IResponse<List<TradeableAccount>>> GetTradeableAccounts(IRequest<NullPayload> request)
        {
            if (request.ValidateEnvelope().HasEnvelopeError)
            {
                return InvalidRequest<List<TradeableAccount>, NullPayload>(request);
            }

            var response = request.CreateResponse(await ResolveTradeableAccounts(request.GetCustomerId()));
            return response;
        }

        public async Task<TradeableAccount> FromCurrency(string currency, Guid customerId)
        {
            var result = await GetAccountFromCurrency(currency, customerId);
            if (result.HasError)
            {
                throw new Exception($"Internal error when getting account from currency {currency} for customerId {customerId}");
            }
            return result.Payload;
        }

        public async Task<TradeableAccount> FromAccountNumber(string accountNumber, Guid customerId)
        {
            var result = await GetAccountFromAccountNumber(accountNumber, customerId);
            if (result.HasError)
            {
                throw new Exception($"Internal error when getting account from account number {accountNumber} for customerId {customerId}");
            }
            return result.Payload;
        }

        public async Task<TradeableAccount> FromAccountId(Guid accountId, Guid customerId)
        {
            var result = await GetAccountFromAccountId(accountId, customerId);
            if (result.HasError)
            {
                throw new Exception($"Internal error when getting account from account id {accountId} for customerId {customerId}");
            }

            return result.Payload;
        }
    }
}
