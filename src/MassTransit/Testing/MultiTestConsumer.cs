namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using MessageObservers;
    using Pipeline;
    using Util;


    public class MultiTestConsumer
    {
        readonly IList<IConsumerConfigurator> _configures;
        readonly ReceivedMessageList _received;

        public MultiTestConsumer(TimeSpan timeout)
        {
            Timeout = timeout;
            _configures = new List<IConsumerConfigurator>();

            _received = new ReceivedMessageList(timeout);
        }

        public IReceivedMessageList Received => _received;
        public TimeSpan Timeout { get; }

        public ReceivedMessageList<T> Consume<T>()
            where T : class
        {
            var consumer = new Of<T>(this);
            var configure = new ConsumerConfigurator<T>(consumer);
            _configures.Add(configure);

            return consumer.Received;
        }

        public ReceivedMessageList<T> Fault<T>()
            where T : class
        {
            var consumer = new FaultOf<T>(this);
            var configure = new ConsumerConfigurator<T>(consumer);
            _configures.Add(configure);

            return consumer.Received;
        }

        public ConnectHandle Connect(IConsumePipeConnector bus)
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (var configure in _configures)
                {
                    var handle = configure.Connect(bus);

                    handles.Add(handle);
                }

                return new MultipleConnectHandle(handles);
            }
            catch (Exception)
            {
                foreach (var handle in handles)
                    handle.Dispose();
                throw;
            }
        }

        public void Configure(IReceiveEndpointConfigurator configurator)
        {
            foreach (var configure in _configures)
                configure.Configure(configurator);
        }


        class ConsumerConfigurator<T> :
            IConsumerConfigurator
            where T : class
        {
            readonly IConsumer<T> _consumer;

            public ConsumerConfigurator(IConsumer<T> consumer)
            {
                _consumer = consumer;
            }

            public ConnectHandle Connect(IConsumePipeConnector bus)
            {
                return bus.ConnectInstance(_consumer);
            }

            public void Configure(IReceiveEndpointConfigurator configurator)
            {
                configurator.Instance(_consumer);
            }
        }


        interface IConsumerConfigurator
        {
            ConnectHandle Connect(IConsumePipeConnector bus);
            void Configure(IReceiveEndpointConfigurator configurator);
        }


        class Of<T> :
            IConsumer<T>
            where T : class
        {
            readonly MultiTestConsumer _multiConsumer;
            readonly ReceivedMessageList<T> _received;

            public Of(MultiTestConsumer multiConsumer)
            {
                _multiConsumer = multiConsumer;
                _received = new ReceivedMessageList<T>(multiConsumer.Timeout);
            }

            public ReceivedMessageList<T> Received => _received;

            public Task Consume(ConsumeContext<T> context)
            {
                _received.Add(context);
                _multiConsumer._received.Add(context);

                return TaskUtil.Completed;
            }
        }


        class FaultOf<T> :
            IConsumer<T>
            where T : class
        {
            readonly MultiTestConsumer _multiConsumer;
            readonly ReceivedMessageList<T> _received;

            public FaultOf(MultiTestConsumer multiConsumer)
            {
                _multiConsumer = multiConsumer;
                _received = new ReceivedMessageList<T>(multiConsumer.Timeout);
            }

            public ReceivedMessageList<T> Received => _received;

            public Task Consume(ConsumeContext<T> context)
            {
                _received.Add(context);
                _multiConsumer._received.Add(context);

                throw new InvalidOperationException("This is intentional from a test");
            }
        }
    }
}
