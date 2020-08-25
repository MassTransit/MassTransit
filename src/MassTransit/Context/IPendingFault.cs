namespace MassTransit.Context
{
    using System.Threading.Tasks;


    interface IPendingFault
    {
        Task Notify(ConsumeContext context);
    }
}
