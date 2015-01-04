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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;


    /// <summary>
    /// Catches a pipeline exception and determines if the rescue pipe should be passed
    /// control of the context.
    /// </summary>
    /// <typeparam name="T">The filter type</typeparam>
    public class RescueFilter<T> :
        IFilter<T>
        where T : class, PipeContext
    {
        readonly RescueExceptionFilter _exceptionFilter;
        readonly IPipe<T> _rescuePipe;

        public RescueFilter(IPipe<T> rescuePipe, RescueExceptionFilter exceptionFilter)
        {
            _rescuePipe = rescuePipe;
            _exceptionFilter = exceptionFilter;
        }

        async Task IFilter<T>.Send(T context, IPipe<T> next)
        {
            try
            {
                await next.Send(context);

                return;
            }
            catch (AggregateException ex)
            {
                if (!ex.InnerExceptions.Any(x => _exceptionFilter(x)))
                    throw;
            }
            catch (Exception ex)
            {
                if (!_exceptionFilter(ex))
                    throw;
            }

            await _rescuePipe.Send(context);
        }

        bool IFilter<T>.Visit(IPipeVisitor visitor)
        {
            return visitor.Visit(this, x => _rescuePipe.Visit(x));
        }
    }
}