namespace ContainerConsumers
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using MassTransit.ConsumeConfigurators;
    using MassTransit.Definition;
    using Microsoft.Extensions.Logging;    

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