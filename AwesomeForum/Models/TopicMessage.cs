using AwesomeForum.Data.Base.AweForum.Data.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeForum.Models
{
    public class TopicMessage : IModelBase
    {
        public int id { get; set; }
        public string messageText { get; set; }
        public string userId { get; set; }
        [ForeignKey(nameof(userId))]
        public AppUser user { get; set; }
        public int topicId { get; set; }
        [ForeignKey(nameof(topicId))]
        public Topic topic { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastEditedDate { get; set; }
        public List<Message_Reaction> message_reactions { get; set; }
        public List<User_Reaction> user_reactions { get; set; }
    }
}
