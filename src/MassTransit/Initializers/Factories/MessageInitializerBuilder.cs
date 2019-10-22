namespace MassTransit.Initializers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Metadata;


    public class MessageInitializerBuilder<TMessage, TInput> :
        IMessageInitializerBuilder<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IMessageFactory<TMessage> _messageFactory;
        readonly IDictionary<string, IPropertyInitializer<TMessage, TInput>> _initializers;
        readonly IList<IHeaderInitializer<TMessage, TInput>> _headerInitializers;
        readonly HashSet<string> _inputPropertyUsed;

        public MessageInitializerBuilder(IMessageFactory<TMessage> messageFactory)
        {
            if (!TypeMetadataCache<TMessage>.IsValidMessageType)
                throw new ArgumentException(TypeMetadataCache<TMessage>.InvalidMessageTypeReason, nameof(TMessage));

            _messageFactory = messageFactory;

            _initializers = new Dictionary<string, IPropertyInitializer<TMessage, TInput>>(StringComparer.OrdinalIgnoreCase);
            _headerInitializers = new List<IHeaderInitializer<TMessage, TInput>>();
            _inputPropertyUsed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public IMessageInitializer<TMessage> Build()
        {
            IMessageFactory<TMessage> messageFactory = _messageFactory ?? MessageFactoryCache<TMessage>.Factory;

            return new MessageInitializer<TMessage, TInput>(messageFactory, _initializers.Values, _headerInitializers);
        }

        public void Add(string propertyName, IPropertyInitializer<TMessage> initializer)
        {
            _initializers[propertyName] = new PropertyAdapter(initializer);
        }

        public void Add(string propertyName, IPropertyInitializer<TMessage, TInput> initializer)
        {
            _initializers[propertyName] = initializer;
        }

        public void Add(IHeaderInitializer<TMessage> initializer)
        {
            _headerInitializers.Add(new HeaderAdapter(initializer));
        }

        public void Add(IHeaderInitializer<TMessage, TInput> initializer)
        {
            _headerInitializers.Add(initializer);
        }

        public bool IsInputPropertyUsed(string propertyName)
        {
            return _inputPropertyUsed.Contains(propertyName);
        }

        public void SetInputPropertyUsed(string propertyName)
        {
            _inputPropertyUsed.Add(propertyName);
        }


        class PropertyAdapter :
            IPropertyInitializer<TMessage, TInput>
        {
            readonly IPropertyInitializer<TMessage> _initializer;

            public PropertyAdapter(IPropertyInitializer<TMessage> initializer)
            {
                _initializer = initializer;
            }

            public Task Apply(InitializeContext<TMessage, TInput> context)
            {
                return _initializer.Apply(context);
            }
        }


        class HeaderAdapter :
            IHeaderInitializer<TMessage, TInput>
        {
            readonly IHeaderInitializer<TMessage> _initializer;

            public HeaderAdapter(IHeaderInitializer<TMessage> initializer)
            {
                _initializer = initializer;
            }

            public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
            {
                return _initializer.Apply(context, sendContext);
            }
        }
    }
}
