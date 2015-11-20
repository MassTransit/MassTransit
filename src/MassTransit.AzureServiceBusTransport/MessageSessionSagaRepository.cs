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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Newtonsoft.Json.Bson;
    using Saga;
    using Serialization;
    using Util;


    public class MessageSessionSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get(typeof(MessageSessionSagaRepository<TSaga>));

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");
            scope.Set(new
            {
                Persistence = "messageSession"
            });
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            var sessionContext = context.GetPayload<MessageSessionContext>();

            var saga = await GetSaga(sessionContext);
            if (saga == null)
            {
                var missingSagaPipe = new MissingPipe<T>(next);

                await policy.Missing(context, missingSagaPipe);
            }
            else
            {
                SagaConsumeContext<TSaga, T> sagaConsumeContext = new MessageSessionSagaConsumeContext<TSaga, T>(context, sessionContext, saga);

                await policy.Existing(sagaConsumeContext, next);

                if (!sagaConsumeContext.IsCompleted)
                {
                    using (var memoryStream = new MemoryStream())
                    using (var writer = new BsonWriter(memoryStream))
                    {
                        BsonMessageSerializer.Serializer.Serialize(writer, saga);

                        writer.Flush();
                        memoryStream.Flush();

                        using (var writeStream = new MemoryStream(memoryStream.ToArray(), false))
                        {
                            await sessionContext.SetStateAsync(writeStream);
                        }
                    }
                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("SAGA:{0}:{1} Updated {2}", TypeMetadataCache<TSaga>.ShortName, saga.CorrelationId, TypeMetadataCache<T>.ShortName);
                    }
                }
            }
        }

        public Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return Send(context, policy, next);
        }

        async Task<TSaga> GetSaga(MessageSessionContext context)
        {
            using (var readStream = await context.GetStateAsync())
            {
                if (readStream == null || readStream.Length == 0)
                    return default(TSaga);

                using (var jsonReader = new BsonReader(readStream))
                {
                    return BsonMessageSerializer.Deserializer.Deserialize<TSaga>(jsonReader);
                }
            }
        }


        /// <summary>
        /// Once the message pipe has processed the saga instance, add it to the saga repository
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        class MissingPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;

            public MissingPipe(IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                var messageSessionContext = context.GetPayload<MessageSessionContext>();

                var proxy = new MessageSessionSagaConsumeContext<TSaga, TMessage>(context, messageSessionContext, context.Saga);

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("SAGA:{0}:{1} Created {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                        TypeMetadataCache<TMessage>.ShortName);
                }

                try
                {
                    await _next.Send(proxy);

                    if (!proxy.IsCompleted)
                    {
                        using (var memoryStream = new MemoryStream())
                        using (var writer = new BsonWriter(memoryStream))
                        {
                            BsonMessageSerializer.Serializer.Serialize(writer, context.Saga);

                            writer.Flush();
                            memoryStream.Flush();

                            using (var writeStream = new MemoryStream(memoryStream.ToArray(), false))
                            {
                                await messageSessionContext.SetStateAsync(writeStream);
                            }
                        }
                        if (_log.IsDebugEnabled)
                        {
                            _log.DebugFormat("SAGA:{0}:{1} Saved {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                                TypeMetadataCache<TMessage>.ShortName);
                        }
                    }
                }
                catch (Exception)
                {
                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("SAGA:{0}:{1} Removed(Fault) {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                            TypeMetadataCache<TMessage>.ShortName);
                    }

                    throw;
                }
            }
        }
    }
}