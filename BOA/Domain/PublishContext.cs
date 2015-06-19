namespace BOA.Domain
{
    class PublishContext : ICommandSender
    {
        public void Send<T>(T command) where T : ICommand
        {
            
        }
    }
}
