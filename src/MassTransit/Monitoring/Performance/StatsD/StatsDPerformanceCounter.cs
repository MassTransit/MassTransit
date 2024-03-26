namespace MassTransit.Monitoring.Performance.StatsD
{
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;


    public class StatsDPerformanceCounter :
        IPerformanceCounter
    {
        readonly UdpClient _client;
        readonly string _fullName;
        readonly byte[] _incrementPayload;

        public StatsDPerformanceCounter(StatsDConfiguration cfg, string category, string name, string instance)
        {
            _fullName = $"{category}.{name}.{instance}";
            var increment = $"{_fullName}:1|c";
            _incrementPayload = Encoding.UTF8.GetBytes(increment);
            _client = new UdpClient(cfg.Hostname, cfg.Port);
        }

        public void Increment()
        {
            Task<int> t = _client.SendAsync(_incrementPayload, _incrementPayload.Length);
        }

        public void IncrementBy(long val)
        {
            var payload = $"{_fullName}:{val}|c";
            var datagram = Encoding.UTF8.GetBytes(payload);
            Task<int> t = _client.SendAsync(datagram, datagram.Length);
        }

        public void Set(long val)
        {
            var payload = $"{_fullName}:{val}|g";
            var datagram = Encoding.UTF8.GetBytes(payload);
            Task<int> t = _client.SendAsync(datagram, datagram.Length);
        }

        public void Dispose()
        {
            _client?.Close();
        }
    }
}
