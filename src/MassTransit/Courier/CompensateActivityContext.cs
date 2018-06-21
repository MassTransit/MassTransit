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
namespace MassTransit.Courier
{
    public interface CompensateActivityContext<TLog> :
        CompensateContext<TLog>
        where TLog : class
    {
        /// <summary>
        /// Return the original consumer/message combined context, reapplying the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        CompensateActivityContext<T, TLog> PopContext<T>()
            where T : class, CompensateActivity<TLog>;
    }


    public interface CompensateActivityContext<out TActivity, TLog> :
        CompensateActivityContext<TLog>
        where TLog : class
        where TActivity : class, CompensateActivity<TLog>
    {
        /// <summary>
        /// The activity that was created/used for this compensation
        /// </summary>
         TActivity Activity { get; }
    }
}