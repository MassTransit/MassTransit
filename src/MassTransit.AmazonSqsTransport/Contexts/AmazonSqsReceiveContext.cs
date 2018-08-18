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
namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Amazon.SQS.Model;
    using Context;
    using Exceptions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Util;


    public sealed class AmazonSqsReceiveContext :
        BaseReceiveContext,
        AmazonSqsMessageContext
    {
        readonly AmazonSqsReceiveEndpointContext _context;
        readonly Message _transportMessage;
        byte[] _body;

        public AmazonSqsReceiveContext(Uri inputAddress, Message transportMessage, bool redelivered, AmazonSqsReceiveEndpointContext context)
            : base(inputAddress, redelivered, context)
        {
            _transportMessage = transportMessage;
            _context = context;
        }

        protected override IHeaderProvider HeaderProvider => new AmazonSqsHeaderProvider(_transportMessage);

        public Message TransportMessage => _transportMessage;

        public Dictionary<string, MessageAttributeValue> Attributes => _transportMessage.MessageAttributes;

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
            if (_body != null)
                return _body;

            if (_transportMessage != null)
            {
                var envelope = JsonConvert.DeserializeObject<JToken>(TransportMessage.Body);

                var envelopeType = envelope["Type"].ToString();
                if (envelopeType == "Notification")
                    return _body = Encoding.UTF8.GetBytes(envelope["Message"].ToString());

                return Encoding.UTF8.GetBytes(TransportMessage.Body);
            }

            throw new AmazonSqsTransportException($"The message type is not supported: {TypeMetadataCache.GetShortName(_transportMessage.GetType())}");
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(GetBody());
        }
    }
}