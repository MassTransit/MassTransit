// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier.Tests
{
    namespace EncryptedArgument_Specs
    {
        using System;
        using System.Security.Cryptography;
        using System.Text;
        using Contracts;
        using NUnit.Framework;


        [TestFixture]
        public class Encrypting_an_argument
        {
            [Test]
            public void Should_fail_if_not_supported()
            {
                using (var provider = new AesCryptoServiceProvider())
                {
                    provider.GenerateKey();
                    provider.GenerateIV();

                    using (ICryptoTransform encryptor = provider.CreateEncryptor())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes("secret");

                        byte[] password = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

                        var builder = new RoutingSlipBuilder(NewId.NextGuid());
                        builder.AddActivity("GetFile", new Uri("rabbitmq://localhost/execute_getfile"), new
                        {
                            Username = "joe",
                            Password = password,
                        });

                        RoutingSlip routingSlip = builder.Build();

                        Console.WriteLine(routingSlip.ToJsonString());
                    }
                }
            }
        }


        public interface TestArguments
        {
            string Username { get; }
            string Password { get; }
        }


        class TestArgumentsArgumentMap :
            ArgumentMap<TestArguments>
        {
            public TestArgumentsArgumentMap(byte[] key, byte[] iv)
            {
                Map(x => x.Password, x => x.Encrypted(key, iv));
            }
        }
    }
}