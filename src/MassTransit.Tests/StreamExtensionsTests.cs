namespace MassTransit.Tests
{
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using Magnum.Extensions;
    using Magnum.TestFramework;

    [TestFixture]
    public class StreamExtensionsTests
    {
        [Test]
public void Really()
        {
            var testData = new string('a', 5000);
            var testBuff = Encoding.UTF8.GetBytes(testData);
            using(var source = new MemoryStream())
            {
                source.Write(testBuff,0,testBuff.Length );
                using(var dest = new MemoryStream())
                {
                    source.CopyTo(dest);
                    source.ShouldEqual(dest);
                }
            }
        }
    }
}