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
    using Metadata;
    using Util;


    public sealed class AmazonSqsReceiveContext :
        BaseReceiveContext,
        AmazonSqsMessageContext
    {
        byte[] _body;

        public AmazonSqsReceiveContext(Uri inputAddress, Message transportMessage, bool redelivered, SqsReceiveEndpointContext context)
            : base(inputAddress, redelivered, context)
        {
            TransportMessage = transportMessage;
        }

        protected override IHeaderProvider HeaderProvider => new AmazonSqsHeaderProvider(TransportMessage);

        public Message TransportMessage { get; }

        public Dictionary<string, MessageAttributeValue> Attributes => TransportMessage.MessageAttributes;

        public override byte[] GetBody()
        {
            if (_body != null)
                return _body;

            if (TransportMessage != null)
            {
                return _body = Encoding.UTF8.GetBytes(TransportMessage.Body);
            }

            throw new AmazonSqsTransportException($"The message type is not supported: {TypeMetadataCache.GetShortName(typeof(Message))}");
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(GetBody());
        }
    }
}