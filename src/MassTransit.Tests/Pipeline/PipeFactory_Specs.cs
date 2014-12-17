namespace MassTransit.Tests.Pipeline
{
    using NUnit.Framework;


    [TestFixture]
    public class PipeFactory_Specs
    {
        [Test]
        public void FirstTestName()
        {
            var pipe = Pipe.New<ConsumeContext>(x =>
            {
                x.Use<LogConsume>();
            });
        }
    }
}
