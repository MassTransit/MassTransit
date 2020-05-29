namespace MassTransit.Monitoring.Performance.StatsD
{
    public class StatsDConfiguration
    {
        public StatsDConfiguration(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }

        public string Hostname { get; set; }
        public int Port { get; set; }

        public static StatsDConfiguration Defaults()
        {
            return new StatsDConfiguration("localhost", 8125);
        }
    }
}
