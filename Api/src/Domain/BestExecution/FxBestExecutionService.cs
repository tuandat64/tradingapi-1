using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Domain.FxOrders;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;
using IAccountService = Domain.Account.IAccountService;
using ICurrencyService = Domain.Currency.ICurrencyService;

namespace Domain.BestExecution
{
    public class FxBestExecutionService : ServiceBase, IFxBestExecutionService
    {
        private readonly FxOrderService fxOrders;
        private readonly IAccountService accounts;
        private readonly ICurrencyService currencies;

        public FxBestExecutionService(
            ILogger<FxBestExecutionService> logger,
            IAccountService accounts,
            FxOrderService fxOrders,
            ICurrencyService currencies)
            : base(logger)
        {
            this.fxOrders = fxOrders;
            this.accounts = accounts;
            this.currencies = currencies;
        }

        public async Task<IResponse<FxBestExecutionResponse>> Create(IRequest<FxBestExecutionRequest> request)
        {
            if (request.ValidateEnvelope().HasEnvelopeError)
            {
                return InvalidRequest<FxBestExecutionResponse, FxBestExecutionRequest>(request);
            }

            FxOrderRequest domainModel;
            try
            {
                domainModel = await request.ToFxOrderRequest(accounts, currencies);
            }
            catch (Exception e)
            {
                return InvalidRequest<FxBestExecutionResponse, FxBestExecutionRequest>(request, e);
            }

            var domainResult = await fxOrders.CreateFxBestExecutionOrder(domainModel);

            if (domainResult.StatusCode != 0)
            {
                var failed = request.CreateResponse(new FxBestExecutionResponse());
                failed.AddError($"Failed to create best execution order, {domainResult.StatusMessage}");
                return ProcessResponse(failed);
            }

            //Logger.LogInformation("Created best execution order");

            //var trackingRequest = request.Map(
            //    v => new TrackOrder
            //    {
            //        TrackingNumber = domainResult.TrackingNumber
            //    });

            //var trackResponse = await Track(trackingRequest);

            //if (trackResponse.HasError)
            //{
            //    var failed = request.CreateResponse(new FxBestExecutionResponse());
            //    failed.AddError($"Created best execution order, but failed to find/track the order with tracking number {domainResult.TrackingNumber}");
            //    return ProcessResponse(failed);
            //}

            //return ProcessResponse(request.CreateResponse(trackResponse.Payload));

            var payload = await domainResult.Order.ToFxBestExecutionResponse(accounts, request.GetCustomerId());
            var response = request.CreateResponse(payload);
            return ProcessResponse(response);
        }

        //============================================================================================
        // order creation END
        //============================================================================================

        //============================================================================================
        // order tracking START
        //============================================================================================

        public async Task<IResponse<FxBestExecutionResponse>> Track(IRequest<TrackOrder> request)
        {
            if (request.ValidateEnvelope().HasEnvelopeError)
            {
                return InvalidRequest<FxBestExecutionResponse, TrackOrder>(request);
            }

            var domainResult = await fxOrders.TrackFxOrder(request.Payload.TrackingNumber);

            if (!domainResult.Succeeded ||
                domainResult.Order == null ||
                domainResult.Order.TrackingNumber != request.Payload.TrackingNumber)
            {
                var failed = request.CreateResponse(new FxBestExecutionResponse());
                failed.AddError($"Could not find order with tracking number {request.Payload.TrackingNumber}");
                return ProcessResponse(failed);
            }

            var payload = await domainResult.Order.ToFxBestExecutionResponse(accounts, request.GetCustomerId());
            var response = request.CreateResponse(payload);
            return ProcessResponse(response);
        }
    }
}
