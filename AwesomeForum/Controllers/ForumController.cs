using AwesomeForum.Data.Services;
using AwesomeForum.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AwesomeForum.Controllers
{
    public class ForumController : Controller
    {
        private readonly string _apiUrl;
        private UserService _userService = new UserService();

        public ForumController(IConfiguration configuration) 
        {
            _apiUrl = configuration.GetValue<string>("ApiUrl");
        }
        public async Task<IActionResult> Details(int id = 7)
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            Forum forum = null;
            var urlExtention = "/forum?id=" + id.ToString();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiUrl + urlExtention))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "{\"id\":3,\"name\":\"Software\",\"orderNr\":1,\"categoryId\":2,\"topics\":[{\"id\":1,\"name\":\"All about React\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"forumId\":3,\"dateCreated\":\"2023-01-18\",\"lastPosted\":\"2023-01-18\",\"messageCount\":1,\"messages\":null}]}";
                    if (apiResponse != null)
                    {
                        forum = JsonConvert.DeserializeObject<Forum>(apiResponse);
                    }
                }
            }
            if (forum == null)
            {
                forum = new Forum
                {
                    id = 1,
                    name = "Software",
                    topicCount = 2,
                    topics = new List<Topic>
                {
                    new Topic
                    {
                        id = 1,
                        name = "All about React",
                        creator = new AppUser
                        {
                            UserName = "Copilul"
                        },
                        creatorId = "7",
                        dateCreated = DateTime.Now.AddDays(-10),
                        lastPosted = DateTime.Now.AddDays(-3).AddMinutes(87),
                        messageCount = 3
                    },
                    new Topic
                    {
                        id = 2,
                        name = "OpenGL Magic",
                        creator = new AppUser
                        {
                            UserName = "Baiatul"
                        },
                        creatorId = "7",
                        dateCreated = DateTime.Now.AddDays(-3),
                        lastPosted = DateTime.Now.AddDays(-1).AddMinutes(723),
                        messageCount = 10
                    }
                }
                };
            }
            return View(forum);
        }

        public async Task<IActionResult> UserTopics(int id)
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            List<Topic> topics = null;
            Forum forum = null;
            AppUser user = null;
            var urlExtention = "/topic/user?id=" + id.ToString();
            using (var httpClient = new HttpClient())
            {
                var urlExt = "/user?id=" + id.ToString();
                using (var response = await httpClient.GetAsync(_apiUrl + urlExt))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "{\"id\":2,\"username\":\"ileana\",\"password\":\"qwerty\",\"email\":\"ileana@gmail.com\",\"nrOfMessages\":7,\"nrOfTopics\":88}";
                    if (apiResponse != null)
                    {
                        user = JsonConvert.DeserializeObject<AppUser>(apiResponse);
                    }
                }

                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = "Enrique",
                        Email = "enrique@enriqmail.com",
                        nrOfMessages = 100,
                        nrOfTopics = 50
                    };
                }

                using (var response = await httpClient.GetAsync(_apiUrl + urlExtention))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "[{\"id\":1,\"name\":\"All about React\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"forumId\":3,\"dateCreated\":\"2023-01-18\",\"lastPosted\":\"2023-01-18\",\"messageCount\":2,\"messages\":null},{\"id\":2,\"name\":\"OpenGL Magic\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"forumId\":3,\"dateCreated\":\"2023-01-18\",\"lastPosted\":\"2023-01-18\",\"messageCount\":1,\"messages\":null}]";
                    if (apiResponse != null)
                    {
                        topics = JsonConvert.DeserializeObject<List<Topic>>(apiResponse);                        
                    }
                }

                if (topics == null)
                {
                    topics = new List<Topic>
                    {
                        new Topic
                        {
                            name = "All about React",
                            creator = new AppUser
                            {
                                UserName = "Copilul"
                            },
                            creatorId = "7",
                            dateCreated = DateTime.Now.AddDays(-10),
                            lastPosted = DateTime.Now.AddDays(-3).AddMinutes(87),
                            messageCount = 3

                        }
                    };
                }
            }
            
            forum = new Forum
            {
                name = "Topicurile userului " + user.UserName,
                topicCount = 1,
                topics = topics
            };

            return View("Details", forum);
        }
    }
}
