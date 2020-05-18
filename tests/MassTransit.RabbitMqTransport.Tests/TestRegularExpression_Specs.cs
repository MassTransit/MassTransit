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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Text.RegularExpressions;
    using NUnit.Framework;


    [TestFixture]
    public class TestRegularExpression_Specs
    {
        [Test]
        public void Verify_regex()
        {
            const string stackTrack =
                @"   at MassTransit.RabbitMqTransport.Tests.A_serialization_exception.<>c.<<ConfigureInputQueueEndpoint>b__13_0>d.MoveNext() in E:\Home\MassTransit\src\MassTransit.RabbitMqTransport.Tests\ErrorQueue_Specs.cs:line 129
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at MassTransit.TestFramework.BusTestFixture.<>c__DisplayClass6_0`1.<<Handler>b__0>d.MoveNext() in E:\Home\MassTransit\src\MassTransit.TestFramework\BusTestFixture.cs:line 143
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at MassTransit.Pipeline.Filters.HandlerMessageFilter`1.<MassTransit-Pipeline-IFilter<MassTransit-ConsumeContext<TMessage>>-Send>d__5.MoveNext() in E:\Home\MassTransit\src\MassTransit\Pipeline\Filters\HandlerMessageFilter.cs:line 56
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at MassTransit.Pipeline.Filters.HandlerMessageFilter`1.<MassTransit-Pipeline-IFilter<MassTransit-ConsumeContext<TMessage>>-Send>d__5.MoveNext() in E:\Home\MassTransit\src\MassTransit\Pipeline\Filters\HandlerMessageFilter.cs:line 69
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at MassTransit.Pipeline.Filters.TeeConsumeFilter`1.<>c__DisplayClass7_0.<<Send>b__0>d.MoveNext() in E:\Home\MassTransit\src\MassTransit\Pipeline\Filters\TeeConsumeFilter.cs:line 59
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at MassTransit.Pipeline.Filters.TeeConsumeFilter`1.<Send>d__7.MoveNext() in E:\Home\MassTransit\src\MassTransit\Pipeline\Filters\TeeConsumeFilter.cs:line 59
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at MassTransit.Pipeline.Filters.MessageConsumeFilter`1.<MassTransit-Pipeline-IFilter<MassTransit-ConsumeContext>-Send>d__7.MoveNext() in E:\Home\MassTransit\src\MassTransit\Pipeline\Filters\MessageConsumeFilter.cs:line 80
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at MassTransit.Pipeline.Filters.MessageConsumeFilter`1.<MassTransit-Pipeline-IFilter<MassTransit-ConsumeContext>-Send>d__7.MoveNext() in E:\Home\MassTransit\src\MassTransit\Pipeline\Filters\MessageConsumeFilter.cs:line 95
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at MassTransit.Pipeline.Filters.DeserializeFilter.<Send>d__4.MoveNext() in E:\Home\MassTransit\src\MassTransit\Pipeline\Filters\DeserializeFilter.cs:line 48
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at MassTransit.Pipeline.Filters.RescueReceiveContextFilter`1.<MassTransit-Pipeline-IFilter<MassTransit-ReceiveContext>-Send>d__5.MoveNext() in E:\Home\MassTransit\src\MassTransit\Pipeline\Filters\RescueReceiveContextFilter.cs:line 55";

            var cleanup =
                new Regex(
                    @"--- End of stack trace.* ---.*\n\s+(at System\.Runtime\.CompilerServices\.TaskAwaiter.*\s*|at System\.Runtime\.ExceptionServices\.ExceptionDispatchInfo.*\s*)+",
                    RegexOptions.Multiline | RegexOptions.Compiled);

            var result = cleanup.Replace(stackTrack, "");
        }
    }
}