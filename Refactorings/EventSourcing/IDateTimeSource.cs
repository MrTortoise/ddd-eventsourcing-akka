using System;

namespace EventSourcing
{
    public interface IDateTimeSource
    {
        DateTime Now { get; }
    }
}