namespace MassTransit.HttpTransport.Configuration
{
    public class HttpReceiveSettings : ReceiveSettings
    {
        public int Port { get; set; }
        public string Path { get; set; }
    }
}