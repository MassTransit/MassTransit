namespace MassTransit
{
    using Azure;


    public interface IServiceBusNamedKeyTokenProviderConfigurator
    {
        AzureNamedKeyCredential NamedKeyCredential { set; }
    }
}
