namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Middleware;
    using Middleware.CircuitBreaker;


    public class CircuitBreakerPipeSpecification<T> :
        ExceptionSpecification,
        IPipeSpecification<T>,
        ICircuitBreakerConfigurator<T>
        where T : class, PipeContext
    {
        readonly Settings _settings;

        public CircuitBreakerPipeSpecification()
        {
            _settings = new Settings();
        }

        public TimeSpan TrackingPeriod
        {
            set => _settings.TrackingPeriod = value;
        }

        public int TripThreshold
        {
            set => _settings.TripThreshold = value;
        }

        public int ActiveThreshold
        {
            set => _settings.ActiveThreshold = value;
        }

        public TimeSpan ResetInterval
        {
            set => _settings.ResetTimeout = IntervalTimeout(value);
        }

        public IPipeRouter Router
        {
            set => _settings.Router = value;
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new CircuitBreakerFilter<T>(_settings, Filter));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings.ActiveThreshold < 1)
                yield return this.Failure(nameof(_settings.ActiveThreshold), "must be >= 1");
            if (_settings.TripThreshold < 0 || _settings.TripThreshold > 100)
                yield return this.Failure(nameof(_settings.TripThreshold), "must be >= 0 and <= 100");
            if (_settings.TrackingPeriod <= TimeSpan.Zero)
                yield return this.Failure(nameof(_settings.TrackingPeriod), "must be > 0");
            if (!_settings.ResetTimeout.Any())
                yield return this.Failure(nameof(_settings.ResetTimeout), "must specify at least one value");
        }

        IEnumerable<TimeSpan> IntervalTimeout(TimeSpan interval)
        {
            while (true)
                yield return interval;
        }


        class Settings :
            CircuitBreakerSettings
        {
            public Settings()
            {
                ActiveThreshold = 5;
                TripThreshold = 5;
                TrackingPeriod = TimeSpan.FromMinutes(1);
                ResetTimeout = DefaultTimeout;
            }

            static IEnumerable<TimeSpan> DefaultTimeout
            {
                get
                {
                    yield return TimeSpan.FromMilliseconds(100);
                    yield return TimeSpan.FromMilliseconds(200);
                    yield return TimeSpan.FromMilliseconds(500);
                    yield return TimeSpan.FromSeconds(1);
                    yield return TimeSpan.FromSeconds(5);
                    yield return TimeSpan.FromSeconds(10);
                    yield return TimeSpan.FromSeconds(15);
                    yield return TimeSpan.FromSeconds(30);

                    while (true)
                        yield return TimeSpan.FromSeconds(60);
                }
            }

            public TimeSpan TrackingPeriod { get; set; }
            public IEnumerable<TimeSpan> ResetTimeout { get; set; }
            public int TripThreshold { get; set; }
            public int ActiveThreshold { get; set; }
            public IPipeRouter Router { get; set; }
        }
    }
}
