namespace MassTransit.Transports.RabbitMq.Tests
{
    using System;
    using Magnum.TestFramework;

    [Scenario]
    public class GivenAVHostAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/queue/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.ParseForInbound(Uri);
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
            _addr.Queue.ShouldEqual("/the_queue");
        }

        [Then]
        public void Rebuilt()
        {
            _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://guest:guest@some_server:5432/thehost/queue/the_queue"));
        }

        [Then]
        public void ShouldBeAQueue()
        {
            _addr.AddressType.ShouldEqual(AddressType.Queue);
        }
    }

    [Scenario]
    public class GivenANonVHostAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/queue/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.ParseForInbound(Uri);
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
            _addr.Queue.ShouldEqual("/the_queue");
        }
        [Then]
        public void ShouldBeAQueue()
        {
            _addr.AddressType.ShouldEqual(AddressType.Queue);
        }

    }

    [Scenario]
    public class GivenAPortedAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server:12/queue/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.ParseForInbound(Uri);
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
            _addr.RebuiltUri.ShouldEqual(new Uri(@"rabbitmq://guest:guest@some_server:12/queue/the_queue"));
        }
        [Then]
        public void ShouldBeAQueue()
        {
            _addr.AddressType.ShouldEqual(AddressType.Queue);
        }
    }

    [Scenario]
    public class GivenANonPortedAddress
    {
        public Uri Uri = new Uri("rabbitmq://some_server/queue/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.ParseForInbound(Uri);
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
        [Then]
        public void ShouldBeAQueue()
        {
            _addr.AddressType.ShouldEqual(AddressType.Queue);
        }
    }


    [Scenario]
    public class GivenAEmptyUserNameUrl
    {
        public Uri Uri = new Uri("rabbitmq://some_server/thehost/queue/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.ParseForInbound(Uri);
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
        [Then]
        public void ShouldBeAQueue()
        {
            _addr.AddressType.ShouldEqual(AddressType.Queue);
        }
    }
    

    [Scenario]
    public class GivenAUserNameUrl
    {
        public Uri Uri = new Uri("rabbitmq://dru:mt@some_server/thehost/queue/the_queue");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.ParseForInbound(Uri);
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
            _addr.RebuiltUri.ShouldEqual(new Uri("rabbitmq://dru:mt@some_server:5432/thehost/queue/the_queue"));
        }

        [Then]
        public void ShouldBeAQueue()
        {
            _addr.AddressType.ShouldEqual(AddressType.Queue);
        }
    }
    [Scenario]
    public class GivenAnExchangeAddressWithHost
    {
        Uri uri  = new Uri("rabbitmq://dru:mt@some_server/vhost/exchange/urn:message:masstransit.ping");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.ParseForOutbound(uri);
        }

        [Then]
        public void ShouldBeAnExchange()
        {
            _addr.AddressType.ShouldEqual(AddressType.Exchange);
        }
    }
    [Scenario]
    public class GivenAnExchangeAddress
    {
        Uri uri  = new Uri("rabbitmq://dru:mt@some_server/exchange/urn:message:masstransit.ping");
        RabbitMqAddress _addr;

        [When]
        public void WhenParsed()
        {
            _addr = RabbitMqAddress.ParseForOutbound(uri);
        }

        [Then]
        public void ShouldBeAnExchange()
        {
            _addr.AddressType.ShouldEqual(AddressType.Exchange);
        }
    }
}