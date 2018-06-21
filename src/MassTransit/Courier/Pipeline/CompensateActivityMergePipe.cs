// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    /// <summary>
    /// Merges the out-of-band consumer back into the pipe
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TLog"></typeparam>
    public class CompensateActivityMergePipe<TActivity, TLog> :
        IPipe<CompensateActivityContext<TLog>>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly IPipe<CompensateActivityContext<TActivity, TLog>> _output;

        public CompensateActivityMergePipe(IPipe<CompensateActivityContext<TActivity, TLog>> output)
        {
            _output = output;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Set(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                LogType = TypeMetadataCache<TLog>.ShortName
            });

            _output.Probe(scope);
        }

        public Task Send(CompensateActivityContext<TLog> context)
        {
            return _output.Send(context.PopContext<TActivity>());
        }
    }
}