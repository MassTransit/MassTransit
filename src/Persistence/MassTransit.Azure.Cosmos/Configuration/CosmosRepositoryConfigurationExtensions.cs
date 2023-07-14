namespace MassTransit
{
    using System;
    using Azure.Core;
    using AzureCosmos;
    using Configuration;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Serialization;


    public static class CosmosRepositoryConfigurationExtensions
    {
        /// <summary>
        /// Configures the Cosmos Saga Repository
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="authSettings"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        static ISagaRegistrationConfigurator<TSaga> CosmosRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            CosmosAuthSettings authSettings,
            Action<ICosmosSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new CosmosSagaRepositoryConfigurator<TSaga>(authSettings);

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The CosmosDb saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Configures the Cosmos Saga Repository
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> CosmosRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<ICosmosSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            return configurator.CosmosRepository(new CosmosAuthSettings(), configure);
        }

        /// <summary>
        /// Configures the Cosmos Saga Repository.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="authKeyOrResourceToken">The cosmos account key or resource token to use to create the client.</param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> CosmosRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            string accountEndpoint, string authKeyOrResourceToken, Action<ICosmosSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            return configurator.CosmosRepository(new CosmosAuthSettings(accountEndpoint, authKeyOrResourceToken), configure);
        }

        /// <summary>
        /// Configures the Cosmos Saga Repository.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">
        /// The connection string to the cosmos account. ex:
        /// AccountEndpoint=https://XXXXX.documents.azure.com:443/;AccountKey=SuperSecretKey;
        /// </param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> CosmosRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            string connectionString, Action<ICosmosSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            return configurator.CosmosRepository(new CosmosAuthSettings(connectionString), configure);
        }

        /// <summary>
        /// Configures the Cosmos Saga Repository.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="tokenCredential"><see cref="TokenCredential" />The token to provide AAD token for authorization</param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> CosmosRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            string accountEndpoint, TokenCredential tokenCredential, Action<ICosmosSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            return configurator.CosmosRepository(new CosmosAuthSettings(accountEndpoint, tokenCredential), configure);
        }

        /// <summary>
        /// Configure the Job Service saga state machines to use Azure Cosmos
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="authKeyOrResourceToken">The cosmos account key or resource token to use to create the client.</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IJobSagaRegistrationConfigurator CosmosRepository(this IJobSagaRegistrationConfigurator configurator, string accountEndpoint,
            string authKeyOrResourceToken, Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            var registrationProvider = new CosmosSagaRepositoryRegistrationProvider(x =>
            {
                x.AccountEndpoint = accountEndpoint;
                x.AuthKeyOrResourceToken = authKeyOrResourceToken;

                configure?.Invoke(x);
            });

            configurator.UseRepositoryRegistrationProvider(registrationProvider);

            return configurator;
        }

        /// <summary>
        /// Configure the Job Service saga state machines to use Azure Cosmos
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">
        /// The connection string to the cosmos account. ex:
        /// AccountEndpoint=https://XXXXX.documents.azure.com:443/;AccountKey=SuperSecretKey;
        /// </param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IJobSagaRegistrationConfigurator CosmosRepository(this IJobSagaRegistrationConfigurator configurator, string connectionString,
            Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            var registrationProvider = new CosmosSagaRepositoryRegistrationProvider(x =>
            {
                x.ConnectionString = connectionString;

                configure?.Invoke(x);
            });

            configurator.UseRepositoryRegistrationProvider(registrationProvider);

            return configurator;
        }

        /// <summary>
        /// Configure the Job Service saga state machines to use Azure Cosmos
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="tokenCredential"><see cref="TokenCredential" />The token to provide AAD token for authorization.</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IJobSagaRegistrationConfigurator CosmosRepository(this IJobSagaRegistrationConfigurator configurator, string accountEndpoint,
            TokenCredential tokenCredential, Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            var registrationProvider = new CosmosSagaRepositoryRegistrationProvider(x =>
            {
                x.AccountEndpoint = accountEndpoint;
                x.TokenCredential = tokenCredential;

                configure?.Invoke(x);
            });

            configurator.UseRepositoryRegistrationProvider(registrationProvider);

            return configurator;
        }

        /// <summary>
        /// Configure the Job Service saga state machines to use Azure Cosmos
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IJobSagaRegistrationConfigurator CosmosRepository(this IJobSagaRegistrationConfigurator configurator,
            Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            var registrationProvider = new CosmosSagaRepositoryRegistrationProvider(configure);

            configurator.UseRepositoryRegistrationProvider(registrationProvider);

            return configurator;
        }

        /// <summary>
        /// Use the Cosmos saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="authKeyOrResourceToken">The cosmos account key or resource token to use to create the client.</param>
        /// <param name="configure"></param>
        public static void SetCosmosSagaRepositoryProvider(this IRegistrationConfigurator configurator, string accountEndpoint, string authKeyOrResourceToken,
            Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new CosmosSagaRepositoryRegistrationProvider(x =>
            {
                x.AccountEndpoint = accountEndpoint;
                x.AuthKeyOrResourceToken = authKeyOrResourceToken;

                configure?.Invoke(x);
            }));
        }

        /// <summary>
        /// Use the Cosmos saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">
        /// The connection string to the cosmos account. ex:
        /// AccountEndpoint=https://XXXXX.documents.azure.com:443/;AccountKey=SuperSecretKey;
        /// </param>
        /// <param name="configure"></param>
        public static void SetCosmosSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString,
            Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new CosmosSagaRepositoryRegistrationProvider(x =>
            {
                x.ConnectionString = connectionString;

                configure?.Invoke(x);
            }));
        }

        /// <summary>
        /// Use the Cosmos saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="tokenCredential"><see cref="TokenCredential" />The token to provide AAD token for authorization.</param>
        /// <param name="configure"></param>
        public static void SetCosmosSagaRepositoryProvider(this IRegistrationConfigurator configurator, string accountEndpoint, TokenCredential tokenCredential,
            Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new CosmosSagaRepositoryRegistrationProvider(x =>
            {
                x.AccountEndpoint = accountEndpoint;
                x.TokenCredential = tokenCredential;

                configure?.Invoke(x);
            }));
        }

        /// <summary>
        /// Use the Cosmos saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetCosmosSagaRepositoryProvider(this IRegistrationConfigurator configurator, Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new CosmosSagaRepositoryRegistrationProvider(configure));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods. This also uses the System.Text.Json serializer. If you need to use Newtonsoft,
        /// call <see cref="AddNewtonsoftCosmosClientFactory(IServiceCollection, CosmosAuthSettings)" /> instead.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="authSettings"></param>
        static IServiceCollection AddCosmosClientFactory(this IServiceCollection collection, CosmosAuthSettings authSettings)
        {
            return collection.AddSingleton<ICosmosClientFactory>(provider =>
                new SystemTextJsonCosmosClientFactory(authSettings, provider.GetRequiredService<IOptions<CosmosClientOptions>>(),
                    SystemTextJsonMessageSerializer.Options.PropertyNamingPolicy));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods. This also uses the System.Text.Json serializer. If you need to use Newtonsoft,
        /// call <see cref="AddNewtonsoftCosmosClientFactory(IServiceCollection, string, string)" /> instead.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="authKeyOrResourceToken">The cosmos account key or resource token to use to create the client.</param>
        public static IServiceCollection AddCosmosClientFactory(this IServiceCollection collection, string accountEndpoint, string authKeyOrResourceToken)
        {
            return collection.AddCosmosClientFactory(new CosmosAuthSettings(accountEndpoint, authKeyOrResourceToken));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods. This also uses the System.Text.Json serializer. If you need to use Newtonsoft,
        /// call <see cref="AddNewtonsoftCosmosClientFactory(IServiceCollection, string)" /> instead.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="connectionString">
        /// The connection string to the cosmos account. ex:
        /// AccountEndpoint=https://XXXXX.documents.azure.com:443/;AccountKey=SuperSecretKey;
        /// </param>
        public static IServiceCollection AddCosmosClientFactory(this IServiceCollection collection, string connectionString)
        {
            return collection.AddCosmosClientFactory(new CosmosAuthSettings(connectionString));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods. This also uses the System.Text.Json serializer. If you need to use Newtonsoft,
        /// call <see cref="AddNewtonsoftCosmosClientFactory(IServiceCollection, string, TokenCredential)" /> instead.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="tokenCredential"><see cref="TokenCredential" />The token to provide AAD token for authorization.</param>
        public static IServiceCollection AddCosmosClientFactory(this IServiceCollection collection, string accountEndpoint, TokenCredential tokenCredential)
        {
            return collection.AddCosmosClientFactory(new CosmosAuthSettings(accountEndpoint, tokenCredential));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="authSettings"></param>
        static IServiceCollection AddNewtonsoftCosmosClientFactory(this IServiceCollection collection, CosmosAuthSettings authSettings)
        {
            return collection.AddSingleton<ICosmosClientFactory>(provider =>
                new NewtonsoftJsonCosmosClientFactory(authSettings, provider.GetRequiredService<IOptions<CosmosClientOptions>>()));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="authKeyOrResourceToken">The cosmos account key or resource token to use to create the client.</param>
        public static IServiceCollection AddNewtonsoftCosmosClientFactory(this IServiceCollection collection, string accountEndpoint,
            string authKeyOrResourceToken)
        {
            return collection.AddNewtonsoftCosmosClientFactory(new CosmosAuthSettings(accountEndpoint, authKeyOrResourceToken));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="connectionString">
        /// The connection string to the cosmos account. ex:
        /// AccountEndpoint=https://XXXXX.documents.azure.com:443/;AccountKey=SuperSecretKey;
        /// </param>
        public static IServiceCollection AddNewtonsoftCosmosClientFactory(this IServiceCollection collection, string connectionString)
        {
            return collection.AddNewtonsoftCosmosClientFactory(new CosmosAuthSettings(connectionString));
        }

        /// <summary>
        /// Add the MassTransit Cosmos Client Factory to the service collection, using the specified parameters. This is option when using configuring the
        /// saga repository using the AddSaga methods.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="accountEndpoint">The cosmos service endpoint to use</param>
        /// <param name="tokenCredential"><see cref="TokenCredential" />The token to provide AAD token for authorization.</param>
        public static IServiceCollection AddNewtonsoftCosmosClientFactory(this IServiceCollection collection, string accountEndpoint,
            TokenCredential tokenCredential)
        {
            return collection.AddNewtonsoftCosmosClientFactory(new CosmosAuthSettings(accountEndpoint, tokenCredential));
        }
    }
}
