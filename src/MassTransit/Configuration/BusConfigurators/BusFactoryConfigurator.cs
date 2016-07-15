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
namespace MassTransit.BusConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using PipeConfigurators;


    public abstract class BusFactoryConfigurator
    {
        readonly ConsumePipeConfigurator _consumePipeConfigurator;
        readonly SendPipeConfigurator _sendPipeConfigurator;
        readonly PublishPipeConfigurator _publishPipeConfigurator;

        protected BusFactoryConfigurator()
        {
            _consumePipeConfigurator = new ConsumePipeConfigurator();
            _sendPipeConfigurator = new SendPipeConfigurator();
            _publishPipeConfigurator = new PublishPipeConfigurator();
        }

        protected IConsumePipeFactory ConsumePipeFactory => _consumePipeConfigurator;

        protected ISendPipeFactory SendPipeFactory => _sendPipeConfigurator;
        protected IPublishPipeFactory PublishPipeFactory => _publishPipeConfigurator;

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeConfigurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            _consumePipeConfigurator.AddPipeSpecification(specification);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_sendPipeConfigurator);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_publishPipeConfigurator);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _consumePipeConfigurator.Validate()
                .Concat(_sendPipeConfigurator.Validate());
        }
    }
}