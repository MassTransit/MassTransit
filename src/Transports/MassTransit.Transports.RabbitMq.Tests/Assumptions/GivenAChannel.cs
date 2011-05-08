namespace MassTransit.Transports.RabbitMq.Tests.Assumptions
{
    using System;
    using System.Diagnostics;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Magnum.Extensions;

    public class GivenAChannel
    {
        private IConnection _connection;
        protected readonly byte[] TheMessage = new byte[]{1,2,3};

        [Given]
        public void BuildUpAConnection()
        {
            var cf = new ConnectionFactory();
            cf.UserName = "guest";
            cf.Password = "guest";
            cf.Port = 5672;
            cf.VirtualHost = "/";
            cf.HostName = "localhost";

            _connection = cf.CreateConnection();
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        public void WithStopWatch(string name, Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            Trace.WriteLine("'{0}' took '{1}' seconds".FormatWith(name, sw.Elapsed.TotalSeconds));
        }
        public void WithChannel(Action<IModel> action)
        {
            IModel model=null;
            try
            {
                model = _connection.CreateModel();
                action(model);
            }
            finally
            {
                if(model != null)
                {
                    model.Dispose();
                    
                }
            }
        }
    }
}