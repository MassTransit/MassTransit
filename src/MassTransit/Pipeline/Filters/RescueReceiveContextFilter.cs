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
    using Context;
    using Logging;
    using Policies;


    /// <summary>
    /// Catches a pipeline exception and determines if the rescue pipe should be passed
    /// control of the context.
    /// </summary>
    /// <typeparam name="T">The filter type</typeparam>
    public class RescueReceiveContextFilter<T> :
        IFilter<ReceiveContext>
        where T : class, PipeContext
    {
        readonly IPolicyExceptionFilter _exceptionFilter;
        readonly ILog _log = Logger.Get<RescueReceiveContextFilter<T>>();
        readonly IPipe<ExceptionReceiveContext> _rescuePipe;

        public RescueReceiveContextFilter(IPipe<ExceptionReceiveContext> rescuePipe, IPolicyExceptionFilter exceptionFilter)
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
        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            Exception exception = null;
            try
            {
                await next.Send(context);
            }
            catch (AggregateException ex)
            {
                if (!ex.InnerExceptions.Any(x => _exceptionFilter.Match(x)))
                    throw;

                exception = ex;

                if (_log.IsErrorEnabled)
                    _log.Error("Rescuing exception", ex);
            }
            catch (Exception ex)
            {
                if (!_exceptionFilter.Match(ex))
                    throw;

                exception = ex;

                if (_log.IsErrorEnabled)
                    _log.Error("Rescuing exception", ex);
            }

            if (exception != null)
            {
                var exceptionContext = new RescueExceptionReceiveContext(context, exception);

                await _rescuePipe.Send(exceptionContext);
            }
        }
    }
}