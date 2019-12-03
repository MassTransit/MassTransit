namespace MassTransit.Transports
{
    public delegate bool AllowTransportHeader(HeaderValue<string> headerValue);
}
