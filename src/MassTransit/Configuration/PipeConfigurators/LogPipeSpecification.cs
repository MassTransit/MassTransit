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
    using System.Collections.Generic;
    using System.IO;
    using Configurators;
    using GreenPipes;
    using Pipeline.Filters;


    public class LogPipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly LogFormatter<T> _formatter;
        readonly TextWriter _writer;

        public LogPipeSpecification(TextWriter writer, LogFormatter<T> formatter)
        {
            _writer = writer;
            _formatter = formatter;
        }

        void IPipeSpecification<T>.Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new LogFilter<T>(_writer, _formatter));
        }

        IEnumerable<ValidationResult> Configurator.Validate()
        {
            if (_writer == null)
                yield return this.Failure("TextWriter", "must not be null");
            if (_formatter == null)
                yield return this.Failure("Formatter", "must not be null");
        }
    }
}