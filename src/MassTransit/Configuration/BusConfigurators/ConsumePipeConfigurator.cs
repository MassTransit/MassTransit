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
    using Builders;
    using Configurators;
    using PipeBuilders;
    using PipeConfigurators;
    using Pipeline;


    public class ConsumePipeConfigurator :
        IConsumePipeConfigurator,
        IConsumePipeFactory,
        IConsumePipeSpecification
    {
        readonly IList<IConsumePipeSpecification> _specifications;

        public ConsumePipeConfigurator()
        {
            _specifications = new List<IConsumePipeSpecification>();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy(specification));
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification) where T : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy<T>(specification));
        }

        public IConsumePipe CreateConsumePipe(params IConsumePipeSpecification[] specifications)
        {
            var builder = new ConsumePipeBuilder();

            Apply(builder);

            for (int i = 0; i < specifications.Length; i++)
                specifications[i].Apply(builder);

            return builder.Build();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(IConsumePipeBuilder builder)
        {
            foreach (IConsumePipeSpecification specification in _specifications)
                specification.Apply(builder);
        }


        class Proxy :
            IConsumePipeSpecification
        {
            readonly IPipeSpecification<ConsumeContext> _specification;

            public Proxy(IPipeSpecification<ConsumeContext> specification)
            {
                _specification = specification;
            }

            public void Apply(IConsumePipeBuilder builder)
            {
                _specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }
        }


        class Proxy<T> :
            IConsumePipeSpecification
            where T : class
        {
            readonly IPipeSpecification<ConsumeContext<T>> _specification;

            public Proxy(IPipeSpecification<ConsumeContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(IConsumePipeBuilder builder)
            {
                _specification.Apply(new BuilderProxy(builder));
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }


            class BuilderProxy :
                IPipeBuilder<ConsumeContext<T>>
            {
                readonly IConsumePipeBuilder _builder;

                public BuilderProxy(IConsumePipeBuilder builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<ConsumeContext<T>> filter)
                {
                    _builder.AddFilter(filter);
                }
            }
        }
    }
}