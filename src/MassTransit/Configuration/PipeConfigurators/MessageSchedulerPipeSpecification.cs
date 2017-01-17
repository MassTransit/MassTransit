// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Pipeline.Filters;


    public class MessageSchedulerPipeSpecification :
        IPipeSpecification<ConsumeContext>
    {
        readonly Uri _schedulerAddress;

        public MessageSchedulerPipeSpecification(Uri schedulerAddress)
        {
            _schedulerAddress = schedulerAddress;
        }

        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new MessageSchedulerFilter(_schedulerAddress));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_schedulerAddress == null)
                yield return this.Failure("SchedulerAddress", "must not be null");
        }
    }
}