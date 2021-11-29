namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;


    public class JsonContractResolver :
        DefaultContractResolver
    {
        readonly JsonConverter[] _converters;

        public JsonContractResolver(params JsonConverter[] converters)
        {
            _converters = converters;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            for (var i = 0; i < _converters.Length; i++)
            {
                if (_converters[i].CanConvert(objectType))
                {
                    contract.Converter = _converters[i];
                    break;
                }
            }

            return contract;
        }

        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var contract = base.CreateDictionaryContract(objectType);

            contract.DictionaryKeyResolver = ContractDictionaryKeyResolver;

            return contract;
        }

        static string ContractDictionaryKeyResolver(string x)
        {
            return x;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (!property.Writable)
            {
                var propertyInfo = member as PropertyInfo;
                if (propertyInfo?.GetSetMethod(true) != null)
                    property.Writable = true;
            }

            return property;
        }
    }
}
