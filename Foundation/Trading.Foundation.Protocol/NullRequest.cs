namespace Trading.Foundation.Protocol
{
    public static class Requests
    {
        public static IRequest<NullPayload> NullRequest =>
            new Request<NullPayload>
            {
                Payload = NullPayload.NullObject
            };
    }
}
