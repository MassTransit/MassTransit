namespace MassTransit.SagaStateMachine
{
    using System;


    [Serializable]
    public class Vertex :
        IEquatable<Vertex>
    {
        public Vertex(Type type, Type targetType, string title, bool isComposite)
        {
            VertexType = type;
            TargetType = targetType;
            Title = title;
            IsComposite = isComposite;
        }

        public string Title { get; }

        public Type VertexType { get; }

        public Type TargetType { get; }

        public bool IsComposite { get; }

        public bool Equals(Vertex other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Title, other.Title) && VertexType == other.VertexType && TargetType == other.TargetType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Vertex)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Title?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (VertexType?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (TargetType?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
