namespace MassTransit.Initializers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Conventions;
    using GreenPipes.Internals.Extensions;


    public class MessageInitializerFactory<TMessage, TInput> :
        IMessageInitializerFactory<TMessage>
        where TMessage : class
        where TInput : class
    {
        readonly IInitializerConvention[] _conventions;

        public MessageInitializerFactory(IInitializerConvention[] conventions)
        {
            _conventions = conventions;
        }

        public IMessageInitializer<TMessage> CreateMessageInitializer()
        {
            var builder = new MessageInitializerBuilder<TMessage, TInput>();

            IPropertyInitializerInspector<TMessage, TInput>[] inspectors = CreatePropertyInspectors().ToArray();

            foreach (IPropertyInitializerInspector<TMessage, TInput> inspector in inspectors)
            {
                foreach (var convention in _conventions)
                {
                    if (inspector.Apply(builder, convention))
                        break;
                }
            }

            return builder.Build();
        }

        static IEnumerable<IPropertyInitializerInspector<TMessage, TInput>> CreatePropertyInspectors()
        {
            return typeof(TMessage).GetAllProperties().Where(x => x.CanRead)
                .Select(x => (IPropertyInitializerInspector<TMessage, TInput>)Activator.CreateInstance(
                    typeof(PropertyInitializerInspector<,,>).MakeGenericType(typeof(TMessage), typeof(TInput), x.PropertyType), x.Name));
        }
    }
}
