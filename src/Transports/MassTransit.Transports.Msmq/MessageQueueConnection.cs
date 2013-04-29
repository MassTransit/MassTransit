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
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Messaging;
    using Exceptions;
    using Logging;

    public class MessageQueueConnection :
        Connection
    {
        static readonly ILog _log = Logger.Get(typeof (MessageQueueConnection));
        
        readonly QueueAccessMode _accessMode;
        readonly IMsmqEndpointAddress _address;
        readonly string _formatName;
        readonly string _multicastAddress;
        bool _disposed;
        MessageQueue _queue;

        public MessageQueueConnection(IMsmqEndpointAddress address, QueueAccessMode accessMode)
        {
            _address = address;
            _accessMode = accessMode;
            _multicastAddress = null;

            switch (_accessMode)
            {
                case QueueAccessMode.Send:
                    _formatName = _address.OutboundFormatName;
                    break;

                case QueueAccessMode.Peek:
                case QueueAccessMode.PeekAndAdmin:
                case QueueAccessMode.Receive:
                case QueueAccessMode.ReceiveAndAdmin:
                    _formatName = _address.InboundFormatName;
                    _multicastAddress = address.MulticastAddress;
                    break;

                default:
                    throw new EndpointException(_address.Uri, "An endpoint connection cannot be send and receive");
            }
        }

        public IMsmqEndpointAddress Address
        {
            get { return _address; }
        }

        public string FormatName
        {
            get { return _formatName; }
        }

        public MessageQueue Queue
        {
            get { return _queue; }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Disconnect()
        {
            _log.DebugFormat("Closing: " + _formatName);

            try
            {
                if (_queue != null)
                {
                    try
                    {
                        _queue.Close();
                    }
                    catch (Exception ex)
                    {
                        _log.Warn("Exception while closing MessageQueue connection", ex);
                    }

                    _queue.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log.Warn("Exception disposing of MessageQueue connection", ex);
            }
            finally
            {
                _queue = null;
            }
        }

        public void Connect()
        {
            Disconnect();

            _log.DebugFormat("Creating MessageQueue: " + _formatName);

            _queue = new MessageQueue(_formatName, _accessMode);
            if (_multicastAddress != null)
            {
                _queue.MulticastAddress = _multicastAddress;
            }
            
            var filter = new MessagePropertyFilter();
            filter.SetAll();
            _queue.MessageReadPropertyFilter = filter;
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                Disconnect();
            }

            _disposed = true;
        }
    }
}