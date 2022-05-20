namespace ContainerConsumers
{
    using System.Threading.Tasks;
    using MassTransit;

    public interface AllocateInventory
    {
    }

    class AllocateInventoryConsumer :
        IConsumer<AllocateInventory>
    {
        public async Task Consume(ConsumeContext<AllocateInventory> context)
        {
        }
    }
}
