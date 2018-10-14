namespace Discord.Net.Framework
{
    public class CommandFollowUpContext
    {
        public CommandFollowUpContext(string tag, object data)
        {
            Tag = tag;
            Data = data;
        }

        public string Tag { get; set; }
        public object Data { get; set; }

        internal DiscordBotFramework _instance;
        internal IUser _owner;

        public void Release()
        {
            if(_instance.FollowUpContexts.ContainsKey(_owner.Id) && _instance.FollowUpContexts[_owner.Id] == this)
                _instance.FollowUpContexts.Remove(_owner.Id);
        }
    }
}