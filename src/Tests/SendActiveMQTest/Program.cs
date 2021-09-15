using MassTransit;
using MassTransit.ActiveMqTransport;
using MassTransit.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SendActiveMQTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var _bus = Bus.Factory.CreateUsingActiveMq(config =>
            {
                config.Host("localhost", 61616, h =>
                {
                    h.Username("admin");
                    h.Password("admin");
                });
                config.ClearMessageDeserializers();
                config.UseRawXmlSerialzier(RawXmlSerializerOptions.Default);

            });
            EndpointConvention.Map<LoadProfile>(new Uri($"queue:TestQueue"));
         
            await _bus.Send(new LoadProfile { Message ="Hello"}, new Dictionary<string, string> { ["Type"] = "Text" });
        }
    }

    public class LoadProfile{
        public string Message { get; set; }
    }
}
