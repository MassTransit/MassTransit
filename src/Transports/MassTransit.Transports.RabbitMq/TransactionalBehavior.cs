namespace MassTransit.Transports.RabbitMq
{
    public interface TransactionStrategy
    {
        
    }

    public class NonTransactionalBehavior : TransactionStrategy
    {
        
    }
    public class TransactionalBehavior : TransactionStrategy
    {
        
    }
}