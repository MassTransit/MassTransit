namespace MassTransit.DocumentDbIntegration.Tests
{
    using System;
    using DocumentDbIntegration.Configuration;


    public static class Configuration
    {
        public static readonly Uri EndpointUri = EmulatorConstants.EndpointUri;

        public static readonly string Key = EmulatorConstants.Key;
    }
}
