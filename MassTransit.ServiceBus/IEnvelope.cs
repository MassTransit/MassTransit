namespace MassTransit.ServiceBus
{
    /// <summary>
    /// A public interface to the envelope containing message(s)
    /// </summary>
    public interface IEnvelope
    {
        /// <summary>
        /// The messages contained in the envelope
        /// </summary>
        IMessage[] Messages { get; set; }

        /// <summary>
        /// The return endpoint for the message(s) in the envelope
        /// </summary>
        IEndpoint ReturnTo { get; }

        string Id { get; set; }
    }
}