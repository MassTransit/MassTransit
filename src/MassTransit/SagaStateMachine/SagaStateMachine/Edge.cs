namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Diagnostics;


    [Serializable]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public class Edge :
        IEquatable<Edge>
    {
        public Edge(Vertex from, Vertex to, string title)
        {
            From = from;
            To = to;
            Title = title;
        }

        public Vertex To { get; }

        public Vertex From { get; }

        string Title { get; }

        public string DebuggerDisplay => ToString();

        public bool Equals(Edge other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(To, other.To) && Equals(From, other.From) && string.Equals(Title, other.Title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Edge)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = To?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (From?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Title?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{From} -> {To} {Title}";
        }
    }
}
