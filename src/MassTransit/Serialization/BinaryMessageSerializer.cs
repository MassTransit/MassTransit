// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using Logging;
    using Util;

    /// <summary>
    /// The binary message serializer used the .NET BinaryFormatter to serialize
    /// message content. 
    /// </summary>
    public class BinaryMessageSerializer :
        IMessageSerializer
    {
        const string ContentTypeHeaderValue = "application/vnd.masstransit+binary";

        const string ConversationIdKey = "ConversationId";
        const string CorrelationIdKey = "CorrelationId";
        const string DestinationAddressKey = "DestinationAddress";
        const string ExpirationTimeKey = "Expiration";
        const string FaultAddressKey = "FaultAddress";
        const string MessageIdKey = "MessageId";
        const string MessageTypeKey = "MessageType";
        const string NetworkKey = "Network";
        const string RequestIdKey = "RequestId";
        const string ResponseAddressKey = "ResponseAddress";
        const string RetryCountKey = "RetryCount";
        const string SourceAddressKey = "SourceAddress";

        static readonly BinaryFormatter _formatter = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple };
        static readonly ILog _log = Logger.Get(typeof (BinaryMessageSerializer));

        public string ContentType
        {
            get { return ContentTypeHeaderValue; }
        }

        public void Serialize<T>(Stream output, ISendContext<T> context)
            where T : class
        {
            CheckConvention.EnsureSerializable(context.Message);

            _formatter.Serialize(output, context.Message, GetHeaders(context));

            context.SetContentType(ContentTypeHeaderValue);
        }

        public void Deserialize(IReceiveContext context)
        {
            object obj = _formatter.Deserialize(context.BodyStream,
                headers => DeserializeHeaderHandler(headers, context));

            context.SetContentType(ContentTypeHeaderValue);
            context.SetMessageTypeConverter(new StaticMessageTypeConverter(obj));
        }

        static Header[] GetHeaders(IMessageContext context)
        {
            var headers = new List<Header>();

            headers.Add(MessageTypeKey, context.MessageType);
            headers.Add(RequestIdKey, context.RequestId);
            headers.Add(ConversationIdKey, context.ConversationId);
            headers.Add(CorrelationIdKey, context.CorrelationId);

            headers.Add(SourceAddressKey, context.SourceAddress);
            headers.Add(DestinationAddressKey, context.DestinationAddress);
            headers.Add(ResponseAddressKey, context.ResponseAddress);
            headers.Add(FaultAddressKey, context.FaultAddress);
            headers.Add(NetworkKey, context.Network);
            headers.Add(RetryCountKey, context.RetryCount);

            if (context.ExpirationTime.HasValue)
                headers.Add(ExpirationTimeKey, context.ExpirationTime.Value);

            return headers.ToArray();
        }

        static object DeserializeHeaderHandler(Header[] headers, IReceiveContext context)
        {
            if (headers == null)
                return null;

            for (int i = 0; i < headers.Length; i++)
            {
                MapNameValuePair(context, headers[i]);
            }

            return null;
        }

        static void MapNameValuePair(IReceiveContext context, Header header)
        {
            switch (header.Name)
            {
                case SourceAddressKey:
                    context.SetSourceAddress(GetAsUri(header.Value));
                    break;

                case ResponseAddressKey:
                    context.SetResponseAddress(GetAsUri(header.Value));
                    break;

                case DestinationAddressKey:
                    context.SetDestinationAddress(GetAsUri(header.Value));
                    break;

                case FaultAddressKey:
                    context.SetFaultAddress(GetAsUri(header.Value));
                    break;

                case RequestIdKey:
                    context.SetRequestId((string) header.Value);
                    break;

                case ConversationIdKey:
                    context.SetConversationId((string) header.Value);
                    break;

                case CorrelationIdKey:
                    context.SetCorrelationId((string) header.Value);
                    break;

                case RetryCountKey:
                    context.SetRetryCount((int) header.Value);
                    break;

                case MessageTypeKey:
                    context.SetMessageType((string) header.Value);
                    break;

                case NetworkKey:
                    context.SetNetwork((string) header.Value);
                    break;

                case ExpirationTimeKey:
                    context.SetExpirationTime((DateTime) header.Value);
                    break;

                default:
                    if (header.MustUnderstand)
                    {
                        _log.WarnFormat("The header was not understood: " + header.Name);
                    }
                    break;
            }
        }

        static Uri GetAsUri(object value)
        {
            if (value == null)
                return null;

            if (value.GetType() != typeof (Uri))
                return null;

            return (Uri) value;
        }
    }
}