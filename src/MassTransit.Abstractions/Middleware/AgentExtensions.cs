namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;
    using Middleware;


    public static class AgentExtensions
    {
        /// <summary>
        /// Stop the agent, using the default StopContext
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task Stop(this IAgent agent, CancellationToken cancellationToken = default)
        {
            var stopContext = new DefaultStopContext(cancellationToken);

            return agent.Stop(stopContext);
        }

        /// <summary>
        /// Stop the agent, using the default StopContext
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="reason">The reason for stopping the agent</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task Stop(this IAgent agent, string reason, CancellationToken cancellationToken = default)
        {
            var stopContext = new DefaultStopContext(cancellationToken) { Reason = reason };

            return agent.Stop(stopContext);
        }


        class DefaultStopContext :
            BasePipeContext,
            StopContext
        {
            public DefaultStopContext(CancellationToken cancellationToken)
                : base(cancellationToken)
            {
                Reason = "Stopped";
            }

            public string Reason { get; set; }
        }
    }
}
