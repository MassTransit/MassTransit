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
    using System.IO;
    using System.Threading.Tasks;
    using PipeConfigurators;


    public static class LogPipeConfiguratorExtensions
    {
        public static void Log<T>(this IPipeConfigurator<T> configurator, TextWriter textWriter, Func<T, Task<string>> formatter)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            var pipeBuilderConfigurator = new LogPipeBuilderConfigurator<T>(textWriter, formatter);

            configurator.AddPipeBuilderConfigurator(pipeBuilderConfigurator);
        }
    }
}