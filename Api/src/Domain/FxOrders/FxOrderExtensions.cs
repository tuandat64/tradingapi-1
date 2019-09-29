using System;
using System.Threading.Tasks;
using BTCS.Bank.Services.Core.Model;
using Domain.Account;
using Trading.Foundation.Dtos;
using Enums = BTCS.Bank.Services.Core.Model.Enums;

namespace Domain.FxOrders
{
    internal static class FxOrderExtensions
    {
        public static decimal DefaultCommissionPercentage = 1.75M;

        public static Order ToLiquidityRequest(this FxOrderRequest model)
        {
            return new Order
            {
                Comments = model.Comments,
                CommissionPercentage = model.CommissionPercentage,
                Amount = model.Amount,
                Total = model.Total,
                BuySell = model.BuySell,
                LimitPrice = null,
                ExpiryDateTime = model.ExpiryDate,
                SourceAccountId = model.FromAccount.AccountId,
                TargetAccountId = model.ToAccount.AccountId,
                OrderTypeId = (int)Enums.OrderType.SliceOrder,
                OrderStateId = (int)Enums.OrderState.Created,
                ProceedsGoesToSegregated = false,
                FundsComeFromSegregated = false
            };
        }

        public static Order ToBestExecutionRequest(this FxOrderRequest model)
        {
            return new Order
            {
                Comments = model.Comments,
                CommissionPercentage = model.CommissionPercentage,
                Amount = model.Amount,
                Total = model.Total,
                BuySell = model.BuySell,
                LimitPrice = null,
                ExpiryDateTime = model.ExpiryDate,
                SourceAccountId = model.FromAccount.AccountId,
                TargetAccountId = model.ToAccount.AccountId,
                OrderTypeId = (int) Enums.OrderType.SliceOrder,
                OrderStateId = (int) Enums.OrderState.Processing,
                ProceedsGoesToSegregated = false,
                FundsComeFromSegregated = false
            };
        }

        public static Order ToQuoteRequest(this FxOrderRequest model)
        {
            return new Order
            {
                Comments = model.Comments,
                CommissionPercentage = model.CommissionPercentage,
                Amount = model.Amount,
                Total = model.Total,
                BuySell = model.BuySell,
                LimitPrice = null,
                ExpiryDateTime = model.ExpiryDate,
                SourceAccountId = model.FromAccount.AccountId,
                TargetAccountId = model.ToAccount.AccountId,
                OrderTypeId = (int) Enums.OrderType.MarketOrder,
                OrderStateId = (int) Enums.OrderState.Created,
                ProceedsGoesToSegregated = false,
                FundsComeFromSegregated = false
            };
        }

        public static FxOrderResult ToFxOrderResult(this Order order, int statusCode, string statusMessage)
        {
            return new FxOrderResult
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage,
                Order = order
            };
        }

        public static async Task<(Side, CurrencyPair, decimal, decimal, string)> GetSidePairBaseQuoteAndState(
            this BTCS.Bank.Services.Core.Model.Order model,
            IAccountService accounts,
            Guid customerId)
        {
            string baseCcy;
            string quoteCcy;
            decimal baseQuantity;
            decimal quoteQuantity;

            Side side = model.BuySell == 0 ? Side.Buy : Side.Sell;

            string fromCcy;
            string toCcy;

            if (model.ExchangeRate != null)
            {
                fromCcy = model.ExchangeRate.FromCurrencyId;
                toCcy = model.ExchangeRate.ToCurrencyId;
            }
            else
            {
                try
                {
                    var from = await accounts.FromAccountId(model.SourceAccountId, customerId);
                    fromCcy = from.Currency;
                    var to = await accounts.FromAccountId(model.TargetAccountId, customerId);
                    toCcy = to.Currency;
                }
                catch (Exception)
                {
                    fromCcy = "???";
                    toCcy = "???";
                }
            }

            if (side.IsBuy)
            {
                baseCcy = toCcy;
                quoteCcy = fromCcy;

                baseQuantity = model.Amount;
                quoteQuantity = model.Total;
            }
            else
            {
                baseCcy = fromCcy;
                quoteCcy = toCcy;

                baseQuantity = model.Total;
                quoteQuantity = model.Amount;
            }

            return (
                side,
                CurrencyPair.Parse(baseCcy + "-" + quoteCcy),
                baseQuantity,
                quoteQuantity,
                ((BTCS.Bank.Services.Core.Model.Enums.OrderState)model.OrderStateId).ToString()
                );
        }
    }
}
