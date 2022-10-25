namespace ContainerConsumers;

using System.Threading.Tasks;
using MassTransit;

public record AllocateInventory
{
    public string Sku { get; init; }
}

class AllocateInventoryConsumer :
    IConsumer<AllocateInventory>
{
    public async Task Consume(ConsumeContext<AllocateInventory> context)
    {
    }
}
