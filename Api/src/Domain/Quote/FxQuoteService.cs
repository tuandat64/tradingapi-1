using BTCS.Bank.Services.Core.Interfaces;
using Domain.BestExecution;
using Domain.FxOrders;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;

namespace Domain.Quote
{
    public class FxQuoteService : ServiceBase, IFxQuoteService
    {
        private readonly FxOrderService fxOrders;
        private readonly Account.IAccountService accounts;
        private readonly IOrderService orders;
        private readonly Currency.ICurrencyService currencies;

        public FxQuoteService(ILogger<FxQuoteService> logger,
                                Account.IAccountService accounts,
                                FxOrderService fxOrders,
                                IOrderService orders,
                                Currency.ICurrencyService currencies)
            : base(logger)
        {
            this.accounts = accounts;
            this.fxOrders = fxOrders;
            this.orders = orders;
            this.currencies = currencies;
        }

        public async Task<IResponse<FxQuoteResponse>> Create(IRequest<FxQuoteRequest> request)
        {
            if (request.ValidateEnvelope().HasError)
            {
                return InvalidRequest<FxQuoteResponse, FxQuoteRequest>(request);
            }

            FxOrderRequest domainRequest;

            try
            {
                domainRequest = await request.ToFxOrderRequest(accounts, currencies);
            }
            catch (Exception e)
            {
                return InvalidRequest<FxQuoteResponse, FxQuoteRequest>(request, e);
            }

            var domainResult = await fxOrders.CreateFxQuoteOrder(domainRequest);

            if (domainResult.StatusCode != 0)
            {
                var failed = request.CreateResponse(new FxQuoteResponse());
                failed.AddError($"Failed to create best execution order, {domainResult.StatusMessage}");
                return ProcessResponse(failed);
            }

            Logger.LogInformation("Created best execution order");

            //var trackingRequest = request.Map(
            //    v => new TrackOrder
            //    {
            //        TrackingNumber = domainResult.TrackingNumber
            //    });

            //var trackResponse = await Track(trackingRequest);

            //if (trackResponse.HasError)
            //{
            //    var failed = request.CreateResponse(new FxQuoteResponse());
            //    failed.AddError($"Created quote order, but failed to find/track the order with tracking number {domainResult.TrackingNumber}");
            //    return ProcessResponse(failed);
            //}

            var payload = await domainResult.Order.ToFxQuoteResponse(accounts, request.GetCustomerId());
            var response = request.CreateResponse(payload);
            return ProcessResponse(response);
        }

        public async Task<IResponse<FxExecuteQuoteResponse>> Execute(IRequest<FxExecuteQuoteRequest> request)
        {
            await Task.Yield();

            if (request.ValidateEnvelope().HasError)
            {
                return InvalidRequest<FxExecuteQuoteResponse, FxExecuteQuoteRequest>(request);
            }

            Guid domainRequest;
            try
            {
                domainRequest = request.Payload.ToDomainFxExecuteQuoteRequest();
            }
            catch (Exception e)
            {
                return InvalidRequest<FxExecuteQuoteResponse, FxExecuteQuoteRequest>(request, e);
            }

            var domainResult = orders.Process(domainRequest, false);

            if (!domainResult.Succeeded || domainResult.Order == null)
            {
                var failed = request.CreateResponse(new FxExecuteQuoteResponse());
                failed.AddError($"Order service failed to execute order with QuoteId {request.Payload.QuoteId}");
                return ProcessResponse(failed);
            }

            var result = await domainResult.Order.ToFxExecuteQuoteResponse(accounts, request.GetCustomerId());
            var response = request.CreateResponse(result);
            return ProcessResponse(response);
        }

        public async Task<IResponse<FxExecuteQuoteResponse>> Track(IRequest<TrackOrder> request)
        {
            await Task.Yield();

            if (request.ValidateEnvelope().HasError)
            {
                return InvalidRequest<FxExecuteQuoteResponse, TrackOrder>(request);
            }

            var domainResult = orders.GetByTrackingId(request.Payload.TrackingNumber);

            if (!domainResult.Succeeded ||
                domainResult.Order == null ||
                domainResult.Order.TrackingNumber != request.Payload.TrackingNumber)
            {
                var failed = request.CreateResponse(new FxExecuteQuoteResponse());
                failed.AddError($"Failed to find order with tracking number {request.Payload.TrackingNumber}");
                return ProcessResponse(failed);
            }

            var payload = await domainResult.Order.ToFxExecuteQuoteResponse(accounts, request.GetCustomerId());

            var response = request.CreateResponse(payload);

            return ProcessResponse(response);
        }
    }
}
