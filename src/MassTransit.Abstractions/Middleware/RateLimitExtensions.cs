namespace MassTransit
{
    using System.Threading.Tasks;
    using Contracts;


    public static class RateLimitExtensions
    {
        public static Task SetRateLimit(this IPipe<CommandContext> pipe, int rateLimit)
        {
            return pipe.SendCommand<SetRateLimit>(new Limit(rateLimit));
        }


        class Limit :
            SetRateLimit
        {
            public Limit(int rateLimit)
            {
                RateLimit = rateLimit;
            }

            public int RateLimit { get; }
        }
    }
}
