namespace MassTransit.Analyzers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;


    class PropertyNameEqualityComparer :
        IEqualityComparer<IPropertySymbol>
    {
        public static readonly PropertyNameEqualityComparer Instance = new PropertyNameEqualityComparer();

        public bool Equals(IPropertySymbol x, IPropertySymbol y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(IPropertySymbol obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
