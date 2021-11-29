namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Linq;
    using Contracts;


    public class CommandContextRetryPolicy :
        IRetryPolicy
    {
        readonly IRetryPolicy _retryPolicy;

        public CommandContextRetryPolicy(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("retry-consumeContext");

            _retryPolicy.Probe(scope);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            if (context is CommandContext consumeContext)
            {
                RetryPolicyContext<CommandContext> retryPolicyContext = _retryPolicy.CreatePolicyContext(consumeContext);

                var innerType = context.GetType().GetClosingArguments(typeof(CommandContext<>)).Single();

                var retryConsumeContext = (RetryCommandContext)Activator.CreateInstance(typeof(RetryCommandContext<>).MakeGenericType(innerType), context);

                return new CommandContextRetryPolicyContext(retryPolicyContext, retryConsumeContext) as RetryPolicyContext<T>;
            }

            throw new ArgumentException("The argument must be a ConsumeContext", nameof(context));
        }

        public bool IsHandled(Exception exception)
        {
            return _retryPolicy.IsHandled(exception);
        }
    }


    public class ConsumeContextRetryPolicy<TFilter, TContext> :
        IRetryPolicy
        where TFilter : class, CommandContext
        where TContext : RetryCommandContext, TFilter
    {
        readonly Func<TFilter, TContext> _contextFactory;
        readonly IRetryPolicy _retryPolicy;

        public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy, Func<TFilter, TContext> contextFactory)
        {
            _retryPolicy = retryPolicy;
            _contextFactory = contextFactory;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("retry-consumeContext");

            _retryPolicy.Probe(scope);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            var filterContext = context as TFilter;
            if (filterContext == null)
                throw new ArgumentException($"The argument must be a {typeof(TFilter).Name}", nameof(context));

            RetryPolicyContext<TFilter> retryPolicyContext = _retryPolicy.CreatePolicyContext(filterContext);

            var retryConsumeContext = _contextFactory(filterContext);

            return new CommandContextRetryPolicyContext<TFilter, TContext>(retryPolicyContext, retryConsumeContext) as RetryPolicyContext<T>;
        }

        public bool IsHandled(Exception exception)
        {
            return _retryPolicy.IsHandled(exception);
        }
    }
}
