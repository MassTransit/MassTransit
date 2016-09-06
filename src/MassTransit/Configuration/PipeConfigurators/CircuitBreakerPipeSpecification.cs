// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using GreenPipes;
    using Pipeline.Filters;
    using Pipeline.Filters.CircuitBreaker;


    public class CircuitBreakerPipeSpecification<T> :
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
            set { _settings.TrackingPeriod = value; }
        }

        public int TripThreshold
        {
            set { _settings.TripThreshold = value; }
        }

        public int ActiveThreshold
        {
            set { _settings.ActiveThreshold = value; }
        }

        public void ResetInterval(TimeSpan interval)
        {
            _settings.ResetTimeout = IntervalTimeout(interval);
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new CircuitBreakerFilter<T>(_settings));
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
        }
    }
}