using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AwesomeForum.Data.ViewModels
{
    public class NewTopicVM
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Display(Name = "Forum")]
        public int ForumId { get; set; }
        [Required(ErrorMessage = "Message is required")]
        [Display(Name = "Message")]
        public string MessageText { get; set; }
    }
}
