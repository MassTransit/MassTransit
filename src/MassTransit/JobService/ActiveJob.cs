namespace MassTransit
{
    using System;


    /// <summary>
    /// Active Jobs are allocated a concurrency slot, and are valid until the deadline is reached, after
    /// which they may be automatically released.
    /// </summary>
    public class ActiveJob :
        IEquatable<ActiveJob>
    {
        public Guid JobId { get; set; }
        public DateTime Deadline { get; set; }

        public Uri InstanceAddress { get; set; }

        public bool Equals(ActiveJob other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return JobId.Equals(other.JobId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ActiveJob)obj);
        }

        public override int GetHashCode()
        {
            return JobId.GetHashCode();
        }
    }
}
