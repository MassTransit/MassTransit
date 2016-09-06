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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using GreenPipes;


    public class LogFilter<T> :
        IFilter<T>
        where T : class, PipeContext
    {
        readonly LogFormatter<T> _formatter;
        readonly TextWriter _writer;

        public LogFilter(TextWriter writer, LogFormatter<T> formatter)
        {
            _writer = writer;
            _formatter = formatter;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("log");
        }

        [DebuggerNonUserCode]
        async Task IFilter<T>.Send(T context, IPipe<T> next)
        {
            DateTime startTime = DateTime.UtcNow;
            Stopwatch timer = Stopwatch.StartNew();

            await next.Send(context).ConfigureAwait(false);

            timer.Stop();

            var logContext = new LogContext(startTime, timer.Elapsed);

            string text = await _formatter(context, logContext).ConfigureAwait(false);

            await _writer.WriteLineAsync(text).ConfigureAwait(false);
        }
    }
}