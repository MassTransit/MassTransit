namespace MassTransitBenchmark
{
    using System;
    using NDesk.Options;


    public class GrpcOptionSet :
        OptionSet
    {
        public GrpcOptionSet()
        {
            Add<string>("h|host:", "The host name of the broker", value => Host = value);
            Add<int>("port:", "The virtual host to use", value => Port = value);
            Add<bool>("split:", "Split into two bus instances to leverage separate connections", x => Split = x);
            Add<bool>("lb:", "Load balance across both bus instances (only valid with split)", x => LoadBalance = x);

            Host = "127.0.0.1";
            Port = 19796;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public bool Split { get; set; }
        public bool LoadBalance { get; set; }

        public Uri HostAddress =>
            new UriBuilder
            {
                Scheme = "http",
                Host = Host,
                Port = Port
            }.Uri;

        public Uri SecondHostAddress =>
            new UriBuilder
            {
                Scheme = "http",
                Host = Host,
                Port = Port + 10000
            }.Uri;

        public void ShowOptions()
        {
            Console.WriteLine("Host: {0}", HostAddress);

            if (Split)
                Console.WriteLine("Second Host: {0}", SecondHostAddress);
        }
    }
}