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
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using Policies;
    using Util;


    /// <summary>
    /// Rescue catches an exception, and if the exception matches the exception filter,
    /// passes control to the rescue pipe.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class RescueConsumeFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        static readonly ILog _log = Logger.Get<RescueConsumeFilter<T>>();
        readonly IPolicyExceptionFilter _exceptionFilter;
        readonly IPipe<ConsumeContext<T>> _rescuePipe;

        public RescueConsumeFilter(IPipe<ConsumeContext<T>> rescuePipe, IPolicyExceptionFilter exceptionFilter)
        {
            _rescuePipe = rescuePipe;
            _exceptionFilter = exceptionFilter;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("rescue");

            _rescuePipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<T>>.Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            try
            {
                await next.Send(context);
            }
            catch (AggregateException ex)
            {
                if (!ex.InnerExceptions.Any(x => _exceptionFilter.Match(x)))
                    throw;

                if (_log.IsErrorEnabled)
                    _log.Error($"Rescue<{TypeMetadataCache<T>.ShortName}>", ex);

                await _rescuePipe.Send(context);
            }
            catch (Exception ex)
            {
                if (!_exceptionFilter.Match(ex))
                    throw;

                if (_log.IsErrorEnabled)
                    _log.Error($"Rescue<{TypeMetadataCache<T>.ShortName}>", ex);

                await _rescuePipe.Send(context);
            }
        }
    }
}