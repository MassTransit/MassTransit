// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Configuration.Specifications
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline;


    public class RenewLockSpecification :
        IPipeSpecification<ConsumeContext>
    {
        readonly TimeSpan _delay;

        public RenewLockSpecification(TimeSpan? delay = default(TimeSpan?))
        {
            _delay = delay ?? TimeSpan.FromSeconds(60);
        }

        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new RenewLockFilter(_delay));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_delay <= TimeSpan.Zero || _delay >= TimeSpan.FromMinutes(5))
                yield return this.Failure("Delay", "Must be > 0 and < 5 minutes");
        }
    }
}