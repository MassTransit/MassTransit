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
    using System.Runtime.Serialization;
    using System.Threading.Tasks;


//    public class FaultFilter :
//        IFilter<ConsumeContext>
//
//    {
//        async Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
//        {
//            try
//            {
//                await next.Send(context);
//            }
//            catch (SerializationException ex)
//            {
//                context
//                throw;
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }
//        }
//
//        bool IFilter<ReceiveContext>.Inspect(IPipeInspector inspector)
//        {
//            return inspector.Inspect(this);
//        }
//    }
}