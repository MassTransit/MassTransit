namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using Azure.Core;
    using AzureCosmos;
    using AzureCosmos.Saga;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using Saga;
    using Serialization;


    public class CosmosSagaRepositoryConfigurator<TSaga> :
        ICosmosSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        readonly CosmosAuthSettings _settings;
        string _clientName;
        Func<IServiceProvider, ICosmosCollectionIdFormatter> _collectionIdFormatter;
        Action<ItemRequestOptions> _itemRequestOptions;
        Action<CosmosLinqSerializerOptions> _linqSerializerOptions;
        Action<QueryRequestOptions> _queryRequestOptions;
        Action<ISagaRepositoryRegistrationConfigurator<TSaga>> _registerClientFactory;

        public CosmosSagaRepositoryConfigurator(CosmosAuthSettings settings = null)
        {
            _collectionIdFormatter = _ => KebabCaseCollectionIdFormatter.Instance;
            _registerClientFactory = RegisterSystemTextJsonClientFactory;
            _settings = settings ?? new CosmosAuthSettings();

            PropertyNamingPolicy = SystemTextJsonMessageSerializer.Options.PropertyNamingPolicy;
        }

        public JsonNamingPolicy PropertyNamingPolicy { private get; set; }

        public void ConfigureEmulator()
        {
            AccountEndpoint = AzureCosmosEmulatorConstants.AccountEndpoint;
            AuthKeyOrResourceToken = AzureCosmosEmulatorConstants.AccountKey;
        }

        public void SetCollectionIdFormatter(ICosmosCollectionIdFormatter collectionIdFormatter)
        {
            if (collectionIdFormatter == null)
                throw new ArgumentNullException(nameof(collectionIdFormatter));
            SetCollectionIdFormatter(_ => collectionIdFormatter);
        }

        public string DatabaseId { get; set; }

        public string CollectionId
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(value));

                _collectionIdFormatter = _ => new DefaultCollectionIdFormatter(value);
            }
        }

        public string AccountEndpoint
        {
            set => _settings.AccountEndpoint = value;
        }

        public string AuthKeyOrResourceToken
        {
            set
            {
                if (_settings.AuthKeyOrResourceToken != null
                    || _settings.ConnectionString != null)
                    throw new ArgumentException("Another type of authentication is already being used");

                _settings.AuthKeyOrResourceToken = value;
            }
        }

        public string ConnectionString
        {
            set
            {
                if (_settings.AuthKeyOrResourceToken != null
                    || _settings.TokenCredential != null)
                    throw new ArgumentException("Another type of authentication is already being used");

                _settings.ConnectionString = value;
            }
        }

        [Obsolete("Use AccountEndpoint")]
        public string EndpointUri
        {
            set => AccountEndpoint = value;
        }

        [Obsolete("Use AuthKeyOrResourceToken")]
        public string Key
        {
            set => AuthKeyOrResourceToken = value;
        }

        public TokenCredential TokenCredential
        {
            set
            {
                if (_settings.AuthKeyOrResourceToken != null || _settings.ConnectionString != null)
                    throw new ArgumentException("Another type of authentication is already being used");

                _settings.TokenCredential = value;
            }
        }

        public void ConfigureItemRequestOptions(Action<ItemRequestOptions> cfg)
        {
            _itemRequestOptions = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }

        public void ConfigureQueryRequestOptions(Action<QueryRequestOptions> cfg)
        {
            _queryRequestOptions = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }

        public void UseClientFactory(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException(nameof(clientName));

            _clientName = clientName;
        }

        public void UseNewtonsoftJson()
        {
            _registerClientFactory = RegisterNewtonsoftJsonClientFactory;
        }

        public void SetCollectionIdFormatter(Func<IServiceProvider, ICosmosCollectionIdFormatter> collectionIdFormatterFactory)
        {
            _collectionIdFormatter = collectionIdFormatterFactory ?? throw new ArgumentNullException(nameof(collectionIdFormatterFactory));
        }

        public void ConfigureLinqSerializerOptions(Action<CosmosLinqSerializerOptions> cfg)
        {
            _linqSerializerOptions = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(DatabaseId))
                yield return this.Failure("DatabaseId", "must be specified");

            if (_collectionIdFormatter == null)
                yield return this.Failure("CollectionIdFormatter", "must be specified");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            if (_clientName == null)
                _registerClientFactory(configurator);

            configurator.TryAddSingleton(DatabaseContextFactory);
            configurator.RegisterLoadSagaRepository<TSaga, CosmosSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterQuerySagaRepository<TSaga, CosmosSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                CosmosSagaRepositoryContextFactory<TSaga>>();
            configurator.AddOptions<CosmosClientOptions>();
        }

        void RegisterNewtonsoftJsonClientFactory(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            configurator.TryAddSingleton<ICosmosClientFactory>(provider =>
                new NewtonsoftJsonCosmosClientFactory(_settings, provider.GetRequiredService<IOptions<CosmosClientOptions>>()));
        }

        void RegisterSystemTextJsonClientFactory(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            configurator.TryAddSingleton<ICosmosClientFactory>(provider =>
                new SystemTextJsonCosmosClientFactory(_settings, provider.GetRequiredService<IOptions<CosmosClientOptions>>(), PropertyNamingPolicy));
        }

        DatabaseContext<TSaga> DatabaseContextFactory(IServiceProvider provider)
        {
            var clientFactory = provider.GetRequiredService<ICosmosClientFactory>();
            var client = clientFactory.GetCosmosClient<TSaga>(_clientName);

            Action<CosmosLinqSerializerOptions> serializationOptions = null;
            if (client.ClientOptions.Serializer is SystemTextJsonCosmosSerializer serializer)
            {
                if (serializer.Options.PropertyNamingPolicy == JsonNamingPolicy.CamelCase ||
                    (serializer.Options.PropertyNamingPolicy is SagaRenamePropertyNamingPolicy policy && policy.Policy == JsonNamingPolicy.CamelCase))
                {
                    serializationOptions = options =>
                    {
                        options.PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase;

                        _linqSerializerOptions?.Invoke(options);
                    };
                }
            }

            var collectionIdFormatter = _collectionIdFormatter(provider);
            var container = client.GetContainer(DatabaseId, collectionIdFormatter.Saga<TSaga>());

            return new CosmosDatabaseContext<TSaga>(container, _queryRequestOptions, _itemRequestOptions, serializationOptions);
        }
    }
}
