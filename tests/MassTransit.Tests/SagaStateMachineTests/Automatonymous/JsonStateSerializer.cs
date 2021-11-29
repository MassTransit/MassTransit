namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;


    public class JsonStateSerializer<TStateMachine, TInstance>
        where TStateMachine : StateMachine<TInstance>
        where TInstance : class, ISaga
    {
        readonly TStateMachine _machine;

        JsonSerializer _deserializer;

        JsonSerializer _serializer;

        public JsonStateSerializer(TStateMachine machine)
        {
            _machine = machine;
        }

        public JsonSerializer Deserializer
        {
            get
            {
                return _deserializer ??= JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    Converters = new List<JsonConverter>(new JsonConverter[]
                    {
                        new StateConverter<TStateMachine>(_machine),
                    })
                });
            }
        }

        public JsonSerializer Serializer
        {
            get
            {
                return _serializer ??= JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    Converters = new List<JsonConverter>(new JsonConverter[]
                    {
                        new StateConverter<TStateMachine>(_machine),
                    }),
                });
            }
        }

        public string Serialize<T>(T instance)
            where T : TInstance
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, instance);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }


        public void Serialize<T>(Stream output, T instance)
            where T : TInstance
        {
            using (var writer = new StreamWriter(output))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;

                Serializer.Serialize(jsonWriter, instance);

                jsonWriter.Flush();
                writer.Flush();
            }
        }

        public T Deserialize<T>(string body)
            where T : TInstance
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(body)))
            {
                return Deserialize<T>(ms);
            }
        }

        public T Deserialize<T>(Stream input)
            where T : TInstance

        {
            using (var reader = new StreamReader(input))
            using (var jsonReader = new JsonTextReader(reader))
                return Deserializer.Deserialize<T>(jsonReader);
        }
    }
}
