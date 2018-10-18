namespace MassTransit.Transports.Tests
{
    using System;


    class Credentials
    {
        public const string AmazonRegion = "us-east-2";
        public const string AmazonAccessKey = "";
        public const string AmazonSecretKey = "";

        public static readonly Uri AzureServiceUri = new Uri("sb: //masstransit-build.servicebus.windows.net");
        public const string AzureKeyName = "";
        public const string AzureKeyValue = "";
    }
}