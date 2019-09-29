namespace Gateway.TradingApi.Queue.Model.Settings
{
    public class RabbitMqSettings
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FxOrderRequestEndPoint { get; set; }
    }
}
