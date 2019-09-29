namespace Gateway.TradingApi.Web.Middleware
{
    internal class ErrorResponse
    {
        public ErrorResponse()
        {
        }

        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}