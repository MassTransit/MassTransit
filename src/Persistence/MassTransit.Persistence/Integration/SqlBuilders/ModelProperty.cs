namespace MassTransit.Persistence.Integration.SqlBuilders
{
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