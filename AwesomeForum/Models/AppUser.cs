using Microsoft.AspNetCore.Identity;

namespace AwesomeForum.Models
{
    public class AppUser : IdentityUser
    {
        public int nrOfMessages { get; set; }
        public int nrOfTopics { get; set; }
        public List<User_Reaction> user_reactions { get; set; }
    }
}
