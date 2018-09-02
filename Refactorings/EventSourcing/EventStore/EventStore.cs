using System.Collections.Generic;

namespace EventSourcing
{
    public class EventStore 
    {
        private Dictionary<string, List<IEvent>> _events = new Dictionary<string, List<IEvent>>();

        public bool ContainsStreamName(string streamName)
        {
            return false;
        }
              
        public void WriteEvent(string streamName, IEvent @event)
        {
            if (!_events.ContainsKey(streamName))
            {
                _events.Add(streamName, new List<IEvent>());
            }
            
            _events[streamName].Add(@event);
        }

        public IEnumerable<IEvent> LoadEventStream(string streamName)
        {
            if (_events.ContainsKey(streamName)) return _events[streamName];
            
            return new List<IEvent>();
        }
    }
}