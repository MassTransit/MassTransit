namespace MassTransit.Monitoring.Performance.StatsD
{
    public class StatsDConfiguration
    {
        public static StatsDConfiguration Defaults()
        {
            return new StatsDConfiguration(8125);
        }

        public StatsDConfiguration(int port)
        {
            Port = port;
        }

        public int Port { get; set; }
    }
}