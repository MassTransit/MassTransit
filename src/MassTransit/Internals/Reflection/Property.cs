namespace MassTransit.Internals.Reflection
{
    using System;


    public class Property
    {
        public Property(string name, Type propertyType)
        {
            Name = name;
            PropertyType = propertyType;
        }

        public string Name { get; }

        /// <summary>
        /// The property type, which is already been realized in the current application, even if it's a dynamically
        /// created type.
        /// </summary>
        public Type PropertyType { get; }
    }
}
