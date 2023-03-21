using AwesomeForum.Data.Base.AweForum.Data.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeForum.Models
{
    public class Topic : IModelBase
    {
        public Topic()
        {
            messageCount = 1;
        }
        public int id { get; set; }
        public string name { get; set; }
        public string creatorId { get; set; }
        [ForeignKey(nameof(creatorId))]
        public AppUser creator { get; set; }
        public int forumId { get; set; }
        [ForeignKey(nameof(forumId))]
        public Forum forum { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime lastPosted { get; set; }
        public int messageCount { get; set; }
    }
}
