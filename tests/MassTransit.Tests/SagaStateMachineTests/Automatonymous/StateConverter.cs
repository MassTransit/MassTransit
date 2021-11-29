namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;


    public class StateConverter<T> :
        JsonConverter
        where T : StateMachine
    {
        readonly T _machine;

        public StateConverter(T machine)
        {
            _machine = machine;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var state = (State)value;
            string text = state.Name;
            if (string.IsNullOrEmpty(text))
                text = "";

            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default(State);

            if (reader.TokenType == JsonToken.String)
            {
                var text = (string)reader.Value;
                if (string.IsNullOrWhiteSpace(text))
                    return default(State);

                return _machine.GetState((string)reader.Value);
            }

            throw new JsonReaderException(string.Format(CultureInfo.InvariantCulture,
                "Error reading State. Expected a string but got {0}.", new object[] {reader.TokenType}));
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(State).IsAssignableFrom(objectType);
        }
    }
}
