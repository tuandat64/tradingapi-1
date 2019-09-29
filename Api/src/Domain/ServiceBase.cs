using System;
using Microsoft.Extensions.Logging;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;

namespace Domain
{
    public abstract class ServiceBase
    {
        protected readonly ILogger Logger;
        protected static readonly string CommandKey = "Command";

        protected ServiceBase(ILogger log)
        {
            this.Logger = log;
        }

        /// <summary>
        /// Logs a request, and returns a non-null request
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="request"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public Request<TIn> Enrich<TIn>(Request<TIn> request, string method)
        {
            return Enrich(request, method, Logger);
        }

        /// <summary>
        /// Logs a request, and returns a non-null request
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="request"></param>
        /// <param name="command"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static Request<TIn> Enrich<TIn>(Request<TIn> request, string command, ILogger log)
        {
            if (request == null)
            {
                request = new Request<TIn>();
                request.AddError("empty request");
            }

            request.AddCorrelationId(Guid.NewGuid());
            request.AddBusinessTransactionId(Guid.NewGuid());
            request.Properties.TryAdd(CommandKey, command);

            log.LogInformation(request.ToString());
            return request;
        }

        public static TOut Strip<TIn, TOut>(IEnvelope<TIn> packet)
        {
            return (TOut)packet;
        }

        public IResponse<TResponse> ProcessResponse<TResponse>(IResponse<TResponse> iResponse)
        {
            return ProcessResponse(Logger, iResponse);
        }

        public static IResponse<TResponse> ProcessResponse<TResponse>(ILogger log, IResponse<TResponse> response)
        {
            if (response.ValidateEnvelope().HasError)
            {
                log.LogError(response.ToString());
            }
            else
            {
                log.LogInformation(response.ToString());
            }
            return response;
        }

        protected IResponse<TResponse> InvalidRequest<TResponse, TRequest>(IRequest<TRequest> request, Exception exception = null)
            where TResponse : class, new()
        {
            if (exception != null)
            {
                Logger.LogError(exception.ToString());
            }
            return InvalidRequest<TResponse, TRequest>(Logger, request, exception?.Message);
        }

        public static IResponse<TResponse> InvalidRequest<TResponse, TRequest>(ILogger log, IRequest<TRequest> request, string error = null)
        where TResponse : class, new()
        {
            try
            {
                var response = request.CreateResponse(new TResponse());
                response.AddError(error ?? "Invalid Request");
                return ProcessResponse(log, response);
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }

            var barfed = new Response<TResponse>();
            barfed.AddError("Invalid Request - internal error when creating error response from request envelope");
            return ProcessResponse(log, barfed);
        }
    }
}
