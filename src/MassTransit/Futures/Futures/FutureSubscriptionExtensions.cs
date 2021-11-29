namespace MassTransit.Futures
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Middleware;
    using SagaStateMachine;


    public static class FutureSubscriptionExtensions
    {
        public static async Task<T> SendMessageToSubscriptions<T>(this BehaviorContext<FutureState> context,
            ContextMessageFactory<BehaviorContext<FutureState>, T> factory, IEnumerable<FutureSubscription> subscriptions)
            where T : class
        {
            return await factory.Use(context, async (ctx, s) =>
            {
                List<Task> tasks = subscriptions.Select(async sub =>
                {
                    var endpoint = await context.GetSendEndpoint(sub.Address).ConfigureAwait(false);

                    if (sub.RequestId.HasValue)
                    {
                        var pipe = new FutureResultPipe<T>(s.Pipe, sub.RequestId.Value);

                        await endpoint.Send(s.Message, pipe, context.CancellationToken).ConfigureAwait(false);
                    }
                    else
                        await endpoint.Send(s.Message, s.Pipe, context.CancellationToken).ConfigureAwait(false);
                }).ToList();

                await Task.WhenAll(tasks).ConfigureAwait(false);

                return s.Message;
            }).ConfigureAwait(false);
        }

        public static async Task<T> SendMessageToSubscriptions<TInput, T>(this BehaviorContext<FutureState, TInput> context,
            ContextMessageFactory<BehaviorContext<FutureState, TInput>, T> factory, IEnumerable<FutureSubscription> subscriptions)
            where TInput : class
            where T : class
        {
            return await factory.Use(context, async (ctx, s) =>
            {
                List<Task> tasks = subscriptions.Select(async sub =>
                {
                    var endpoint = await context.GetSendEndpoint(sub.Address).ConfigureAwait(false);

                    if (sub.RequestId.HasValue)
                    {
                        var pipe = new FutureResultPipe<T>(s.Pipe, sub.RequestId.Value);

                        await endpoint.Send(s.Message, pipe, context.CancellationToken).ConfigureAwait(false);
                    }
                    else
                        await endpoint.Send(s.Message, s.Pipe, context.CancellationToken).ConfigureAwait(false);
                }).ToList();

                await Task.WhenAll(tasks).ConfigureAwait(false);

                return s.Message;
            }).ConfigureAwait(false);
        }
    }
}
