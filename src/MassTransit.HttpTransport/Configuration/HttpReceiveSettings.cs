namespace MassTransit.HttpTransport.Configuration
{
    using System.Diagnostics;


    [DebuggerDisplay("{DebuggerDisplay}")]
    public class HttpReceiveSettings : ReceiveSettings
    {
        public int Port { get; set; }
        public string Path { get; set; }

        string DebuggerDisplay => $"{Port} {Path}";
    }
}