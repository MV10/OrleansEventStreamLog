using System;

namespace DomainModel.DomainEvents
{
    public class DomainEventBase
    {
        public static readonly int NEW_ETAG = -1;
        public DateTimeOffset Timestamp = DateTimeOffset.Now;
        public int ETag { get; set; } = NEW_ETAG;
    }
}
