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
namespace MassTransit.Hosting
{
    /// <summary>
    /// A queue name provider for Courier activities, built based on the activity name
    /// </summary>
    public interface IActivityQueueNameProvider
    {
        /// <summary>
        /// Returns the queue name for execution 
        /// </summary>
        /// <param name="activityName">The activity name</param>
        /// <returns>The name of the queue</returns>
        string GetExecuteActivityQueueName(string activityName);

        /// <summary>
        /// Returns the queue name for compensation
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        string GetCompensateActivityQueueName(string activityName);
    }
}