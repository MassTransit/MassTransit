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
    using PipeBuilders;
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

        public TimeSpan Duration
        {
            set { _settings.Duration = value; }
        }

        public int TripThreshold
        {
            set { _settings.TripThreshold = value; }
        }

        public int ActiveCount
        {
            set { _settings.ActiveCount = value; }
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new CircuitBreakerFilter<T>(_settings));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings.ActiveCount < 1)
                yield return this.Failure("ActiveCount", "must be >= 1");
            if (_settings.TripThreshold < 0 || _settings.TripThreshold > 100)
                yield return this.Failure("TripThreshold", "must be >= 0 and <= 100");
            if (_settings.Duration <= TimeSpan.Zero)
                yield return this.Failure("Duration", "must be > 0");
            if (!_settings.ResetTimeout.Any())
                yield return this.Failure("ResetTimeout", "must specify at least one value");
        }


        class Settings :
            CircuitBreakerSettings
        {
            public Settings()
            {
                ActiveCount = 5;
                TripThreshold = 5;
                Duration = TimeSpan.FromMinutes(1);
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

            public TimeSpan Duration { get; set; }

            public IEnumerable<TimeSpan> ResetTimeout { get; private set; }

            public int TripThreshold { get; set; }

            public int ActiveCount { get; set; }
        }
    }
}