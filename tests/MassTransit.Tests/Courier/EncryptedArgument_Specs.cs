namespace MassTransit.Tests.Courier
{
    namespace EncryptedArgument_Specs
    {
        using System;
        using System.Security.Cryptography;
        using System.Text;
        using MassTransit.Courier;
        using NUnit.Framework;


        [TestFixture]
        public class Encrypting_an_argument
        {
            [Test]
            public void Should_fail_if_not_supported()
            {
                using (var provider = Aes.Create())
                {
                    provider.GenerateKey();
                    provider.GenerateIV();

                    using (var encryptor = provider.CreateEncryptor())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes("secret");

                        byte[] password = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

                        var builder = new RoutingSlipBuilder(NewId.NextGuid());
                        builder.AddActivity("GetFile", new Uri("rabbitmq://localhost/execute_getfile"), new
                        {
                            Username = "joe",
                            Password = password
                        });

                        var routingSlip = builder.Build();

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
