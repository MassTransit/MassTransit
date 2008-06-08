/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;
    using Internal;
    using Messages;

    public class Investigator : 
        Consumes<Suspect>.All,
        Consumes<Pong>.For<Guid> //, Produces<DownEndpoint>
    {
        private readonly IServiceBus _bus;
        private readonly Guid _correlationId;
        private ServiceBusRequest<Investigator> _request;
        private Suspect _suspectMessage;
        private readonly Ping _pingMessage;
        private Pong _pongMessage;
        private IEndpointResolver _resolver;

        public Investigator(IServiceBus bus, IEndpointResolver resolver)
        {
            _bus = bus;
            _resolver = resolver;
            _correlationId = Guid.NewGuid();
            _pingMessage = new Ping(this.CorrelationId);
        }


        //this starts things
        //produce<Ping>
        public void Consume(Suspect msg)
        {
            _suspectMessage = msg;

            IEndpoint ep = _resolver.Resolve(msg.EndpointUri);

            ep.Send(_pingMessage, new TimeSpan(0,3,0));
        }


        public void Consume(Pong msg)
        {
            //if we get this we are ok. but its weird that the heartbeat is down
            _pongMessage = msg;
            _request.Complete();
        }

        //Produce<DownEndpoint>
        public void OnPingTimeOut()
        {
            //I have a confirmed dead endpoint
            _bus.Publish(new DownEndpoint(_suspectMessage.EndpointUri));
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

    }
}