namespace MassTransit.EventHubIntegration.Tests
{
    static class Configuration
    {
        public static string ConsumerGroup = "cg1";

        public static string EventHubNamespace =>
            "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

        public static string StorageAccount => "UseDevelopmentStorage=true";
    }
}
