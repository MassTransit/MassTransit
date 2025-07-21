namespace MassTransit.Persistence.Integration.SqlBuilders
{
    using System.Collections;


    public class ModelPropertyCollection : IEnumerable<ModelProperty>
    {
        readonly List<ModelProperty> _properties = new();
        ModelPropertyCollection() { }

        public static ModelPropertyCollection FromProperties(IEnumerable<(string columnName, string propertyName)> properties)
        {
            var collection = new ModelPropertyCollection();
            var duplicates = new HashSet<ModelProperty>(ModelProperty.ColumnNameComparer);

            foreach (var property in properties)
            {
                var isId = property.propertyName.Equals(nameof(ISaga.CorrelationId), StringComparison.OrdinalIgnoreCase);

                var modelProperty = new ModelProperty(
                    property.columnName,
                    property.propertyName
                );

                if (!duplicates.Add(modelProperty))
                    continue;
                
                if (isId)
                    collection._properties.Insert(0, modelProperty);
                else
                    collection._properties.Add(modelProperty);
            }

            return collection;
        }

        public void Remove(string propertyName)
            => _properties.RemoveAll(p => p.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        public IEnumerator<ModelProperty> GetEnumerator() => _properties.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}