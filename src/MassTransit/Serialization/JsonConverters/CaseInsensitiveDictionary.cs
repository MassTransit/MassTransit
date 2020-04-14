namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Generic;


    public class CaseInsensitiveDictionary<T> :
        Dictionary<string, T>
    {
        public CaseInsensitiveDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
