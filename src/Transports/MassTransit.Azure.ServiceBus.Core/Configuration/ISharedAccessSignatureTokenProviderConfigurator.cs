namespace MassTransit
{
    using Azure;


    public interface ISharedAccessSignatureTokenProviderConfigurator
    {
        AzureSasCredential SasCredential { set; }
    }
}
