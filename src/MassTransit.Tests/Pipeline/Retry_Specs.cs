namespace MassTransit.Tests.Pipeline
{
    using System;
    using Context.Converters;
    using GreenPipes;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Using_the_retry_filter
    {
        class A
        {
        }


        [Test]
        public void Should_retry_the_specified_times_and_fail()
        {
            int count = 0;
            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestConsumeContext<A>(new A());

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            count.ShouldBe(5);
        }

        [Test]
        public void Should_support_overloading_downstream()
        {
            int count = 0;
            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseRetry(r => r.None());
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestConsumeContext<A>(new A());

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            count.ShouldBe(1);
        }

        [Test]
        public void Should_support_overloading_downstream_on_cc()
        {
            int count = 0;
            IPipe<ConsumeContext> pipe = Pipe.New<ConsumeContext>(x =>
            {
                x.UseRetry(r =>
                {
                    r.Handle<IntentionalTestException>();
                    r.Interval(4, TimeSpan.FromMilliseconds(2));
                });
                x.UseRetry(r =>
                {
                    r.Handle<IntentionalTestException>();
                    r.None();
                });
                x.UseDispatch(new ConsumeContextConverterFactory(), d =>
                {
                    d.Pipe<ConsumeContext<A>>(a =>
                    {
                        a.UseExecute(payload =>
                        {
                            count++;
                            throw new IntentionalTestException("Kaboom!");
                        });

                    });
                });
            });

            var context = new TestConsumeContext<A>(new A());

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            count.ShouldBe(1);
        }

        [Test]
        public void Should_support_overloading_downstream_either_way()
        {
            int count = 0;
            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.UseRetry(r => r.None());
                x.UseRetry(r => r.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.UseExecute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestConsumeContext<A>(new A());

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<IntentionalTestException>());

            count.ShouldBe(5);
        }
    }
}
