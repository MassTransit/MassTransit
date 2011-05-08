namespace MassTransit.Transports.RabbitMq.Tests
{
    using System;
    using Magnum.TestFramework;
    using NUnit.Framework;

    [Scenario]
    public class GivenAVHostAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            var host = _addr.VHost;
            host.ShouldEqual("thehost");
        }

        [Then]
        public void TheQueue()
        {
            _addr.Path.ShouldEqual("queue");
        }

        [Then]
        public void Rebuilt()
        {
            _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/queue"));
        }
    }

    [Scenario]
    public class GivenAnAddressWithUnderscore
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheQueue()
        {
            _addr.Path.ShouldEqual("the_queue");
        }

        [Then]
        public void Rebuilt()
        {
            _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/the_queue"));
        }
    }

    [Scenario]
    public class GivenAnAddressWithPeriod
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the.queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheQueue()
        {
            _addr.Path.ShouldEqual("the.queue");
        }

        [Then]
        public void Rebuilt()
        {
            _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/the.queue"));
        }
    }
    [Scenario]
    public class GivenAnAddressWithColon
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the:queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheQueue()
        {
            _addr.Path.ShouldEqual("the:queue");
        }

        [Then]
        public void Rebuilt()
        {
            _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/the:queue"));
        }
    }


    [Scenario]
    public class GivenAnAddressWithSlash
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the/queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
        }

        [Then][ExpectedException(typeof(Exception))]
        public void TheQueue()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }
    }

    [Scenario]
    public class GivenANonVHostAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            var host = _addr.VHost;
            host.ShouldEqual("/");
        }

        [Then]
        public void TheQueue()
        {
            _addr.Path.ShouldEqual("the_queue");
        }
    }

    [Scenario]
    public class GivenAPortedAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server:12/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            var host = _addr.VHost;
            host.ShouldEqual("/");
        }

        [Then]
        public void ThePort()
        {
            _addr.Port.ShouldEqual(12);
        }

        [Then]
        public void Rebuilt()
        {
            _addr.RebuiltUri.ShouldEqual(new Uri(@"rabbitmq://guest:guest@some_server:12/the_queue"));
        }
    }

    [Scenario]
    public class GivenANonPortedAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheHost()
        {
            var host = _addr.VHost;
            host.ShouldEqual("/");
        }

        [Then]
        public void ThePort()
        {
            _addr.Port.ShouldEqual(5432);
        }
    }


    [Scenario]
    public class GivenAEmptyUserNameUrl
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheUsername()
        {
            _addr.Username.ShouldEqual("guest");
        }

        [Then]
        public void ThePassword()
        {
            _addr.Password.ShouldEqual("guest");
        }
    }
    

    [Scenario]
    public class GivenAUserNameUrl
    {
        public Uri Uri = new Uri("rabbitmq://dru:mt@some_server/thehost/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.Parse(Uri);
        }

        [Then]
        public void TheUsername()
        {
            _addr.Username.ShouldEqual("dru");
        }

        [Then]
        public void ThePassword()
        {
            _addr.Password.ShouldEqual("mt");
        }

        [Then]
        public void Rebuilt()
        {
            _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://dru:mt@some_server:5432/thehost/the_queue"));
        }
    }
}