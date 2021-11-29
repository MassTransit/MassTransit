namespace MassTransit
{
    /// <summary>
    /// Update an existing payload, using the existing payload
    /// </summary>
    /// <param name="existing">The existing payload</param>
    /// <typeparam name="TPayload">The payload type</typeparam>
    public delegate TPayload UpdatePayloadFactory<TPayload>(TPayload existing)
        where TPayload : class;
}
