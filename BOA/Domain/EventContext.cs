namespace BOA.Domain
{
    class EventContext : IEventPublisher
    {
        public void Publish<T>(T @event) where T : IEvent
        {
            
        }
    }
}