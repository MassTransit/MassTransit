namespace nu.Utility
{
    using System.Reflection;

    public class ArgumentTarget
    {
        private readonly ArgumentAttribute _attribute;
        private readonly PropertyInfo _property;

        public ArgumentTarget(ArgumentAttribute attribute, PropertyInfo property)
        {
            _attribute = attribute;
            _property = property;
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public ArgumentAttribute Attribute
        {
            get { return _attribute; }
        }
    }

    public class DefaultArgumentTarget : ArgumentTarget
    {
        public DefaultArgumentTarget(ArgumentAttribute attribute, PropertyInfo property)
            : base(attribute, property)
        {
        }
    }
}