namespace MassTransit.ServiceBus
{
    public static class EnvelopeFactory
    {
        public static IEnvelope NewEnvelope(params IMessage[] messages)
        {
            Envelope e = new Envelope();

            e.Messages = messages;

            return e;
        }

        public static IEnvelope NewEnvelope(IEndpoint returnTo, params IMessage[] messages)
        {
            Envelope e = new Envelope();

            e.ReturnTo = returnTo;

            e.Messages = messages;

            return e;
        }
    }
}