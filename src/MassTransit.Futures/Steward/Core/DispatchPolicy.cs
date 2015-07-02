// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Steward.Core
{
    /// <summary>
    /// A dispatch policy is used to determine the availability of a command or request
    /// based on resource availability, etc.
    /// </summary>
    public interface DispatchPolicy
    {
        /// <summary>
        /// Determine if given the dispatch context if the dispatch is allowed, etc.
        /// </summary>
        /// <param name="dispatchContext"></param>
        /// <returns></returns>
        DispatchResult Check(DispatchContext dispatchContext);
    }
}