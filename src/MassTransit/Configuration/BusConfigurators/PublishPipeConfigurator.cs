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
    using System.Threading.Tasks;
    using Builders;
    using Configurators;
    using Context;
    using PipeBuilders;
    using PipeConfigurators;
    using Pipeline;


    public class PublishPipeConfigurator :
        IPublishPipeConfigurator,
        IPublishPipeFactory,
        IPublishPipeSpecification
    {
        readonly IList<IPublishPipeSpecification> _specifications;

        public PublishPipeConfigurator()
        {
            _specifications = new List<IPublishPipeSpecification>();
        }

        public void AddPipeSpecification(IPipeSpecification<PublishContext> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy(specification));
        }

        public void AddPipeSpecification<T>(IPipeSpecification<PublishContext<T>> specification) where T : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new Proxy<T>(specification));
        }

        public void AddPipeSpecification(IPipeSpecification<SendContext> specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new SendProxy(specification));
        }

        public void AddPipeSpecification<T>(IPipeSpecification<SendContext<T>> specification) where T : class
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(new SendProxy<T>(specification));
        }

        public IPublishPipe CreatePublishPipe(params IPublishPipeSpecification[] specifications)
        {
            var builder = new PublishPipeBuilder();

            Apply(builder);

            for (int i = 0; i < specifications.Length; i++)
                specifications[i].Apply(builder);

            return builder.Build();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        public void Apply(IPublishPipeBuilder builder)
        {
            foreach (IPublishPipeSpecification specification in _specifications)
                specification.Apply(builder);
        }


        class Proxy :
            IPublishPipeSpecification
        {
            readonly IPipeSpecification<PublishContext> _specification;

            public Proxy(IPipeSpecification<PublishContext> specification)
            {
                _specification = specification;
            }

            public void Apply(IPublishPipeBuilder builder)
            {
                _specification.Apply(builder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }
        }


        class SendProxy :
            IPublishPipeSpecification
        {
            readonly IPipeSpecification<SendContext> _specification;

            public SendProxy(IPipeSpecification<SendContext> specification)
            {
                _specification = specification;
            }

            public void Apply(IPublishPipeBuilder builder)
            {
                _specification.Apply(new BuilderProxy(builder));
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }


            class BuilderProxy :
                IPipeBuilder<SendContext>
            {
                readonly IPublishPipeBuilder _builder;

                public BuilderProxy(IPublishPipeBuilder builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<SendContext> filter)
                {
                    _builder.AddFilter(new Adapter(filter));
                }


                class Adapter :
                    IFilter<PublishContext>
                {
                    readonly IFilter<SendContext> _filter;

                    public Adapter(IFilter<SendContext> filter)
                    {
                        _filter = filter;
                    }

                    public Task Send(PublishContext context, IPipe<PublishContext> next)
                    {
                        var nextAdapter = new Next(next, context);

                        return _filter.Send(context, nextAdapter);
                    }

                    public void Probe(ProbeContext context)
                    {
                        _filter.Probe(context);
                    }


                    class Next :
                        IPipe<SendContext>
                    {
                        readonly PublishContext _context;
                        readonly IPipe<PublishContext> _next;

                        public Next(IPipe<PublishContext> next, PublishContext context)
                        {
                            _next = next;
                            _context = context;
                        }

                        public Task Send(SendContext context)
                        {
                            var proxy = new PublishContextProxy(context);
                            return _next.Send(proxy);
                        }

                        public void Probe(ProbeContext context)
                        {
                            _next.Probe(context);
                        }
                    }
                }
            }
        }


        class SendProxy<T> :
            IPublishPipeSpecification
            where T : class
        {
            readonly IPipeSpecification<SendContext<T>> _specification;

            public SendProxy(IPipeSpecification<SendContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPublishPipeBuilder builder)
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
                readonly IPublishPipeBuilder _builder;

                public BuilderProxy(IPublishPipeBuilder builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<SendContext<T>> filter)
                {
                    _builder.AddFilter(new Adapter(filter));
                }


                class Adapter :
                    IFilter<PublishContext<T>>
                {
                    readonly IFilter<SendContext<T>> _filter;

                    public Adapter(IFilter<SendContext<T>> filter)
                    {
                        _filter = filter;
                    }

                    public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
                    {
                        var nextAdapter = new Next(next, context);

                        return _filter.Send(context, nextAdapter);
                    }

                    public void Probe(ProbeContext context)
                    {
                        _filter.Probe(context);
                    }


                    class Next :
                        IPipe<SendContext<T>>
                    {
                        readonly PublishContext<T> _context;
                        readonly IPipe<PublishContext<T>> _next;

                        public Next(IPipe<PublishContext<T>> next, PublishContext<T> context)
                        {
                            _next = next;
                            _context = context;
                        }

                        public Task Send(SendContext<T> context)
                        {
                            var proxy = new PublishContextProxy<T>(context, context.Message);

                            return _next.Send(proxy);
                        }

                        public void Probe(ProbeContext context)
                        {
                            _next.Probe(context);
                        }
                    }
                }
            }
        }


        class Proxy<T> :
            IPublishPipeSpecification
            where T : class
        {
            readonly IPipeSpecification<PublishContext<T>> _specification;

            public Proxy(IPipeSpecification<PublishContext<T>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPublishPipeBuilder builder)
            {
                _specification.Apply(new BuilderProxy(builder));
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }


            class BuilderProxy :
                IPipeBuilder<PublishContext<T>>
            {
                readonly IPublishPipeBuilder _builder;

                public BuilderProxy(IPublishPipeBuilder builder)
                {
                    _builder = builder;
                }

                public void AddFilter(IFilter<PublishContext<T>> filter)
                {
                    _builder.AddFilter(filter);
                }
            }
        }
    }
}