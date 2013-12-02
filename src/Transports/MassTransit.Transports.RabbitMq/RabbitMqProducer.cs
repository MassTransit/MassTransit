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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
#if NET40
    using System.Threading.Tasks;
#endif
    using Magnum.Caching;
    using Magnum.Extensions;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    public class RabbitMqProducer :
        ConnectionBinding<RabbitMqConnection>
    {
#if NET40
        readonly Cache<ulong, TaskCompletionSource<bool>> _confirms;
#endif
        static readonly ILog _log = Logger.Get<RabbitMqProducer>();
        readonly IRabbitMqEndpointAddress _address;
        readonly bool _bindToQueue;
        readonly object _lock = new object();
        IModel _channel;
        bool _immediate;
        bool _mandatory;

        public RabbitMqProducer(IRabbitMqEndpointAddress address, bool bindToQueue)
        {
            _address = address;
            _bindToQueue = bindToQueue;
#if NET40
            _confirms = new ConcurrentCache<ulong, TaskCompletionSource<bool>>();
#endif
        }

        public void Bind(RabbitMqConnection connection)
        {
            lock (_lock)
            {
                IModel channel = null;
                try
                {
                    channel = connection.Connection.CreateModel();

                    DeclareAndBindQueue(connection, channel);

                    BindEvents(channel);

                    _channel = channel;
                }
                catch (Exception ex)
                {
                    channel.Cleanup(500, ex.Message);

                    throw new InvalidConnectionException(_address.Uri, "Invalid connection to host", ex);
                }
            }
        }

        void DeclareAndBindQueue(RabbitMqConnection connection, IModel channel)
        {
            if (_bindToQueue)
            {
                connection.DeclareExchange(channel, _address.Name, _address.Durable, _address.AutoDelete);

                connection.BindQueue(channel, _address.Name, _address.Durable, _address.Exclusive, _address.AutoDelete,
                    _address.QueueArguments());
            }
        }

        void BindEvents(IModel channel)
        {
            channel.BasicAcks += HandleAck;
            channel.BasicNacks += HandleNack;
            channel.BasicReturn += HandleReturn;
            channel.FlowControl += HandleFlowControl;
            channel.ModelShutdown += HandleModelShutdown;
            channel.ConfirmSelect();
        }

        public void Unbind(RabbitMqConnection connection)
        {
            lock (_lock)
            {
                try
                {
                    if (_channel != null)
                    {
#if NET40
                        WaitForPendingConfirms();
#endif

                        UnbindEvents(_channel);
                        _channel.Cleanup(200, "Producer Unbind");
                    }
                }
                finally
                {
                    if (_channel != null)
                        _channel.Dispose();
                    _channel = null;

                    FailPendingConfirms();
                }
            }
        }

        void WaitForPendingConfirms()
        {
            try
            {
                bool timedOut;
                _channel.WaitForConfirms(60.Seconds(), out timedOut);
                if (timedOut)
                    _log.WarnFormat("Timeout waiting for all pending confirms on {0}", _address.Uri);
            }
            catch (Exception ex)
            {
                _log.Error("Waiting for pending confirms threw an exception", ex);
            }
        }

        void UnbindEvents(IModel channel)
        {
            channel.BasicAcks -= HandleAck;
            channel.BasicNacks -= HandleNack;
            channel.BasicReturn -= HandleReturn;
            channel.FlowControl -= HandleFlowControl;
            channel.ModelShutdown -= HandleModelShutdown;
        }

        void FailPendingConfirms()
        {
#if NET40
            try
            {
                var exception = new MessageNotConfirmedException(_address.Uri,
                    "Publish not confirmed before channel closed");

                _confirms.Each((id, task) => task.TrySetException(exception));
            }
            catch (Exception ex)
            {
                _log.Error("Exception while failing pending confirms", ex);
            }

            _confirms.Clear();
#endif
        }

        public IBasicProperties CreateProperties()
        {
            lock (_lock)
            {
                if (_channel == null)
                    throw new InvalidConnectionException(_address.Uri, "Channel should not be null");

                return _channel.CreateBasicProperties();
            }
        }

        public void Publish(string exchangeName, IBasicProperties properties, byte[] body)
        {
            lock (_lock)
            {
                if (_channel == null)
                    throw new InvalidConnectionException(_address.Uri, "No connection to RabbitMQ Host");

                _channel.BasicPublish(exchangeName, "", properties, body);
            }
        }

#if NET40
        public Task PublishAsync(string exchangeName, IBasicProperties properties, byte[] body)
        {
            lock (_lock)
            {
                if (_channel == null)
                    throw new InvalidConnectionException(_address.Uri, "No connection to RabbitMQ Host");

                ulong deliveryTag = _channel.NextPublishSeqNo;

                var task = new TaskCompletionSource<bool>();
                _confirms.Add(deliveryTag, task);

                try
                {
                    _channel.BasicPublish(exchangeName, "", _mandatory, _immediate, properties, body);
                }
                catch
                {
                    _confirms.Remove(deliveryTag);                    
                    throw;
                }

                return task.Task;
            }
        }
#endif

        void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            try
            {
                FailPendingConfirms();
            }
            catch (Exception ex)
            {
                _log.Error("Fail pending confirms failed during model shutdown", ex);
            }
        }

        void HandleFlowControl(IModel sender, FlowControlEventArgs args)
        {
        }

        void HandleReturn(IModel model, BasicReturnEventArgs args)
        {
        }

        void HandleNack(IModel model, BasicNackEventArgs args)
        {
#if NET40
            IEnumerable<ulong> ids = Enumerable.Repeat(args.DeliveryTag, 1);
            if (args.Multiple)
                ids = _confirms.GetAllKeys().Where(x => x <= args.DeliveryTag);

            var exception = new InvalidOperationException("Publish was nacked by the broker");

            foreach (ulong id in ids)
            {
                _confirms[id].TrySetException(exception);
                _confirms.Remove(id);
            }
#endif
        }

        void HandleAck(IModel model, BasicAckEventArgs args)
        {
#if NET40
            IEnumerable<ulong> ids = Enumerable.Repeat(args.DeliveryTag, 1);
            if (args.Multiple)
                ids = _confirms.GetAllKeys().Where(x => x <= args.DeliveryTag);

            foreach (ulong id in ids)
            {
                _confirms[id].TrySetResult(true);
                _confirms.Remove(id);
            }
#endif
        }
    }
}