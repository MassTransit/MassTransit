namespace MassTransit.Initializers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Conventions;
    using GreenPipes;


    public static class MessageInitializer
    {
        static readonly IList<IInitializerConvention> _conventions;
        static IInitializerConvention[] _conventionsArray;

        static MessageInitializer()
        {
            _conventions = new List<IInitializerConvention>
            {
                new DefaultInitializerConvention(),
                new DictionaryInitializerConvention(),
            };
        }

        public static IInitializerConvention[] Conventions => _conventionsArray ??= _conventions.ToArray();

        public static void AddConvention<T>()
            where T : IInitializerConvention, new()
        {
            var convention = new T();
            _conventions.Add(convention);
            _conventionsArray = null;
        }
    }


    /// <summary>
    /// Initializes a message using the input, which can include message properties, headers, etc.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TInput">The input type</typeparam>
    public class MessageInitializer<TMessage, TInput> :
        IMessageInitializer<TMessage>
        where TMessage : class
        where TInput : class
    {
        readonly IMessageFactory<TMessage> _factory;
        readonly IHeaderInitializer<TMessage, TInput>[] _headerInitializers;
        readonly IPropertyInitializer<TMessage, TInput>[] _initializers;

        public MessageInitializer(IMessageFactory<TMessage> factory, IEnumerable<IPropertyInitializer<TMessage, TInput>> initializers,
            IEnumerable<IHeaderInitializer<TMessage, TInput>> headerInitializers)
        {
            _factory = factory;
            _initializers = initializers.ToArray();
            _headerInitializers = headerInitializers.ToArray();
        }

        public InitializeContext<TMessage> Create(PipeContext context)
        {
            var baseContext = new ProxyInitializeContext(context);

            return _factory.Create(baseContext);
        }

        public Task<InitializeContext<TMessage>> Initialize(object input, CancellationToken cancellationToken)
        {
            return InitializeMessage((TInput)input, cancellationToken);
        }

        public Task<InitializeContext<TMessage>> Initialize(InitializeContext<TMessage> context, object input)
        {
            return InitializeMessage(context, (TInput)input);
        }

        public async Task<TMessage> Send(ISendEndpoint endpoint, object input, CancellationToken cancellationToken)
        {
            var messageContext = await PrepareMessage((TInput)input, cancellationToken).ConfigureAwait(false);

            if (_headerInitializers.Length > 0)
            {
                await endpoint.Send(messageContext.Message, new InitializerSendContextPipe(_headerInitializers, messageContext), cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await endpoint.Send(messageContext.Message, cancellationToken).ConfigureAwait(false);
            }

            return messageContext.Message;
        }

        public async Task<TMessage> Send(ISendEndpoint endpoint, InitializeContext<TMessage> context, object input)
        {
            InitializeContext<TMessage, TInput> messageContext = await PrepareMessage(context, (TInput)input).ConfigureAwait(false);

            if (_headerInitializers.Length > 0)
                await endpoint.Send(messageContext.Message, new InitializerSendContextPipe(_headerInitializers, messageContext),
                    messageContext.CancellationToken).ConfigureAwait(false);
            else
                await endpoint.Send(messageContext.Message, messageContext.CancellationToken).ConfigureAwait(false);

            return messageContext.Message;
        }

        public async Task<TMessage> Send(ISendEndpoint endpoint, object input, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
        {
            var messageContext = await PrepareMessage((TInput)input, cancellationToken).ConfigureAwait(false);

            await endpoint.Send(messageContext.Message,
                    _headerInitializers.Length > 0
                        ? new InitializerSendContextPipe(_headerInitializers, messageContext, pipe)
                        : pipe, cancellationToken)
                .ConfigureAwait(false);

            return messageContext.Message;
        }

        public async Task<TMessage> Send(ISendEndpoint endpoint, object input, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            InitializeContext<TMessage, TInput> messageContext = await PrepareMessage((TInput)input, cancellationToken).ConfigureAwait(false);

            if (_headerInitializers.Length > 0)
                await endpoint.Send(messageContext.Message, new InitializerSendContextPipe(_headerInitializers, messageContext, pipe), cancellationToken)
                    .ConfigureAwait(false);
            else
                await endpoint.Send(messageContext.Message, pipe, cancellationToken).ConfigureAwait(false);

            return messageContext.Message;
        }

        public async Task<TMessage> Send(ISendEndpoint endpoint, InitializeContext<TMessage> context, object input, IPipe<SendContext> pipe)
        {
            InitializeContext<TMessage, TInput> messageContext = await PrepareMessage(context, (TInput)input).ConfigureAwait(false);

            if (_headerInitializers.Length > 0)
                await endpoint.Send(messageContext.Message, new InitializerSendContextPipe(_headerInitializers, messageContext, pipe),
                    messageContext.CancellationToken).ConfigureAwait(false);
            else
                await endpoint.Send(messageContext.Message, pipe, messageContext.CancellationToken).ConfigureAwait(false);

            return messageContext.Message;
        }

        public async Task<TMessage> Send(ISendEndpoint endpoint, InitializeContext<TMessage> context, object input, IPipe<SendContext<TMessage>> pipe)
        {
            InitializeContext<TMessage, TInput> messageContext = await PrepareMessage(context, (TInput)input).ConfigureAwait(false);

            var sendPipe = _headerInitializers.Length > 0
                ? new InitializerSendContextPipe(_headerInitializers, messageContext, pipe)
                : pipe;

            await endpoint.Send(messageContext.Message, sendPipe, messageContext.CancellationToken).ConfigureAwait(false);

            return messageContext.Message;
        }

        Task<InitializeContext<TMessage>> InitializeMessage(TInput input, CancellationToken cancellationToken)
        {
            var context = new BaseInitializeContext(cancellationToken);

            InitializeContext<TMessage> messageContext = _factory.Create(context);

            return InitializeMessage(messageContext, input);
        }

        async Task<InitializeContext<TMessage>> InitializeMessage(InitializeContext<TMessage> messageContext, TInput input)
        {
            InitializeContext<TMessage, TInput> inputContext = messageContext.CreateInputContext(input);

            await Task.WhenAll(_initializers.Select(x => x.Apply(inputContext))).ConfigureAwait(false);

            return inputContext;
        }

        Task<InitializeContext<TMessage, TInput>> PrepareMessage(TInput input, CancellationToken cancellationToken)
        {
            var context = new BaseInitializeContext(cancellationToken);

            InitializeContext<TMessage> messageContext = _factory.Create(context);

            return PrepareMessage(messageContext, input);
        }

        async Task<InitializeContext<TMessage, TInput>> PrepareMessage(InitializeContext<TMessage> messageContext, TInput input)
        {
            InitializeContext<TMessage, TInput> inputContext = messageContext.CreateInputContext(input);

            await Task.WhenAll(_initializers.Select(x => x.Apply(inputContext))).ConfigureAwait(false);

            return inputContext;
        }


        class InitializerSendContextPipe :
            IPipe<SendContext<TMessage>>
        {
            readonly InitializeContext<TMessage, TInput> _context;
            readonly IHeaderInitializer<TMessage, TInput>[] _initializers;
            readonly IPipe<SendContext<TMessage>> _pipe;
            readonly IPipe<SendContext> _sendPipe;

            public InitializerSendContextPipe(IHeaderInitializer<TMessage, TInput>[] initializers, InitializeContext<TMessage, TInput> context)
            {
                _initializers = initializers;
                _context = context;
            }

            public InitializerSendContextPipe(IHeaderInitializer<TMessage, TInput>[] initializers, InitializeContext<TMessage, TInput> context,
                IPipe<SendContext<TMessage>> pipe)
            {
                _initializers = initializers;
                _pipe = pipe;
                _context = context;
            }

            public InitializerSendContextPipe(IHeaderInitializer<TMessage, TInput>[] initializers, InitializeContext<TMessage, TInput> context,
                IPipe<SendContext> pipe)
            {
                _initializers = initializers;
                _sendPipe = pipe;
                _context = context;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
                _sendPipe?.Probe(context);
            }

            public async Task Send(SendContext<TMessage> context)
            {
                await Task.WhenAll(_initializers.Select(x => x.Apply(_context, context))).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);

                if (_sendPipe.IsNotEmpty())
                    await _sendPipe.Send(context).ConfigureAwait(false);
            }
        }
    }
}
