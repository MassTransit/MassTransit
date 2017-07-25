namespace System.Net.Mime
{
    public class ContentType : IEquatable<ContentType>
    {
        public ContentType(string mediaType)
        {
            MediaType = mediaType;
        }

        public string MediaType { get; private set; }

        public bool Equals(ContentType other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(this.MediaType, other.MediaType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((ContentType)obj);
        }

        public override int GetHashCode()
        {
            return (this.MediaType != null ? this.MediaType.GetHashCode() : 0);
        }

        public static bool operator ==(ContentType left, ContentType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ContentType left, ContentType right)
        {
            return !Equals(left, right);
        }
    }
}