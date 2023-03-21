using AwesomeForum.Models;
using System.Security.Claims;

namespace AwesomeForum.Data.Services
{
    public class UserService
    {
        public AppUser GetLoggedInUser(HttpRequest request)
        {
            var user = new AppUser
            {
                Id = request.Cookies["UserId"],
                UserName = request.Cookies["UserName"],
                nrOfMessages = Convert.ToInt32(request.Cookies["UserNrOfMessages"]),
                nrOfTopics = Convert.ToInt32(request.Cookies["UserNrOfTopics"])
            };

            return user;
        }

        public bool UserLoggedIn(HttpRequest request)
        {
            if (request.Cookies["UserName"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetHttpContextUser(HttpRequest request, HttpContext context)
        {
            if (UserLoggedIn(request))
            {
                var userName = request.Cookies["UserName"];
                var userId = request.Cookies["UserId"];
                var nrOfMessages = request.Cookies["UserNrOfMessages"];
                var nrOfTopics = request.Cookies["UserNrOfTopics"];

                var identity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("UserName", userName, ClaimValueTypes.String),
                    new Claim("UserId", userId, ClaimValueTypes.String),
                    new Claim("UserNrOfMessages", nrOfMessages, ClaimValueTypes.String),
                    new Claim("UserNrOfTopics", nrOfTopics, ClaimValueTypes.String)
                }, "Custom");

                context.User = new ClaimsPrincipal(identity);
            }
        }

        public void SetUserLoginCookie(HttpResponse response, AppUser user)
        {
            response.Cookies.Append("UserName", user.UserName);
            response.Cookies.Append("UserId", user.Id);
            response.Cookies.Append("UserNrOfMessages", user.nrOfMessages.ToString());
            response.Cookies.Append("UserNrOfTopics", user.nrOfTopics.ToString());
        }
    }
}
