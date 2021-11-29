namespace MassTransit.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using MassTransit.Topology;
    using NUnit.Framework;
    using Observables;
    using RetryPolicies;
    using RetryPolicies.ExceptionFilters;


    [TestFixture]
    public class SendSpecification_Specs
    {
        [Test]
        public void Should_get_interfaces_in_proper_order()
        {
            IEnumerable<Type> messageTypes = GetMessageTypes<SubClass>();

            foreach (var type in messageTypes)
                Console.WriteLine(TypeCache.GetShortName(type));
        }

        [Test]
        public async Task Should_properly_delegate_the_specifications()
        {
            var specification = new SendPipeSpecification();

            specification.GetMessageSpecification<MyMessage>()
                .UseExecute(context => Console.WriteLine("Hello, World."));

            specification.GetMessageSpecification<IMyMessage>()
                .UseExecute(context =>
                {
                });

            IPipe<SendContext<MyMessage>> pipe = specification.GetMessageSpecification<MyMessage>().BuildMessagePipe();

            var sendContext = new MessageSendContext<MyMessage>(new MyMessage());

            await pipe.Send(sendContext).ConfigureAwait(false);
        }

        [Test]
        public async Task Should_properly_use_the_original_specification()
        {
            var specification = new SendPipeSpecification();

            specification.GetMessageSpecification<MyMessage>()
                .UseExecute(context => Console.WriteLine("Hello, World."));

            specification.GetMessageSpecification<IMyMessage>()
                .UseExecute(context =>
                {
                });

            var endpointSpecification = new SendPipeSpecification();
            endpointSpecification.ConnectSendPipeSpecificationObserver(new ParentSendPipeSpecificationObserver(specification));

            IPipe<SendContext<MyMessage>> pipe = endpointSpecification.GetMessageSpecification<MyMessage>().BuildMessagePipe();

            var sendContext = new MessageSendContext<MyMessage>(new MyMessage());

            await pipe.Send(sendContext).ConfigureAwait(false);
        }

        [Test]
        public async Task Should_properly_use_the_original_specification_plus_more()
        {
            var specification = new SendPipeSpecification();

            specification.GetMessageSpecification<MyMessage>()
                .UseExecute(context => Console.WriteLine("Hello, World."));

            specification.GetMessageSpecification<IMyMessage>()
                .UseExecute(context =>
                {
                });

            var endpointSpecification = new SendPipeSpecification();
            endpointSpecification.ConnectSendPipeSpecificationObserver(new ParentSendPipeSpecificationObserver(specification));

            endpointSpecification.GetMessageSpecification<IMyMessage>()
                .UseConcurrencyLimit(1);

            endpointSpecification.GetMessageSpecification<ITraceableMessage>()
                .UsePartitioner(8, x => NewId.NextGuid());

            IPipe<SendContext<MyMessage>> pipe = endpointSpecification.GetMessageSpecification<MyMessage>().BuildMessagePipe();

            var sendContext = new MessageSendContext<MyMessage>(new MyMessage());

            await pipe.Send(sendContext).ConfigureAwait(false);
        }

        [Test]
        public async Task Should_properly_use_the_original_specification_plus_topology()
        {
            var sendTopology = new SendTopology();

            sendTopology.GetMessageTopology<MyMessage>()
                .Add(new TestMessageSendTopology<MyMessage>());

            var specification = new SendPipeSpecification();
            specification.ConnectSendPipeSpecificationObserver(new TopologySendPipeSpecificationObserver(sendTopology));

            specification.GetMessageSpecification<MyMessage>()
                .UseExecute(context => Console.WriteLine("Hello, World."));

            specification.GetMessageSpecification<IMyMessage>()
                .UseExecute(context =>
                {
                });

            var endpointSpecification = new SendPipeSpecification();
            endpointSpecification.ConnectSendPipeSpecificationObserver(new ParentSendPipeSpecificationObserver(specification));

            endpointSpecification.GetMessageSpecification<IMyMessage>()
                .UseConcurrencyLimit(1);

            endpointSpecification.GetMessageSpecification<ITraceableMessage>()
                .UsePartitioner(8, x => NewId.NextGuid());

            IPipe<SendContext<MyMessage>> pipe = endpointSpecification.GetMessageSpecification<MyMessage>().BuildMessagePipe();

            var sendContext = new MessageSendContext<MyMessage>(new MyMessage());

            await pipe.Send(sendContext).ConfigureAwait(false);
        }

        [Test]
        public async Task Should_properly_use_the_original_specification_plus_topology_on_type()
        {
            var sendTopology = new SendTopology();
            sendTopology.GetMessageTopology<IMyMessage>()
                .Add(new TestMessageSendTopology<IMyMessage>());

            var specification = new SendPipeSpecification();
            specification.ConnectSendPipeSpecificationObserver(new TopologySendPipeSpecificationObserver(sendTopology));

            specification.GetMessageSpecification<MyMessage>()
                .UseExecute(context => Console.WriteLine("Hello, World."));

            specification.GetMessageSpecification<IMyMessage>()
                .UseExecute(context =>
                {
                });

            var endpointSpecification = new SendPipeSpecification();
            endpointSpecification.ConnectSendPipeSpecificationObserver(new ParentSendPipeSpecificationObserver(specification));

            endpointSpecification.GetMessageSpecification<IMyMessage>()
                .UseConcurrencyLimit(1);

            endpointSpecification.GetMessageSpecification<ITraceableMessage>()
                .UsePartitioner(8, x => NewId.NextGuid());

            IPipe<SendContext<MyMessage>> pipe = endpointSpecification.GetMessageSpecification<MyMessage>().BuildMessagePipe();

            var sendContext = new MessageSendContext<MyMessage>(new MyMessage());

            await pipe.Send(sendContext).ConfigureAwait(false);
        }

        static IEnumerable<Type> GetMessageTypes<TMessage>()
        {
            if (MessageTypeCache<TMessage>.IsValidMessageType)
                yield return typeof(TMessage);

            foreach (var baseInterface in GetImplementedInterfaces(typeof(TMessage)))
                yield return baseInterface;

            var baseType = typeof(TMessage).GetTypeInfo().BaseType;
            while (baseType != null && MessageTypeCache.IsValidMessageType(baseType))
            {
                yield return baseType;

                foreach (var baseInterface in GetImplementedInterfaces(baseType))
                    yield return baseInterface;

                baseType = baseType.GetTypeInfo().BaseType;
            }
        }

        static IEnumerable<Type> GetImplementedInterfaces(Type baseType)
        {
            IEnumerable<Type> baseInterfaces = baseType
                .GetInterfaces()
                .Where(MessageTypeCache.IsValidMessageType)
                .ToArray();

            if (baseType.GetTypeInfo().BaseType != null && baseType.GetTypeInfo().BaseType != typeof(object))
            {
                baseInterfaces = baseInterfaces
                    .Except(baseType.GetTypeInfo().BaseType.GetInterfaces())
                    .Except(baseInterfaces.SelectMany(x => x.GetInterfaces()))
                    .ToArray();
            }

            return baseInterfaces;
        }


        public interface IZeLastInterface
        {
        }


        public interface ISuperClass :
            ISuperBase
        {
        }


        public interface ISuperBase
        {
        }


        public class SuperClass :
            ISuperClass,
            IZeLastInterface
        {
        }


        public interface ISubClass
        {
        }


        public class SubClass :
            SuperClass,
            ISubClass
        {
        }


        public class MyMessage :
            IMyMessage,
            ITraceableMessage
        {
        }


        public interface IMyMessage
        {
        }


        public interface ITraceableMessage
        {
        }


        class TestMessageSendTopology<T> :
            IMessageSendTopology<T>
            where T : class
        {
            public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
            {
                builder.AddFilter(new RetryFilter<SendContext<T>>(new ImmediateRetryPolicy(new AllExceptionFilter(), 5), new RetryObservable()));
            }
        }
    }
}
