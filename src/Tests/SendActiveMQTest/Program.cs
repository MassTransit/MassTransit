using MassTransit;
using MassTransit.ActiveMqTransport;
using MassTransit.ActiveMqTransport.Transport;
using MassTransit.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
            EndpointConvention.Map<AhmedProfile>(new Uri($"queue:AhmedQueue"));
            var profile = new LoadProfile
            {
                Message = "Hello"
            };
            var ahmedProfile = new AhmedProfile { Message = "Ahmed" };
            await _bus.Send(profile);
            await _bus.SendAsTextMessage(profile);
            await _bus.SendAsBinaryMessage(ahmedProfile);
        }
    }

    public class LoadProfile
    {
        public string Message { get; set; }
    }

    public class AhmedProfile
    {
        public string Message { get; set; }
    }
}
