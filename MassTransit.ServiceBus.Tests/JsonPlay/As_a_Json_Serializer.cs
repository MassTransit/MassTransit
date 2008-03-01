namespace MassTransit.ServiceBus.Tests.JsonPlay
{
    using System.Globalization;
    using System.IO;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class As_a_Json_Serializer
    {
        [Test]
        public void NAME()
        {
            string json = JavaScriptConvert.SerializeObject(new Bob("Chris"));

            Bob clone = JavaScriptConvert.DeserializeObject<Bob>(json);
            Assert.That(clone.Friend, Is.EqualTo("Chris"));
        }

        [Test]
        public void Hmmm()
        {
            string json = JavaScriptConvert.SerializeObject(new Bob("Chris"));

            Bill clone = JavaScriptConvert.DeserializeObject<Bill>(json);
            Assert.That(clone.Friend, Is.EqualTo("Chris"));
        }

        [Test]
        public void More_Control()
        {
            object value = new Bob("dru");

            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
            JsonSerializer jsonSerializer = new JsonSerializer();
            
            using (JsonWriter jsonWriter = new JsonWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonSerializer.Serialize(jsonWriter, value);
            }

            string json = sw.ToString();
            int i = 0;
        }
    }


    public class Bob : IMessage
    {
        private string _friend;

        //for JSON
        public Bob()
        {
        }

        public Bob(string friend)
        {
            _friend = friend;
        }

        public string Friend
        {
            get { return _friend; }
            set { _friend = value; }
        }
    }

    public class Bill : IMessage
    {
        private string _friend;

        //for JSON
        public Bill()
        {
        }

        public Bill(string friend)
        {
            _friend = friend;
        }

        public string Friend
        {
            get { return _friend; }
            set { _friend = value; }
        }
    }
}