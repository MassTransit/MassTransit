namespace MassTransit
{
    public delegate byte[] PartitionKeyProvider<in TContext>(TContext context);
}
