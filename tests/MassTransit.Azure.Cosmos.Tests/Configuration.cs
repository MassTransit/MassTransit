namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using Azure.Cosmos.Configuration;


    public static class Configuration
    {
        public static readonly string EndpointUri = EmulatorConstants.EndpointUri;

        public static readonly string Key = EmulatorConstants.Key;
    }
}
