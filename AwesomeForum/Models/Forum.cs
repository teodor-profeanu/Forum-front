using AwesomeForum.Data.Base.AweForum.Data.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeForum.Models
{
    public class Forum : IModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public int topicCount { get; set; }
        public List<Topic> topics { get; set; }
        public int orderNr { get; set; }
        public int categoryId { get; set; }
        [ForeignKey(nameof(categoryId))]
        public Category category { get; set; }
    }
}
