namespace MassTransit.Initializers.Factories
{
    using Conventions;


    public interface IPropertyInitializerInspector<in TMessage, in TInput>
        where TMessage : class
        where TInput : class
    {
        bool Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention);
    }
}
