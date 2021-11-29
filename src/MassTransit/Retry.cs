namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Observables;
    using RetryPolicies;
    using RetryPolicies.ExceptionFilters;


    public static class Retry
    {
        static readonly IExceptionFilter _all = new AllExceptionFilter();

        /// <summary>
        /// Create a policy that does not retry any messages
        /// </summary>
        public static IRetryPolicy None { get; } = new NoRetryPolicy(new AllExceptionFilter());

        /// <summary>
        /// Create an immediate retry policy with the specified number of retries, with no
        /// delay between attempts.
        /// </summary>
        /// <param name="retryLimit">The number of retries to attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Immediate(int retryLimit)
        {
            return new ImmediateRetryPolicy(All(), retryLimit);
        }

        /// <summary>
        /// Create an immediate retry policy with the specified number of retries, with no
        /// delay between attempts.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="retryLimit">The number of retries to attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Immediate(this IExceptionFilter filter, int retryLimit)
        {
            return new ImmediateRetryPolicy(filter, retryLimit);
        }

        /// <summary>
        /// Create an interval retry policy with the specified intervals. The retry count equals
        /// the number of intervals provided
        /// </summary>
        /// <param name="intervals">The intervals before each subsequent retry attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Intervals(params TimeSpan[] intervals)
        {
            return new IntervalRetryPolicy(All(), intervals);
        }

        /// <summary>
        /// Create an interval retry policy with the specified intervals. The retry count equals
        /// the number of intervals provided
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="intervals">The intervals before each subsequent retry attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Intervals(this IExceptionFilter filter, params TimeSpan[] intervals)
        {
            return new IntervalRetryPolicy(filter, intervals);
        }

        /// <summary>
        /// Create an interval retry policy with the specified intervals. The retry count equals
        /// the number of intervals provided
        /// </summary>
        /// <param name="intervals">The intervals before each subsequent retry attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Intervals(params int[] intervals)
        {
            return new IntervalRetryPolicy(All(), intervals);
        }

        /// <summary>
        /// Create an interval retry policy with the specified intervals. The retry count equals
        /// the number of intervals provided
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="intervals">The intervals before each subsequent retry attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Intervals(this IExceptionFilter filter, params int[] intervals)
        {
            return new IntervalRetryPolicy(filter, intervals);
        }

        /// <summary>
        /// Create an interval retry policy with the specified number of retries at a fixed interval
        /// </summary>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <param name="interval">The interval between each retry attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Interval(int retryCount, TimeSpan interval)
        {
            return new IntervalRetryPolicy(All(), Enumerable.Repeat(interval, retryCount).ToArray());
        }

        /// <summary>
        /// Create an interval retry policy with the specified number of retries at a fixed interval
        /// </summary>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <param name="interval">The interval between each retry attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Interval(int retryCount, int interval)
        {
            return new IntervalRetryPolicy(All(), Enumerable.Repeat(interval, retryCount).ToArray());
        }

        /// <summary>
        /// Create an interval retry policy with the specified number of retries at a fixed interval
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <param name="interval">The interval between each retry attempt</param>
        /// <returns></returns>
        public static IRetryPolicy Interval(this IExceptionFilter filter, int retryCount, TimeSpan interval)
        {
            return new IntervalRetryPolicy(filter, Enumerable.Repeat(interval, retryCount).ToArray());
        }

        /// <summary>
        /// Create an exponential retry policy with the specified number of retries at exponential
        /// intervals
        /// </summary>
        /// <param name="retryLimit"></param>
        /// <param name="minInterval"></param>
        /// <param name="maxInterval"></param>
        /// <param name="intervalDelta"></param>
        /// <returns></returns>
        public static IRetryPolicy Exponential(int retryLimit, TimeSpan minInterval, TimeSpan maxInterval,
            TimeSpan intervalDelta)
        {
            return new ExponentialRetryPolicy(All(), retryLimit, minInterval, maxInterval, intervalDelta);
        }

        /// <summary>
        /// Create an exponential retry policy that never gives up
        /// intervals
        /// </summary>
        /// <param name="minInterval"></param>
        /// <param name="maxInterval"></param>
        /// <param name="intervalDelta"></param>
        /// <returns></returns>
        public static IRetryPolicy Exponential(TimeSpan minInterval, TimeSpan maxInterval, TimeSpan intervalDelta)
        {
            return new ExponentialRetryPolicy(All(), int.MaxValue, minInterval, maxInterval, intervalDelta);
        }

        /// <summary>
        /// Create an exponential retry policy with the specified number of retries at exponential
        /// intervals
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="retryLimit"></param>
        /// <param name="minInterval"></param>
        /// <param name="maxInterval"></param>
        /// <param name="intervalDelta"></param>
        /// <returns></returns>
        public static IRetryPolicy Exponential(this IExceptionFilter filter, int retryLimit,
            TimeSpan minInterval, TimeSpan maxInterval,
            TimeSpan intervalDelta)
        {
            return new ExponentialRetryPolicy(filter, retryLimit, minInterval, maxInterval, intervalDelta);
        }

        /// <summary>
        /// Create an incremental retry policy with the specified number of retry attempts with an incrementing
        /// interval between retries
        /// </summary>
        /// <param name="retryLimit">The number of retry attempts</param>
        /// <param name="initialInterval">The initial retry interval</param>
        /// <param name="intervalIncrement">The interval to add to the retry interval with each subsequent retry</param>
        /// <returns></returns>
        public static IRetryPolicy Incremental(int retryLimit, TimeSpan initialInterval,
            TimeSpan intervalIncrement)
        {
            return new IncrementalRetryPolicy(All(), retryLimit, initialInterval, intervalIncrement);
        }

        /// <summary>
        /// Create an incremental retry policy with the specified number of retry attempts with an incrementing
        /// interval between retries
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="retryLimit">The number of retry attempts</param>
        /// <param name="initialInterval">The initial retry interval</param>
        /// <param name="intervalIncrement">The interval to add to the retry interval with each subsequent retry</param>
        /// <returns></returns>
        public static IRetryPolicy Incremental(this IExceptionFilter filter, int retryLimit,
            TimeSpan initialInterval,
            TimeSpan intervalIncrement)
        {
            return new IncrementalRetryPolicy(filter, retryLimit, initialInterval, intervalIncrement);
        }

        public static IRetryPolicy CreatePolicy(Action<IRetryConfigurator> configure)
        {
            var configurator = new RetryConfigurator();

            configure(configurator);

            return configurator.Build();
        }

        /// <summary>
        /// Retry all exceptions except for the exception types specified
        /// </summary>
        /// <param name="exceptionTypes"></param>
        /// <returns></returns>
        public static IExceptionFilter Except(params Type[] exceptionTypes)
        {
            return new IgnoreExceptionFilter(exceptionTypes);
        }

        /// <summary>
        /// Retry all exceptions except for the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IExceptionFilter Except<T1>()
        {
            return new IgnoreExceptionFilter(typeof(T1));
        }

        /// <summary>
        /// Retry all exceptions except for the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IExceptionFilter Except<T1, T2>()
        {
            return new IgnoreExceptionFilter(typeof(T1), typeof(T2));
        }

        /// <summary>
        /// Retry all exceptions except for the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IExceptionFilter Except<T1, T2, T3>()
        {
            return new IgnoreExceptionFilter(typeof(T1), typeof(T2), typeof(T3));
        }

        /// <summary>
        /// Retry only the exception types specified
        /// </summary>
        /// <param name="exceptionTypes"></param>
        /// <returns></returns>
        public static IExceptionFilter Selected(params Type[] exceptionTypes)
        {
            return new HandleExceptionFilter(exceptionTypes);
        }

        /// <summary>
        /// Retry only the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IExceptionFilter Selected<T1>()
        {
            return new HandleExceptionFilter(typeof(T1));
        }

        /// <summary>
        /// Retry only the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IExceptionFilter Selected<T1, T2>()
        {
            return new HandleExceptionFilter(typeof(T1), typeof(T2));
        }

        /// <summary>
        /// Retry only the exception types specified
        /// </summary>
        /// <returns></returns>
        public static IExceptionFilter Selected<T1, T2, T3>()
        {
            return new HandleExceptionFilter(typeof(T1), typeof(T2), typeof(T3));
        }

        /// <summary>
        /// Retry all exceptions
        /// </summary>
        /// <returns></returns>
        public static IExceptionFilter All()
        {
            return _all;
        }

        /// <summary>
        /// Filter an exception type
        /// </summary>
        /// <typeparam name="T">The exception type</typeparam>
        /// <param name="filter">The filter expression</param>
        /// <returns>True if the exception should be retried, otherwise false</returns>
        public static IExceptionFilter Filter<T>(Func<T, bool> filter)
            where T : Exception
        {
            return new FilterExceptionFilter<T>(filter);
        }


        class RetryConfigurator :
            ExceptionSpecification,
            IRetryConfigurator,
            ISpecification
        {
            readonly RetryObservable _observers;
            RetryPolicyFactory _policyFactory;

            public RetryConfigurator()
            {
                _observers = new RetryObservable();
            }

            public void SetRetryPolicy(RetryPolicyFactory factory)
            {
                _policyFactory = factory;
            }

            ConnectHandle IRetryObserverConnector.ConnectRetryObserver(IRetryObserver observer)
            {
                return _observers.Connect(observer);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                if (_policyFactory == null)
                    yield return this.Failure("RetryPolicy", "must not be null");
            }

            public IRetryPolicy Build()
            {
                IReadOnlyList<ValidationResult> result = Validate().ThrowIfContainsFailure("The retry configuration is invalid:");

                try
                {
                    return _policyFactory(Filter);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationException(result, "An exception occurred during retry policy creation", ex);
                }
            }
        }
    }
}
