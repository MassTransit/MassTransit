namespace MassTransit.Pipeline
{
    using GreenPipes;


    public interface ISendPipe :
        ISendContextPipe,
        IProbeSite
    {
    }
}
