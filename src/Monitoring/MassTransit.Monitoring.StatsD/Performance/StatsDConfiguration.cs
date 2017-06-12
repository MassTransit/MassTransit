namespace MassTransit.Monitoring.Performance.StatsD
{
    public class StatsDConfiguration
    {
        public static StatsDConfiguration Defaults()
        {
            return new StatsDConfiguration("localhost", 8125);
        }

        public StatsDConfiguration(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }

        public string Hostname { get; set; }
        public int Port { get; set; }
    }
}