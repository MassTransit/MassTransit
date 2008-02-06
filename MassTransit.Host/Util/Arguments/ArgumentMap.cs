namespace nu.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;

    public class ArgumentMap : IArgumentMap
    {
        private readonly Dictionary<string, ArgumentTarget> _namedArgs = new Dictionary<string, ArgumentTarget>();
        private readonly Type _type;
        private readonly List<ArgumentTarget> _unnamedArgs = new List<ArgumentTarget>();

        public ArgumentMap(Type type)
        {
            _type = type;

            foreach (PropertyInfo property in _type.GetProperties())
            {
                object[] attributes = property.GetCustomAttributes(typeof (ArgumentAttribute), true);
                if (attributes.Length == 1)
                {
                    ArgumentAttribute attribute = (ArgumentAttribute) attributes[0];
                    DefaultArgumentAttribute defaultAttribute = attribute as DefaultArgumentAttribute;

                    ArgumentTarget arg = (defaultAttribute == null)
                                             ? new ArgumentTarget(attribute, property)
                                             : new DefaultArgumentTarget(attribute, property);

                    if (string.IsNullOrEmpty(arg.Attribute.Key))
                        _unnamedArgs.Add(arg);
                    else
                        _namedArgs.Add(arg.Attribute.Key, arg);
                }
            }
        }

        #region IArgumentMap Members

        public string Usage
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                List<ArgumentTarget> allArgs = new List<ArgumentTarget>();
                allArgs.AddRange(_namedArgs.Values);
                allArgs.AddRange(_unnamedArgs);

                foreach (ArgumentTarget target in allArgs)
                {
                    if (target is DefaultArgumentTarget)
                    {
                        sb.AppendFormat("{1}<{0}>{2} ", target.Property.Name, target.Attribute.Required ? "" : "[",
                                        target.Attribute.Required ? "" : "]");
                    }
                    else if (target.Attribute.Required)
                        sb.AppendFormat("-{0} ", target.Attribute.Key);
                    else
                        sb.AppendFormat("[-{0}] ", target.Attribute.Key);
                }

                sb.Append(Environment.NewLine + Environment.NewLine);

                foreach (ArgumentTarget target in allArgs)
                {
                    if (target is DefaultArgumentTarget)
                    {
                        sb.AppendFormat("{0,-20}{1}" + Environment.NewLine, "<" + target.Property.Name + ">",
                                        target.Attribute.Description);
                    }
                    else
                    {
                        sb.AppendFormat("{0,-20}{1}" + Environment.NewLine, target.Attribute.Key,
                                        target.Attribute.Description);
                    }
                }

                return sb.ToString();
            }
        }

        #endregion

        public IEnumerable<IArgument> ApplyTo(object obj, IEnumerable<IArgument> arguments)
        {
            List<IArgument> unused = new List<IArgument>();

            int unnamedIndex = 0;

            foreach (IArgument arg in arguments)
            {
                if (string.IsNullOrEmpty(arg.Key))
                {
                    if (unnamedIndex < _unnamedArgs.Count)
                    {
                        ApplyValueToProperty(_unnamedArgs[unnamedIndex++].Property, obj, arg.Value);
                    }
                    else
                    {
                        unused.Add(arg);
                    }
                }
                else if (_namedArgs.ContainsKey(arg.Key))
                {
                    ApplyValueToProperty(_namedArgs[arg.Key].Property, obj, arg.Value);
                }
                else
                {
                    unused.Add(arg);
                }
            }

            return unused;
        }

        public void ApplyValueToProperty(PropertyInfo property, object obj, string argumentValue)
        {
            object value;

            if (property.PropertyType == typeof (bool))
            {
                value = bool.Parse(argumentValue);
            }
            else
            {
                value = argumentValue;
            }

            property.SetValue(obj, value, BindingFlags.Default, null, null, CultureInfo.InvariantCulture);
        }
    }
}