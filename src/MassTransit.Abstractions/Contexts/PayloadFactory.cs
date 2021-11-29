namespace MassTransit
{
    public delegate TPayload PayloadFactory<out TPayload>()
        where TPayload : class;
}
