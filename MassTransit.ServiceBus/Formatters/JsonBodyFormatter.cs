namespace MassTransit.ServiceBus.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Exceptions;
    using Newtonsoft.Json;

    public class JsonBodyFormatter :
        IBodyFormatter
    {
        public void Serialize(IFormattedBody body, object message)
        {
            JsonWrapper jw = JsonWrapper.Create(message);
            string wrappedJson = JavaScriptConvert.SerializeObject(jw);

			MemoryStream mstream = new MemoryStream(Encoding.UTF8.GetBytes(wrappedJson));

        	mstream.WriteTo(body.BodyStream);
        }

        public T Deserialize<T>(IFormattedBody formattedBody) where T : class
        {
        		string body;
			using (StreamReader reader = new StreamReader(formattedBody.BodyStream))
			{
				body = reader.ReadToEnd();
			}

            JsonWrapper jw = JavaScriptConvert.DeserializeObject<JsonWrapper>(body);

            Type desiredType = Type.GetType(jw.Types[0], true, true);
            object o = JavaScriptConvert.DeserializeObject(jw.WrappedJson, desiredType);

            //object obj JavaScriptConvert.DeserializeObject<T>(jw.WrappedJson);

			if(typeof(T).IsAssignableFrom(o.GetType()))
				return (T) o;

			throw new JsonSerializationException("Unable to convert the message to the requested type");
        }

        public object Deserialize(IFormattedBody formattedBody)
        {
			string body;
			using (StreamReader reader = new StreamReader(formattedBody.BodyStream))
			{
				body = reader.ReadToEnd();
			}

            JsonWrapper jw = JavaScriptConvert.DeserializeObject<JsonWrapper>(body);

            Type desiredType = Type.GetType(jw.Types[0], true, true);
            object o = JavaScriptConvert.DeserializeObject(jw.WrappedJson, desiredType);
            //search through types to find a match?
            return o;
        }
    }

    public class JsonWrapper
    {
        private string _wrappedJson;
        private string[] _types;

        public JsonWrapper()
        {
        }

        public JsonWrapper(string wrappedJson, string[] types)
        {
            _wrappedJson = wrappedJson;
            _types = types;
        }

        public string WrappedJson
        {
            get { return _wrappedJson; }
            set { _wrappedJson = value; }
        }

        public string[] Types
        {
            get { return _types; }
            set { _types = value; }
        }

        public static JsonWrapper Create(object message)
        {
            List<string> types = new List<string>();
            Type temp = message.GetType();
            while (temp != typeof(object))
            {
                types.Add(string.Format("{0}, {1}",temp.FullName, temp.Assembly.GetName().Name));
                temp = temp.BaseType;
            }

            return new JsonWrapper(JavaScriptConvert.SerializeObject(message), types.ToArray());
        }
    }
}