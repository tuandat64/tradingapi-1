using Domain.Account;
using Domain.Currency;
using Domain.FxOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;
using PFS = BTCS.Bank.Services.Core;

namespace Domain.Quote
{
    public static class FxQuoteServiceExtensions
    {
        public static async Task Validate(this IRequest<FxQuoteRequest> request, ICurrencyService currencies)
        {
            var model = request.Payload;

            var errors = new List<string>();

            if (model.Quantity <= 0)
            {
                errors.Add("quantity must be > 0");
            }

            if (string.IsNullOrWhiteSpace(model.Pair.Base))
            {
                errors.Add("empty base currency");
            }

            if (string.IsNullOrWhiteSpace(model.Pair.Quote))
            {
                errors.Add("empty quote currency");
            }

            var ccyRequest = request.Map(v => NullPayload.NullObject);

            var ccyResponse = await currencies.GetCurrencyPairs(ccyRequest);

            var ccyPairs = from pairs in ccyResponse.Payload
                           where pairs.Equals(model.Pair)
                           select pairs;

            if (!ccyPairs.Any())
            {
                errors.Add($"invalid currency pair {model.Pair.ToString()}");
            }

            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(",", errors.ToArray().Select(p => p.ToString()).ToArray()));
            }
        }

        internal static async Task<FxOrderRequest> ToFxOrderRequest(
            this IRequest<FxQuoteRequest> request,
            IAccountService accounts,
            ICurrencyService currencies)
        {
            await request.Validate(currencies);

            var payload = request.Payload;

            var baseAccount = await accounts.FromCurrency(payload.Pair.Base, request.GetCustomerId());
            var quoteAccount = await accounts.FromCurrency(payload.Pair.Quote, request.GetCustomerId());

            if (payload.Side.IsBuy)
            {
                return new FxOrderRequest
                {
                    FromAccount = quoteAccount,
                    ToAccount = baseAccount,
                    ExpiryDate = DateTime.Now,
                    Amount = payload.Quantity,
                    BuySell = 0,
                    CommissionPercentage = FxOrderExtensions.DefaultCommissionPercentage
                };
            }

            return new FxOrderRequest
            {
                FromAccount = baseAccount,
                ToAccount = quoteAccount,
                ExpiryDate = DateTime.Now,
                Total = payload.Quantity,
                BuySell = 1,
                CommissionPercentage = FxOrderExtensions.DefaultCommissionPercentage
            };
        }

        public static async Task<FxQuoteResponse> ToFxQuoteResponse(
            this PFS.Model.Order model,
            IAccountService accounts,
            Guid customerId)
        {
            if (model == null)
            {
                return null;
            }

            long ticks = 0;

            if (model.ExpiryDateTime.HasValue)
            {
                ticks = model.ExpiryDateTime.Value.Ticks;
            }

            var orderInfo = await model.GetSidePairBaseQuoteAndState(accounts, customerId);

            return new FxQuoteResponse
            {
                QuoteId = model.Id.ToString(),
                Side = orderInfo.Item1,
                Pair = orderInfo.Item2,
                BaseQuantity = orderInfo.Item3,
                QuoteQuantity = orderInfo.Item4,
                CommissionPercentage = model.CommissionPercentage ?? 0,
                ExpiryTimestamp = ticks
            };
        }

        public static Guid ToDomainFxExecuteQuoteRequest(this FxExecuteQuoteRequest model)
        {
            if (model == null)
            {
                throw new Exception("null model");
            }

            return Guid.Parse(model.QuoteId);
        }

        public static async Task<FxExecuteQuoteResponse> ToFxExecuteQuoteResponse(
            this BTCS.Bank.Services.Core.Model.Order model,
            IAccountService accounts,
            Guid customerId)
        {
            if (model == null)
            {
                return null;
            }

            var orderInfo = await model.GetSidePairBaseQuoteAndState(accounts, customerId);

            return new FxExecuteQuoteResponse
            {
                State = orderInfo.Item5,
                Side = orderInfo.Item1,
                Pair = orderInfo.Item2,
                TrackingNumber = model.TrackingNumber,
                BaseQuantity = orderInfo.Item3,
                QuoteQuantity = orderInfo.Item4,
                CommissionPercentage = model.CommissionPercentage ?? 0,
                Description = model.Description
            };
        }
    }
}
