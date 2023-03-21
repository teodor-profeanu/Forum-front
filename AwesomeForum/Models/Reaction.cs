using AwesomeForum.Data.Base.AweForum.Data.Base;

namespace AwesomeForum.Models
{
    public class Reaction : IModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public List<User_Reaction> user_reactions { get; set; }
        public List<Message_Reaction> message_reactions { get; set; }
    }
}
