namespace MassTransit.Transports.Components
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    public class KillSwitchOptions :
        IOptions,
        ISpecification
    {
        public KillSwitchOptions()
        {
            ActivationThreshold = 100;
            TripThreshold = 10;
            TrackingPeriod = TimeSpan.FromMinutes(1);
            RestartTimeout = TimeSpan.FromSeconds(60);

            ExceptionFilter = new FilterSpecification().Build();
        }

        /// <summary>
        /// The time window for tracking exceptions
        /// </summary>
        public TimeSpan TrackingPeriod { get; set; }

        /// <summary>
        /// The wait time before restarting the receive endpoint
        /// </summary>
        public TimeSpan RestartTimeout { get; set; }

        /// <summary>
        /// The percentage of failed messages that triggers the kill switch. Should be 0-100, but seriously like 5-10.
        /// </summary>
        public int TripThreshold { get; set; }

        /// <summary>
        /// The number of messages that must be consumed before the kill switch activates.
        /// </summary>
        public int ActivationThreshold { get; set; }

        /// <summary>
        /// By default, all exceptions are tracked. An exception filter can be specified (using <see cref="SetExceptionFilter" /> to only track
        /// specific exceptions.
        /// </summary>
        public IExceptionFilter ExceptionFilter { get; private set; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (ActivationThreshold < 1)
                yield return this.Failure(nameof(ActivationThreshold), "must be >= 1");
            if (TripThreshold < 0 || TripThreshold > 100)
                yield return this.Failure(nameof(TripThreshold), "must be >= 0 and <= 100");
            if (TrackingPeriod <= TimeSpan.Zero)
                yield return this.Failure(nameof(TrackingPeriod), "must be > 0");
            if (RestartTimeout < TimeSpan.FromSeconds(1))
                yield return this.Failure(nameof(RestartTimeout), "must be at least one second");
            if (ExceptionFilter == null)
                yield return this.Failure(nameof(ExceptionFilter), "must not be null");
        }

        /// <summary>
        /// The number of messages that must be consumed before the kill switch activates.
        /// </summary>
        public KillSwitchOptions SetActivationThreshold(int value)
        {
            ActivationThreshold = value;
            return this;
        }

        /// <summary>
        /// The percentage of failed messages that triggers the kill switch. Should be 0-100, but seriously like 5-10.
        /// </summary>
        public KillSwitchOptions SetTripThreshold(int value)
        {
            TripThreshold = value;
            return this;
        }

        /// <summary>
        /// The percentage of failed messages that triggers the kill switch. Should be 0-100, but seriously like 5-10.
        /// </summary>
        public KillSwitchOptions SetTripThreshold(double percentage)
        {
            TripThreshold = (int)(percentage * 100.0);
            return this;
        }

        /// <summary>
        /// The time window for tracking exceptions
        /// </summary>
        public KillSwitchOptions SetTrackingPeriod(TimeSpan value)
        {
            TrackingPeriod = value;
            return this;
        }

        /// <summary>
        /// The time window for tracking exceptions
        /// </summary>
        public KillSwitchOptions SetTrackingPeriod(int? d = null, int? h = null, int? m = null, int? s = null, int? ms = null)
        {
            var value = new TimeSpan(d ?? 0, h ?? 0, m ?? 0, s ?? 0, ms ?? 0);
            if (value <= TimeSpan.Zero)
                throw new ArgumentException("The tracking period must be > 0");

            TrackingPeriod = value;
            return this;
        }

        /// <summary>
        /// The wait time before restarting the receive endpoint
        /// </summary>
        public KillSwitchOptions SetRestartTimeout(TimeSpan value)
        {
            RestartTimeout = value;

            return this;
        }

        /// <summary>
        /// The wait time before restarting the receive endpoint
        /// </summary>
        public KillSwitchOptions SetRestartTimeout(int? d = null, int? h = null, int? m = null, int? s = null, int? ms = null)
        {
            var value = new TimeSpan(d ?? 0, h ?? 0, m ?? 0, s ?? 0, ms ?? 0);
            if (value <= TimeSpan.Zero)
                throw new ArgumentException("The tracking period must be > 0");

            RestartTimeout = value;

            return this;
        }

        /// <summary>
        /// By default, all exceptions are tracked. An exception filter can be configured to only track specific exceptions.
        /// </summary>
        public KillSwitchOptions SetExceptionFilter(Action<IExceptionConfigurator> configure)
        {
            var specification = new FilterSpecification();

            configure?.Invoke(specification);

            ExceptionFilter = specification.Build();

            return this;
        }


        class FilterSpecification :
            ExceptionSpecification
        {
            public IExceptionFilter Build()
            {
                return Filter;
            }
        }
    }
}
