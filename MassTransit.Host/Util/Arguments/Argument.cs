namespace nu.Utility
{
    public class Argument : IArgument
    {
        private readonly string _value;
        protected readonly string _key;

        public Argument(string value)
        {
            _value = value;
        }

        public Argument(string key, string value)
        {
            _key = key;
            _value = value;
        }

        #region IArgument Members

        public string Value
        {
            get { return _value; }
        }

        public string Key
        {
            get { return _key; }
        }

        #endregion
    }
      
}