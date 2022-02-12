#nullable enable
namespace MassTransit.Initializers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Conventions;


    public static class MessageInitializer
    {
        static readonly IList<IInitializerConvention> _conventions;
        static IInitializerConvention[]? _conventionsArray;

        static MessageInitializer()
        {
            _conventions = new List<IInitializerConvention>
            {
                new DefaultInitializerConvention(),
                new DictionaryInitializerConvention()
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
            var baseContext = new ScopeInitializeContext(context);

            return _factory.Create(baseContext);
        }

        public InitializeContext<TMessage> Create(CancellationToken cancellationToken)
        {
            var baseContext = new BaseInitializeContext(cancellationToken);

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

        public Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object input, IPipe<SendContext<TMessage>>? pipe)
        {
            return PrepareSendTuple(Create(context), (TInput)input, pipe);
        }

        public Task<SendTuple<TMessage>> InitializeMessage(object input, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
        {
            return PrepareSendTuple(Create(cancellationToken), (TInput)input, pipe);
        }

        public async Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object input, object?[] moreInputs, IPipe<SendContext<TMessage>>? pipe)
        {
            InitializeContext<TMessage> initializeContext = Create(context);

            for (var i = 0; i < moreInputs.Length; i++)
            {
                var moreInput = moreInputs[i];
                if (moreInput != null)
                {
                    IMessageInitializer<TMessage> initializer = MessageInitializerCache<TMessage>.GetInitializer(moreInput.GetType());

                    initializeContext = await initializer.Initialize(initializeContext, moreInput).ConfigureAwait(false);
                }
            }

            return await PrepareSendTuple(initializeContext, (TInput)input, pipe).ConfigureAwait(false);
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

            return messageContext;
        }

        async Task<SendTuple<TMessage>> PrepareSendTuple(InitializeContext<TMessage> messageContext, TInput input, IPipe<SendContext<TMessage>>? pipe = null)
        {
            InitializeContext<TMessage, TInput> inputContext = messageContext.CreateInputContext(input);

            await Task.WhenAll(_initializers.Select(x => x.Apply(inputContext))).ConfigureAwait(false);

            return _headerInitializers.Length > 0
                ? new SendTuple<TMessage>(inputContext.Message, new InitializerSendContextPipe(_headerInitializers, inputContext, pipe))
                : new SendTuple<TMessage>(inputContext.Message, pipe);
        }


        class InitializerSendContextPipe :
            IPipe<SendContext<TMessage>>
        {
            readonly InitializeContext<TMessage, TInput> _context;
            readonly IHeaderInitializer<TMessage, TInput>[] _initializers;
            readonly IPipe<SendContext<TMessage>>? _pipe;

            public InitializerSendContextPipe(IHeaderInitializer<TMessage, TInput>[] initializers, InitializeContext<TMessage, TInput> context,
                IPipe<SendContext<TMessage>>? pipe)
            {
                _initializers = initializers;
                _pipe = pipe;
                _context = context;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public async Task Send(SendContext<TMessage> context)
            {
                await Task.WhenAll(_initializers.Select(x => x.Apply(_context, context))).ConfigureAwait(false);

                if (_pipe != null && _pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);
            }
        }
    }
}
