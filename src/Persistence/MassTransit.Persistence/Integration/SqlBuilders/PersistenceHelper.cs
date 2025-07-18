namespace MassTransit.Persistence.Integration.SqlBuilders
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using Saga;


    /// <summary>
    /// Had to have a god-object somewhere to shove all the tangentially related methods...
    /// </summary>
    public static class PersistenceHelper
    {
        public static string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        public static string GetTableName(Type type)
        {
            var tableName = AttributeValue(type, "TableAttribute", "Name");
            if (!string.IsNullOrEmpty(tableName))
                return tableName!;

            return type.Name + "s";
        }

        public static string GetIdColumnName<T>()
        {
            return GetIdColumnName(typeof(T));
        }

        public static string GetIdColumnName(Type type)
        {
            var properties = type.GetProperties();

            // support the Dapper.Contrib manual-mapping of keys or non-identity keys
            var keyColumn = AttributeValue(type, "KeyAttribute", "Name");
            if (!string.IsNullOrEmpty(keyColumn))
                return keyColumn!;

            var explicitKeyColumn = AttributeValue(type, "ExplicitKeyAttribute", "Name");
            if (!string.IsNullOrEmpty(explicitKeyColumn))
                return explicitKeyColumn!;

            if (properties.Any(p => p.Name == "CorrelationId"))
                return "CorrelationId";

            throw new InvalidOperationException("Only CorrelationId can be auto-detected as the key column.  Use constructor if necessary to override.");
        }

        public static string? GetVersionColumnName<TProp>(Type type, string defaultName)
        {
            var candidate = type.GetProperties()
                .FirstOrDefault(p => p.Name.Equals(defaultName, StringComparison.OrdinalIgnoreCase));

            return candidate is not null && candidate.PropertyType == typeof(TProp)
                ? candidate.Name
                : null;
        }

        public static string? GetColumnName(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);
            if (property is null)
                return null;

            return GetColumnName(type, property);
        }

        public static string GetColumnName(Type type, PropertyInfo property, List<SqlPropertyMapping>? mappings = null)
        {
            var name = property.Name;
            var mapping = mappings?.FirstOrDefault(m => m.Property.Member.Name == name);
            if (mapping is not null)
                return mapping.Name ?? name;

            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute is null || string.IsNullOrEmpty(columnAttribute.Name))
                return name;

            return columnAttribute.Name;
        }

        public static ModelPropertyCollection BuildProperties(Type modelType, List<SqlPropertyMapping> mappings)
        {
            var properties = (from prop in modelType.GetProperties()
                let columnName = GetColumnName(modelType, prop, mappings)
                let propertyName = NormalizeName(prop.Name)
                select (columnName, propertyName));

            return ModelPropertyCollection.FromProperties(properties);
        }
        
        public static string NormalizeName(string original)
        {
#if NET8_0_OR_GREATER
            return new string(original.ToLowerInvariant().Where(char.IsAsciiLetterOrDigit).ToArray());
#else
            return new string(original.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
#endif
        }

        public static string? AttributeValue(Type type, string attributeName, string propertyName)
        {
            var tableAttribute = type.GetCustomAttributes()
                .FirstOrDefault(a => a.GetType().Name.StartsWith(attributeName, StringComparison.OrdinalIgnoreCase));

            var nameProperty = tableAttribute?.GetType().GetProperty(propertyName);
            if (nameProperty is null)
                return null;

            var nameValue = (string?)nameProperty.GetValue(tableAttribute);

            return !string.IsNullOrEmpty(nameValue)
                ? nameValue
                : null;
        }
    }


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


    public class ModelProperty : IEquatable<ModelProperty>
    {
        sealed class ColumnNameEqualityComparer : IEqualityComparer<ModelProperty>
        {
            public bool Equals(ModelProperty? x, ModelProperty? y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (x is null)
                    return false;
                if (y is null)
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.ColumnName, y.ColumnName, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(ModelProperty obj)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ColumnName);
            }
        }
        
        public static IEqualityComparer<ModelProperty> ColumnNameComparer { get; } = new ColumnNameEqualityComparer();

        public bool Equals(ModelProperty? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(ColumnName, other.ColumnName, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ModelProperty)obj);
        }

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(ColumnName);

        public static bool operator ==(ModelProperty? left, ModelProperty? right) => Equals(left, right);

        public static bool operator !=(ModelProperty? left, ModelProperty? right) => !Equals(left, right);

        public ModelProperty(string columnName, string propertyName)
        {
            ColumnName = columnName;
            PropertyName = propertyName;
        }

        public string ColumnName { get; private set; }
        public string PropertyName { get; private set; }
    }
}
