namespace HeavyLoad.Load
{
    using System;

    [Serializable]
    public class SimpleResponse
    {
        private int[] _values;

        public SimpleResponse()
        {
            _values = new int[64];
        }

        public int[] Values
        {
            get { return _values; }
            set { _values = value; }
        }
    }
}