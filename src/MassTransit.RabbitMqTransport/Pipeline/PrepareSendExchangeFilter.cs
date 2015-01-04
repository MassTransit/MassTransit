// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareSendExchangeFilter :
        IFilter<ModelContext>
    {
        readonly ILog _log = Logger.Get<PrepareSendExchangeFilter>();

        readonly SendSettings _settings;

        public PrepareSendExchangeFilter(SendSettings settings)
        {
            _settings = settings;
        }

        Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            if (!context.HasPayloadType(typeof(SendSettings)))
                DeclareExchange(context);

            return next.Send(context);
        }

        bool IFilter<ModelContext>.Visit(IPipeVisitor visitor)
        {
            return visitor.Visit(this);
        }

        void DeclareExchange(ModelContext context)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Exchange: {0} ({1})", _settings.ExchangeName,
                    string.Join(", ", new[]
                    {
                        _settings.Durable ? "durable" : "",
                        _settings.AutoDelete ? "auto-delete" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            if (!string.IsNullOrWhiteSpace(_settings.ExchangeName))
            {
                context.Model.ExchangeDeclare(_settings.ExchangeName, _settings.ExchangeType, _settings.Durable, _settings.AutoDelete,
                    _settings.ExchangeArguments);
            }

            context.GetOrAddPayload(() => _settings);
        }
    }

    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PreparePublishExchangeFilter :
        IFilter<ModelContext>
    {
        readonly ILog _log = Logger.Get<PreparePublishExchangeFilter>();

        readonly PublishSettings _settings;

        public PreparePublishExchangeFilter(PublishSettings settings)
        {
            _settings = settings;
        }

        Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            if (!context.HasPayloadType(typeof(PublishSettings)))
                DeclareExchange(context);

            return next.Send(context);
        }

        bool IFilter<ModelContext>.Visit(IPipeVisitor visitor)
        {
            return visitor.Visit(this);
        }

        void DeclareExchange(ModelContext context)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Exchange: {0} ({1})", _settings.ExchangeName,
                    string.Join(", ", new[]
                    {
                        _settings.Durable ? "durable" : "",
                        _settings.AutoDelete ? "auto-delete" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            if (!string.IsNullOrWhiteSpace(_settings.ExchangeName))
            {
                context.Model.ExchangeDeclare(_settings.ExchangeName, _settings.ExchangeType, _settings.Durable, _settings.AutoDelete,
                    _settings.ExchangeArguments);
            }

            context.GetOrAddPayload(() => _settings);
        }
    }
}