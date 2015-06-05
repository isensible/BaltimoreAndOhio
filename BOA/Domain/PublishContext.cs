namespace BOA.Domain
{
    class PublishContext : ICommandSender
    {
        public void Send<T>(T command) where T : ICommand
        {
            
        }
    }

    internal interface ICommandSender
    {
    }

    class EventContext : IEventPublisher
    {
        public void Publish<T>(T @event) where T : IEvent
        {
            
        }
    }

    internal interface IEvent
    {
    }

    internal interface IEventPublisher
    {
    }
}
