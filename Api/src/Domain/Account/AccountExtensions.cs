using Trading.Foundation.Dtos;
using PFS = BTCS.Bank.Services.Core;

namespace Domain.Account
{
    static class AccountExtensions
    {
        public static TradeableAccount TradeableAccount(this PFS.Model.Account model)
        {
            return new TradeableAccount
            {
                AccountId = model.Id,
                AccountNumber = model.Number,
                Nickname = model.NickName ?? "",
                Currency = model.CurrencyId,
                TotalBalance = model.Balance,
                TradeableBalance = model.BalanceAvailable
            };
        }
    }
}
