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
namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class AddressActivity :
        Activity<AddressArguments, AddressLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<AddressArguments> context)
        {
            Console.WriteLine("Address: {0}", context.Arguments.Address);

            return context.Completed(new Log(context.Arguments.Address));
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AddressLog> context)
        {
            return context.Compensated();
        }


        class Log :
            AddressLog
        {
            public Log(Uri usedAddress)
            {
                UsedAddress = usedAddress;
            }

            public Uri UsedAddress { get; private set; }
        }
    }
}