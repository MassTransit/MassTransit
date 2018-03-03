// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.IO;
    using System.Text;
    using Apache.NMS;
    using Context;
    using Topology;
    using Util;


    public sealed class ActiveMqReceiveContext :
        BaseReceiveContext,
        ActiveMqMessageContext
    {
        readonly ActiveMqReceiveEndpointContext _context;
        readonly IMessage _transportMessage;

        public ActiveMqReceiveContext(Uri inputAddress, IMessage transportMessage, IReceiveObserver observer, ActiveMqReceiveEndpointContext context)
            : base(inputAddress, transportMessage.NMSRedelivered, observer, context)
        {
            _transportMessage = transportMessage;
            _context = context;
        }

        protected override IHeaderProvider HeaderProvider => new ActiveMqHeaderProvider(_transportMessage.Properties);

        public string Destination => _transportMessage.NMSDestination.ToString();

        public IMessage TransportMessage => _transportMessage;

        public IPrimitiveMap Properties => _transportMessage.Properties;

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _context.CreateSendEndpointProvider(this);
        }

        protected override IPublishEndpointProvider GetPublishEndpointProvider()
        {
            return _context.CreatePublishEndpointProvider(this);
        }

        public override byte[] GetBody()
        {
            if (_transportMessage is ITextMessage textMessage)
                return Encoding.UTF8.GetBytes(textMessage.Text);

            if (_transportMessage is IBytesMessage bytesMessage)
                return bytesMessage.Content;

            throw new ActiveMqTransportException($"The message type is not supported: {TypeMetadataCache.GetShortName(_transportMessage.GetType())}");
        }

        public override Stream GetBodyStream()
        {
            if (_transportMessage is ITextMessage textMessage)
                return new MemoryStream(Encoding.UTF8.GetBytes(textMessage.Text));

            if (_transportMessage is IBytesMessage bytesMessage)
                return new MemoryStream(bytesMessage.Content);

            throw new ActiveMqTransportException($"The message type is not supported: {TypeMetadataCache.GetShortName(_transportMessage.GetType())}");
        }
    }
}