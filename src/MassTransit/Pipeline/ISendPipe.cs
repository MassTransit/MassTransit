namespace MassTransit.Pipeline
{
    using GreenPipes;


    public interface ISendPipe :
        ISendContextPipe,
        ISendObserverConnector,
        ISendMessageObserverConnector,
        IProbeSite
    {
    }
}
