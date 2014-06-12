// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline
{
    using System;
    using System.Collections.Generic;
    using Magnum.Concurrency;


    /// <summary>
    /// An inbound message pipeline takes a consume context and maps it
    /// to a number if actual consumers, in its <see cref="Enumerate"/> method. It
    /// is also a place-holder for configuration regarding the consume context/its mapped
    /// outputs.
    /// </summary>
    public class InboundMessagePipeline :
        IInboundMessagePipeline
    {
        readonly IInboundPipelineConfigurator _configurator;
        readonly Atomic<IPipelineSink<IConsumeContext>> _output;

        public InboundMessagePipeline(IPipelineSink<IConsumeContext> output, IInboundPipelineConfigurator configurator)
        {
            _output = Atomic.Create(output);
            _configurator = configurator;
        }

        public IEnumerable<Action<IConsumeContext>> Enumerate(IConsumeContext context)
        {
            return _output.Value.Enumerate(context);
        }

        public bool Inspect(IPipelineInspector inspector)
        {
            return inspector.Inspect(this, () => _output.Value.Inspect(inspector));
        }

        public void Configure(Action<IInboundPipelineConfigurator> configureCallback)
        {
            configureCallback(_configurator);
        }

        public TResult Configure<TResult>(Func<IInboundPipelineConfigurator, TResult> configureCallback)
        {
            return configureCallback(_configurator);
        }

        /// <summary>
        /// Atomically replaces the current output sink with the argument.
        /// </summary>
        /// <param name="sink">The argument sink.</param>
        /// <returns>The passed argument when the replace operation is done.</returns>
        public IPipelineSink<IConsumeContext> ReplaceOutputSink(IPipelineSink<IConsumeContext> sink)
        {
            return _output.Set(output => sink);
        }
    }
}