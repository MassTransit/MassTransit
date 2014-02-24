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
namespace MassTransit.Pipeline.Inspectors
{
    using System;
    using System.Text;
    using Distributor.Pipeline;
    using Magnum.Extensions;
    using Saga;
    using Saga.Pipeline;
    using Sinks;
    using Util;


    public class PipelineViewer :
        PipelineInspectorBase<PipelineViewer>
    {
        readonly StringBuilder _text = new StringBuilder();
        int _depth;

        public string Text
        {
            get { return _text.ToString(); }
        }

        public bool Inspect(InboundMessagePipeline pipeline)
        {
            Append("Pipeline");

            return true;
        }

        public bool Inspect(OutboundMessagePipeline pipeline)
        {
            Append("Pipeline");

            return true;
        }

        public bool Inspect<TMessage>(MessageRouter<TMessage> router)
            where TMessage : class
        {
            Append(string.Format("Routed ({0})", GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TMessage>(OutboundMessageFilter<TMessage> element) where TMessage : class
        {
            Append(string.Format("Filtered '{0}'", GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect(InboundMessageInterceptor element)
        {
            Append(string.Format("Interceptor"));

            return true;
        }

        public bool Inspect(OutboundMessageInterceptor element)
        {
            Append(string.Format("Interceptor"));

            return true;
        }

        public bool Inspect<TMessage>(InstanceMessageSink<TMessage> sink)
            where TMessage : class
        {
            Append(string.Format("Consumed by Instance ({0})", GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TMessage>(DistributorMessageSink<TMessage> sink)
            where TMessage : class
        {
            Append(string.Format("Distributor ({0})", GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TMessage>(WorkerMessageSink<TMessage> sink)
            where TMessage : class
        {
            Append(string.Format("Distributor Worker ({0})", typeof(TMessage).ToFriendlyName()));

            return true;
        }

        public bool Inspect<TMessage>(EndpointMessageSink<TMessage> sink)
            where TMessage : class
        {
            Append(string.Format("Send {0} to Endpoint {1}", GetMessageName<TMessage>(), sink.Endpoint.Address.Uri));

            return true;
        }

        public bool Inspect<TMessage>(IPipelineSink<TMessage> sink)
            where TMessage : class
        {
            Append(string.Format("Unknown Message Sink {0} ({1})", sink.GetType().ToFriendlyName(),
                typeof(TMessage).ToFriendlyName()));

            return true;
        }

        public bool Inspect<T, TMessage, TKey>(CorrelatedMessageRouter<T, TMessage, TKey> sink)
            where TMessage : class, CorrelatedBy<TKey>
            where T : class, IMessageContext<TMessage>
        {
            Append(string.Format("Correlated by {1} ({0})", GetMessageName<TMessage>(), typeof(TKey).ToFriendlyName()));

            return true;
        }

        public bool Inspect<T, TMessage, TKey>(CorrelatedMessageSinkRouter<T, TMessage, TKey> sink)
            where T : class
            where TMessage : class, CorrelatedBy<TKey>
        {
            Append(string.Format("Routed for Correlation Id {1} ({0})", GetMessageName<TMessage>(), sink.CorrelationId));

            return true;
        }

        public bool Inspect<T, TMessage>(RequestMessageRouter<T, TMessage> router)
            where T : class, IConsumeContext<TMessage>
            where TMessage : class
        {
            Append(string.Format("Routed Request {0}", GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TMessage>(InboundConvertMessageSink<TMessage> converter)
            where TMessage : class
        {
            Append(string.Format("Translated to {0}", GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TMessage>(OutboundConvertMessageSink<TMessage> converter)
            where TMessage : class
        {
            Append(string.Format("Translated to {0}", GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TComponent, TMessage>(ConsumerMessageSink<TComponent, TMessage> sink)
            where TMessage : class
            where TComponent : class, Consumes<TMessage>.All
        {
            Append(string.Format("Consumed by Component {0} ({1})", GetComponentName<TComponent>(),
                GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TComponent, TMessage>(ContextConsumerMessageSink<TComponent, TMessage> sink)
            where TMessage : class
            where TComponent : class, IConsumer<TMessage>
        {
            Append(string.Format("Consumed by Component {0} ({1} w/Context)", GetComponentName<TComponent>(),
                GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TComponent, TMessage>(CorrelatedSagaMessageSink<TComponent, TMessage> sink)
            where TMessage : class, CorrelatedBy<Guid>
            where TComponent : class, Consumes<TMessage>.All, ISaga
        {
            string policyDescription = GetPolicy(sink.Policy);

            Append(string.Format("{0} Saga {1} ({2})", policyDescription, GetComponentName<TComponent>(),
                GetMessageName<TMessage>()));

            return true;
        }

        public bool Inspect<TComponent, TMessage>(PropertySagaMessageSink<TComponent, TMessage> sink)
            where TMessage : class
            where TComponent : class, Consumes<TMessage>.All, ISaga
        {
            string policyDescription = GetPolicy(sink.Policy);
            string expression = sink.Selector.ToString();

            Append(string.Format("{0} Saga {1} ({2}): {3}", policyDescription, GetComponentName<TComponent>(),
                GetMessageName<TMessage>(), expression));

            return true;
        }

        public bool Inspect<TComponent, TMessage>(PropertySagaStateMachineMessageSink<TComponent, TMessage> sink)
            where TMessage : class
            where TComponent : SagaStateMachine<TComponent>, ISaga
        {
            string policyDescription = GetPolicy(sink.Policy);
            string expression = sink.Selector.ToString();

            Append(string.Format("{0} Saga {1} ({2}): {3}", policyDescription, GetComponentName<TComponent>(),
                GetMessageName<TMessage>(), expression));

            return true;
        }

        public bool Inspect<TComponent, TMessage>(CorrelatedSagaStateMachineMessageSink<TComponent, TMessage> sink)
            where TMessage : class, CorrelatedBy<Guid>
            where TComponent : SagaStateMachine<TComponent>, ISaga
        {
            string policyDescription = GetPolicy(sink.Policy);

            Append(string.Format("{0} SagaStateMachine {1} ({2})", policyDescription, GetComponentName<TComponent>(),
                GetMessageName<TMessage>()));

            return true;
        }

        static string GetMessageName<TMessage>() where TMessage : class
        {
            Type messageType = typeof(TMessage);
            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition().Implements<IMessageContext>())
                messageType = messageType.GetGenericArguments()[0];

            return messageType.ToMessageName();
        }

        static string GetComponentName<TComponent>()
        {
            Type componentType = typeof(TComponent);

            string componentName = componentType.IsGenericType
                ? componentType.GetGenericTypeDefinition().ToFriendlyName()
                : componentType.ToFriendlyName();
            return componentName;
        }

        protected override void IncreaseDepth()
        {
            _depth++;
        }

        protected override void DecreaseDepth()
        {
            _depth--;
        }

        void Pad()
        {
            _text.Append(new string('\t', _depth));
        }

        void Append(string text)
        {
            Pad();

            _text.AppendFormat(text).AppendLine();
        }

        public static void Trace<T>(IPipelineSink<T> pipeline)
            where T : class
        {
            var viewer = new PipelineViewer();

            pipeline.Inspect(viewer);

            System.Diagnostics.Trace.WriteLine(viewer.Text);
        }

        public static void Trace<T>(IPipelineSink<T> pipeline, Action<string> callback)
            where T : class
        {
            var viewer = new PipelineViewer();

            pipeline.Inspect(viewer);

            callback(viewer.Text);
        }

        static string GetPolicy<TComponent, TMessage>(ISagaPolicy<TComponent, TMessage> policy)
            where TComponent : class, ISaga
            where TMessage : class
        {
            string description;
            Type policyType = policy.GetType().GetGenericTypeDefinition();
            if (policyType == typeof(InitiatingSagaPolicy<,>))
                description = "Initiates New";
            else if (policyType == typeof(ExistingOrIgnoreSagaPolicy<,>))
                description = "Orchestrates Existing";
            else if (policyType == typeof(CreateOrUseExistingSagaPolicy<,>))
                description = "Initiates New Or Orchestrates Existing";
            else
                description = policyType.ToFriendlyName();
            return description;
        }
    }
}