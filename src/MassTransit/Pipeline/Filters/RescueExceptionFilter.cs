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


    /// <summary>
    /// Given the exception, should determine if the exception should cause
    /// the pipe to follow the rescue path instead of throwing the exception
    /// back up the pipeline.
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>True if the context should be sent through the rescue pipe, otherwise false.</returns>
    public delegate bool RescueExceptionFilter(Exception exception);
}