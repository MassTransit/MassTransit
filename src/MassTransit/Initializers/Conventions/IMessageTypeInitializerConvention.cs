namespace MassTransit.Initializers.Conventions
{
    public interface IMessageTypeInitializerConvention<in TMessage>
        where TMessage : class
    {
    }


    public interface IMessageTypeInitializerConvention
    {
    }
}