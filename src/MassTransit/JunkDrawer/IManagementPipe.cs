namespace MassTransit.JunkDrawer
{
    using Middleware;


    /// <summary>
    /// A management pipe is used by filters to communicate with the outside world, for management
    /// purposes such as configuring the acceptance of command and request/response messages.
    /// </summary>
    public interface IManagementPipe :
        IDynamicRouter<ConsumeContext>,
        IConsumePipeConnector,
        IRequestPipeConnector
    {
    }
}
