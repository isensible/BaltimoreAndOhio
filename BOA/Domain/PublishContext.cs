using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Events;

namespace BOA.Domain
{
    class PublishContext : ICommandSender
    {
        public void Send<T>(T command) where T : ICommand
        {
            
        }
    }

    class EventContext : CQRSlite.Events.IEventPublisher
    {
        public void Publish<T>(T @event) where T : IEvent
        {
            
        }
    }
}
