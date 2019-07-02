namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;


    public class JsonContractResolver :
        DefaultContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);

            contract.DictionaryKeyResolver = ContractDictionaryKeyResolver;

            return contract;
        }

        static string ContractDictionaryKeyResolver(string x)
        {
            return x;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
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
