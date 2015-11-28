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
    using PipeBuilders;
    using PipeConfigurators;
    using Pipeline;


    public class SendPipeSpecificationList :
        ISendPipeSpecification
    {
        readonly IList<ISendPipeSpecification> _specifications;

        public SendPipeSpecificationList()
        {
            _specifications = new List<ISendPipeSpecification>();
        }

        public void Apply(ISendPipeBuilder builder)
        {
            foreach (ISendPipeSpecification specification in _specifications)
                specification.Apply(builder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Add(IPipeSpecification<SendContext> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy(specification));
        }

        public void Add<T>(IPipeSpecification<SendContext<T>> specification)
            where T : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy<T>(specification));
        }


        class Proxy :
            ISendPipeSpecification
        {
            readonly IPipeSpecification<SendContext> _specification;

            public Proxy(IPipeSpecification<SendContext> specification)
            {
                _specification = specification;
            }

            public void Apply(ISendPipeBuilder builder)
            {
                _specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }
        }


        class Proxy<T> :
            ISendPipeSpecification
            where T : class
        {
            readonly IPipeSpecification<SendContext<T>> _specification;

            public Proxy(IPipeSpecification<SendContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(ISendPipeBuilder builder)
            {
                _specification.Apply(new BuilderProxy(builder));
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }


            class BuilderProxy :
                IPipeBuilder<SendContext<T>>
            {
                readonly ISendPipeBuilder _builder;

                public BuilderProxy(ISendPipeBuilder builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<SendContext<T>> filter)
                {
                    _builder.AddFilter(filter);
                }
            }
        }
    }
}