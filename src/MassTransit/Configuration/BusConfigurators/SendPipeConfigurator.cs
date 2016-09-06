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
    using GreenPipes;
    using PipeConfigurators;
    using Pipeline;


    public class SendPipeConfigurator :
        ISendPipeConfigurator,
        ISendPipeFactory,
        ISendPipeSpecification
    {
        readonly IList<ISendPipeSpecification> _specifications;

        public SendPipeConfigurator()
        {
            _specifications = new List<ISendPipeSpecification>();
        }

        public void AddPipeSpecification(IPipeSpecification<SendContext> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy(specification));
        }

        public void AddPipeSpecification<T>(IPipeSpecification<SendContext<T>> specification) where T : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy<T>(specification));
        }

        public ISendPipe CreateSendPipe(params ISendPipeSpecification[] specifications)
        {
            var builder = new SendPipeBuilder();

            Apply(builder);

            for (int i = 0; i < specifications.Length; i++)
                specifications[i].Apply(builder);

            return builder.Build();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(ISendPipeBuilder builder)
        {
            foreach (ISendPipeSpecification specification in _specifications)
                specification.Apply(builder);
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