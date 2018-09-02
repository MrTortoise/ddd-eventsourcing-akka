using System;

namespace EventSourcing
{
    public class DateTimeSource : IDateTimeSource
    {
        public DateTime Now => DateTime.Now;
    }
}