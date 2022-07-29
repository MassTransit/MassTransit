namespace MassTransit.Util
{
    using System;
    using System.Threading.Tasks;
    using Logging;


    static class ThrowHelper
    {
        public static class ConsumeFilter
        {
            public static async Task Throw<TConsumer, TMessage>(ConsumeContext<TMessage> context, TimeSpan duration, Exception exception, StartedActivity? activity)
                where TMessage : class
            {
                await context.NotifyFaulted(duration, TypeCache<TConsumer>.ShortName, exception).ConfigureAwait(false);

                activity?.RecordException(exception, escaped: true);

                throw exception;
            }

            public static async Task ThrowOperationCancelled<TConsumer, TMessage>(ConsumeContext<TMessage> context, TimeSpan duration, OperationCanceledException exception, StartedActivity? activity)
                where TMessage : class
            {
                await context.NotifyFaulted(duration, TypeCache<TConsumer>.ShortName, exception).ConfigureAwait(false);

                activity?.RecordException(exception, escaped: exception.CancellationToken == context.CancellationToken);

                if (exception.CancellationToken == context.CancellationToken)
                    throw exception;

                ThrowConsumerCanceled<TConsumer>(activity, escaped: true);
            }

            public static void ThrowConsumerCanceled<TConsumer>(StartedActivity? activity, bool escaped)
            {
                var exception = new ConsumerCanceledException($"The operation was canceled by the consumer: {TypeCache<TConsumer>.ShortName}");

                activity?.RecordException(exception, escaped);

                throw exception;
            }
        }
    }
}
