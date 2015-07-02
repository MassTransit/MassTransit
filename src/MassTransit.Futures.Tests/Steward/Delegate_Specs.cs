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
namespace MassTransit.Tests.Steward
{
    using MassTransit.Steward.Core.Policies;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_command_delegate_is_used
    {
        [Test]
        public void Should_return_accepted_when_allowed()
        {
            var dispatchPolicy = new DelegateMessageDispatchPolicy<TestCommand>(x => x.Message.Accept
                ? x.Accept()
                : x.Reject("by your command"));
        }


        class TestCommand
        {
            public bool Accept { get; set; }
        }
    }
}