namespace nu.Utility
{
    using System;

    /// <summary>
    /// Specifies the target field or property as an argument
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ArgumentAttribute :
        Attribute
    {
        private bool _allowMultiple;
        private string _defaultValue;
        private string _description;
        private string _key;
        private bool _required;

        /// <summary>
        /// True if the argument is required to exist
        /// </summary>
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        /// <summary>
        /// If no argument is specified, the default value to use for the associated property
        /// </summary>
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        /// <summary>
        /// True if multiple arguments are allowed for the associated property
        /// </summary>
        public bool AllowMultiple
        {
            get { return _allowMultiple; }
            set { _allowMultiple = value; }
        }

        /// <summary>
        /// The key used to identify this argument (-key:value)
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// The description to display for this argument
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DefaultArgumentAttribute :
        ArgumentAttribute
    {
    }
}