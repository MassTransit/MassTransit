namespace MassTransit.MongoDbIntegration
{
    using System.Reflection;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Conventions;


    public class SagaConvention :
        ConventionBase,
        IClassMapConvention
    {
        public void Apply(BsonClassMap classMap)
        {
            if (classMap.ClassType.GetTypeInfo().IsClass && typeof(ISaga).IsAssignableFrom(classMap.ClassType))
                classMap.MapIdProperty(nameof(ISaga.CorrelationId));
        }
    }
}
