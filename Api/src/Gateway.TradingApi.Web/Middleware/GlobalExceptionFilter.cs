using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace Gateway.TradingApi.Web.Middleware
{
    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly ILogger _logger;

        public GlobalExceptionFilter(ILoggerFactory logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger.CreateLogger("Global Exception Filter");
        }

        public void Dispose()
        {

        }

        public void OnException(ExceptionContext context)
        {
            var response = new ErrorResponse()
            {
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500,
                DeclaredType = typeof(ErrorResponse)
            };

            _logger.LogError(context.Exception.ToString());
        }
    }
}
