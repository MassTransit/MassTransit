// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Integration;
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    public class RabbitMqModelContext :
        ModelContext,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<RabbitMqModelContext>();

        readonly ConnectionContext _connectionContext;
        readonly IModel _model;
        readonly PayloadCache _payloadCache;
        readonly object _publishLock = new object();
        readonly ConcurrentDictionary<ulong, PendingPublish> _published;
        readonly CancellationTokenSource _tokenSource;
        CancellationTokenRegistration _registration;

        public RabbitMqModelContext(ConnectionContext connectionContext, IModel model, CancellationToken cancellationToken)
        {
            _connectionContext = connectionContext;
            _model = model;

            _payloadCache = new PayloadCache();
            _published = new ConcurrentDictionary<ulong, PendingPublish>();


            _tokenSource = new CancellationTokenSource();
            _registration = cancellationToken.Register(OnCancellationRequested);

            model.ModelShutdown += OnModelShutdown;
            model.BasicAcks += OnBasicAcks;
            model.BasicNacks += OnBasicNacks;
            model.BasicReturn += OnBasicReturn;
            model.ConfirmSelect();
        }

        public void Dispose()
        {
            _model.ModelShutdown -= OnModelShutdown;

            _registration.Dispose();

            _model.Cleanup(200, "ModelContext Disposed");
        }

        public bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            return _payloadCache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        public IModel Model
        {
            get { return _model; }
        }

        public ConnectionContext ConnectionContext
        {
            get { return _connectionContext; }
        }

        public CancellationToken CancellationToken
        {
            get { return _tokenSource.Token; }
        }

        public Task BasicPublishAsync(string exchange, string routingKey, bool mandatory, bool immediate, IBasicProperties basicProperties,
            byte[] body)
        {
            PendingPublish pendingPublish;
            lock (_publishLock)
            {
                ulong publishTag = _model.NextPublishSeqNo;
                pendingPublish = new PendingPublish(_connectionContext, exchange, publishTag);
                try
                {
                    _published.AddOrUpdate(publishTag, key => pendingPublish, (key, existing) =>
                    {
                        existing.PublishNotConfirmed();
                        return pendingPublish;
                    });

                    _model.BasicPublish(exchange, routingKey, mandatory, immediate, basicProperties, body);
                }
                catch
                {
                    PendingPublish ignored;
                    _published.TryRemove(publishTag, out ignored);

                    throw;
                }
            }

            return pendingPublish.Task;
        }

        void OnBasicReturn(object model, BasicReturnEventArgs args)
        {
        }

        void OnModelShutdown(object model, ShutdownEventArgs reason)
        {
            _tokenSource.Cancel();

            _model.WaitForConfirms(TimeSpan.FromSeconds(30));

            _model.ModelShutdown -= OnModelShutdown;
            _model.BasicAcks -= OnBasicAcks;
            _model.BasicNacks -= OnBasicNacks;
            _model.BasicReturn -= OnBasicReturn;

            FaultPendingPublishes();
        }

        void FaultPendingPublishes()
        {
            try
            {
                foreach (ulong key in _published.Keys)
                {
                    PendingPublish pending;
                    if (_published.TryRemove(key, out pending))
                        pending.PublishNotConfirmed();
                }
            }
            catch (Exception)
            {
            }
        }

        void OnBasicNacks(object model, BasicNackEventArgs args)
        {
            if (args.Multiple)
            {
                ulong[] ids = _published.Keys.Where(x => x <= args.DeliveryTag).ToArray();
                foreach (ulong id in ids)
                {
                    PendingPublish value;
                    if (_published.TryRemove(id, out value))
                        value.Nack();
                }
            }
            else
            {
                PendingPublish value;
                if (_published.TryRemove(args.DeliveryTag, out value))
                    value.Nack();
            }
        }

        void OnBasicAcks(object model, BasicAckEventArgs args)
        {
            if (args.Multiple)
            {
                ulong[] ids = _published.Keys.Where(x => x <= args.DeliveryTag).ToArray();
                foreach (ulong id in ids)
                {
                    PendingPublish value;
                    if (_published.TryRemove(id, out value))
                        value.Ack();
                }
            }
            else
            {
                PendingPublish value;
                if (_published.TryRemove(args.DeliveryTag, out value))
                    value.Ack();
            }
        }

        void OnCancellationRequested()
        {
            _tokenSource.Cancel();
        }
    }
}