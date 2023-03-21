using AwesomeForum.Models;

namespace AwesomeForum.Data.ViewModels
{
    public class NewTopicDropdownVM
    {
        public NewTopicDropdownVM()
        {
            Forums = new List<Forum>();
        }
        public List<Forum> Forums { get; set; }
    }
}
