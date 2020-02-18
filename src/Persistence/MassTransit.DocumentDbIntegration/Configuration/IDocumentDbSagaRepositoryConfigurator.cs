namespace MassTransit.DocumentDbIntegration
{
    using System;


    public interface IDocumentDbSagaRepositoryConfigurator<TSaga>
        where TSaga : class, IVersionedSaga
    {
        Uri EndpointUri { set; }
        string Key { set; }

        string DatabaseId { set; }

        string CollectionId { set; }

        /// <summary>
        /// Configure the EndpointUri and Key to use the Cosmos Emulator
        /// </summary>
        void ConfigureEmulator();
    }
}
