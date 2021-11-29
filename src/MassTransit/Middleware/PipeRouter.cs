namespace MassTransit.Middleware
{
    public class PipeRouter :
        DynamicRouter<PipeContext>,
        IPipeRouter
    {
        public PipeRouter()
            : base(new PipeContextConverterFactory())
        {
        }
    }
}
