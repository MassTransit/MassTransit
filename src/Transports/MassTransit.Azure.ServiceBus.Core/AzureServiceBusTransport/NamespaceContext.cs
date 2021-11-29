namespace MassTransit.AzureServiceBusTransport
{
    /// <summary>
    /// A service bus namespace which has the appropriate messaging factories available
    /// </summary>
    public interface NamespaceContext :
        PipeContext
    {
        ConnectionContext ConnectionContext { get; }
    }
}
