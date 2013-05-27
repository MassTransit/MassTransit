// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports
{
    using Magnum;
    using Serialization;
    using Util;

    public class EndpointSettings :
        TransportSettings,
        IEndpointSettings
    {
        public EndpointSettings(IEndpointAddress address)
            : base(address)
        {
            ErrorAddress = GetErrorEndpointAddress();
        }

        public EndpointSettings(IEndpointAddress address, IEndpointSettings source)
            : base(address, source)
        {
            Guard.AgainstNull(source, "source");

            Serializer = source.Serializer;
            SupportedSerializers = source.SupportedSerializers;
            if (source.ErrorAddress != address)
                ErrorAddress = source.ErrorAddress;
            RetryLimit = source.RetryLimit;
            TrackerFactory = source.TrackerFactory;
        }

        public EndpointSettings(IEndpointAddress address, IMessageSerializer serializer, ITransportSettings source)
            : base(address, source)
        {
            Guard.AgainstNull(source, "source");

            Serializer = serializer;

            var messageSerializers = new SupportedMessageSerializers();
            messageSerializers.AddSerializer(serializer);
            SupportedSerializers = messageSerializers;

            ErrorAddress = GetErrorEndpointAddress();
        }

        public IEndpointAddress ErrorAddress { get; set; }
        public IMessageSerializer Serializer { get; set; }
        public ISupportedMessageSerializers SupportedSerializers { get; set; }
        public int RetryLimit { get; set; }
        public MessageTrackerFactory TrackerFactory { get; set; }

        EndpointAddress GetErrorEndpointAddress()
        {
            return new EndpointAddress(Address.Uri.AppendToPath("_error"));
        }
    }
}