namespace MassTransit
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;


    [Serializable]
    [DebuggerDisplay("{Status}")]
    public struct CompositeEventStatus :
        IComparable<CompositeEventStatus>
    {
        int _bits;

        public CompositeEventStatus(int bits)
        {
            _bits = bits;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Status
        {
            get
            {
                var bits = _bits;
                return string.Join("", Enumerable.Range(0, 32).Select(x => (bits & (1 << x)) == 0 ? "0" : "1"));
            }
        }

        public int Bits => _bits;

        public int CompareTo(CompositeEventStatus other)
        {
            return other._bits - _bits;
        }

        public bool Equals(CompositeEventStatus other)
        {
            return other._bits == _bits;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (obj.GetType() != typeof(CompositeEventStatus))
                return false;
            return Equals((CompositeEventStatus)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public void Set(int flag)
        {
            _bits |= flag;
        }
    }
}
