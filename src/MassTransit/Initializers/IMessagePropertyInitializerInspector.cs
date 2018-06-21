namespace MassTransit.Initializers
{
    using Conventions;


    public interface IMessagePropertyInitializerInspector<in TMessage, in TInput>
        where TMessage : class
        where TInput : class
    {
        void Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention);
    }
}