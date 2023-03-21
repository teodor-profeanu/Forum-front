using AwesomeForum.Data.Base.AweForum.Data.Base;

namespace AwesomeForum.Models
{
    public class Category : IModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public int orderNr { get; set; }
        public List<Forum> forums { get; set; }
        public List<Message_Reaction> message_reactions { get; set; }
    }
}
