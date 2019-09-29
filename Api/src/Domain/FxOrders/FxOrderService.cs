using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using BTCS.Bank.Services.Core.Model;
using PFS = BTCS.Bank.Services.Core;
using Domain.FxExposure;

namespace Domain.FxOrders
{
    public class FxOrderService
    {
        private readonly ILogger logger;
        private readonly Account.IAccountService accounts;
        private readonly IFxExposureService exposure;
        private readonly PFS.Interfaces.IOrderService orderService;
        private readonly PFS.Interfaces.ITradeService tradeService;

        public FxOrderService(
            ILogger<FxOrderService> logger,
            Account.IAccountService accounts,
            IFxExposureService exposure,
            PFS.Interfaces.IOrderService orderService,
            PFS.Interfaces.ITradeService tradeService
            )
        {
            this.logger = logger;
            this.accounts = accounts;
            this.exposure = exposure;
            this.orderService = orderService;
            this.tradeService = tradeService;
        }

        private FxOrderResult ExitError(string srcErrMsg, Exception e = null)
        {
            var errMsg = $"[FxOrderService] ERROR: {srcErrMsg}";

            logger.LogError(errMsg);
            if (e != null)
            {
                logger.LogError($"[FxOrderService] EXCEPTION: {e.ToString()}");
            }

            var response = new FxOrderResult
            {
                StatusMessage = errMsg,
                StatusCode = 1
            };

            return response;
        }

        internal async Task<PFS.Model.OrderResult> TrackFxOrder(string trackingNumber)
        {
            await Task.Yield();
            var orderResult = orderService.GetByTrackingId(trackingNumber);

            if (!orderResult.Succeeded) return orderResult;

            var tradesResult = tradeService.GetByRelatedOrderId((Guid?)orderResult.Order.Id);

            var trades = tradesResult.Trades;

            if (!tradesResult.Succeeded || !trades.Any()) return orderResult;

            var trade = trades.First();
            orderResult.Order.Description = trade.Description;
            orderResult.Order.Amount = trade.Amount;
            orderResult.Order.Total = trade.Total;

            return orderResult;
        }

        internal async Task<(string, Exception)> CheckAccountLiquidity(FxOrderRequest request)
        {
            await Task.Yield();

            PFS.Model.OrderResult orderResult;
            try
            {
                var domainOrder = request.ToLiquidityRequest();
                orderResult = orderService.Create(domainOrder);
            }
            catch (Exception e)
            {
                return ("could not confirm funds availability", e);
            }

            if (!orderResult.Succeeded)
            {
                return ($"could not check funds availability because {orderResult.Errors.ToString()}", null);
            }

            try
            {
                orderService.Delete(orderResult.Order.Id);
            }
            catch(Exception e)
            {
                return ("could not confirm funds availability (resource issue)", e);
            }

            if (orderResult.Order.Total > request.FromAccount.TradeableBalance)
            {
                return ($"not enough funds are available on account {request.FromAccount.AccountId}", null);
            }

            return ("", null);
        }

        internal async Task<(string, Exception)> CheckExposure(FxOrderRequest request)
        {
            await Task.Yield();
            return ("", null);
        }

        internal async Task<FxOrderResult> CreateFxBestExecutionOrder(FxOrderRequest request)
        {
            return await CreateFxOrder(request, true);
        }

        internal async Task<FxOrderResult> CreateFxQuoteOrder(FxOrderRequest request)
        {
            return await CreateFxOrder(request, false);
        }

        private async Task<FxOrderResult> CreateFxOrder(FxOrderRequest model, bool isBestExecution)
        {
            await Task.Yield();

            if (model == null)
            {
                return ExitError("empty request");
            }

            if (model.ExpiryDate < DateTime.Now.Date)
            {
                return ExitError("expiry date can't be before today");
            }

            var liquidityIssues = await CheckAccountLiquidity(model);

            if (liquidityIssues.Item1.Length > 0)
            {
                return ExitError($"could not create order because {liquidityIssues.Item1}", liquidityIssues.Item2);
            }

            var exposureIssues = await CheckExposure(model);
            if (exposureIssues.Item1.Length > 0)
            {
                return ExitError($"could not create order because {exposureIssues.Item1}", exposureIssues.Item2);
            }

            var domainOrder = isBestExecution ? model.ToBestExecutionRequest() : model.ToQuoteRequest();

            //try
            //{
            //    var businessStream = _businessStreamService.GetByEnumeration("MYPORTFOLIO");
            //    if (businessStream != null && businessStream.BusinessStream != null)
            //    {
            //        domainOrder.BusinessStreamId = businessStream.BusinessStream.Id;
            //    }
            //    else
            //    {
            //        return ExitError("could not get business stream Id");
            //    }

            //    var domainOrderJson = JsonConvert.SerializeObject(domainOrder, Formatting.Indented);
            //    logger.LogInformation(domainOrderJson);
            //}
            //catch (Exception e)
            //{
            //    return ExitError("could not get business stream Id", e);
            //}

            PFS.Model.OrderResult orderResult;

            try
            {
                orderResult = orderService.Create(domainOrder);
            }
            catch (Exception e)
            {
                return ExitError($"could not create order because {e.Message}", e);
            }

            if (!orderResult.Succeeded)
            {
                return ExitError($"could not create order because {orderResult.Errors.ToString()}");
            }

            return orderResult.Order.ToFxOrderResult(0, "success");
        }
    }
}
