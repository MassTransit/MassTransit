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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Configurators;
    using PipeBuilders;
    using PipeConfigurators;
    using Pipeline.Filters;


    public class LogConsume :
        IPipeConfigurable<ConsumeContext>
    {
        Func<ConsumeContext, Task<string>> _formatter;
        TextWriter _writer;

        public LogConsume()
        {
            _writer = Console.Out;
            _formatter = async x => string.Format("Message Id: {0}", x.ReceiveContext.TransportHeaders.Get("MessageId", "N/A"));
        }

        void IPipeBuilderConfigurator<ConsumeContext>.Build(IPipeBuilder<ConsumeContext> builder)
        {
            builder.AddFilter(new LogFilter<ConsumeContext>(_writer, _formatter));
        }

        IEnumerable<ValidationResult> Configurator.Validate()
        {
            if (_formatter == null)
                yield return this.Failure("Formatter", "must not be null");
            if (_writer == null)
                yield return this.Failure("Writer", "must not be null");
        }

        public void Format(Func<ConsumeContext, string> formatter)
        {
            _formatter = async x => formatter(x);
        }

        public void Format(Func<ConsumeContext, Task<string>> formatter)
        {
            _formatter = formatter;
        }

        public void Writer(TextWriter writer)
        {
            _writer = writer;
        }
    }
}