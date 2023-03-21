using AwesomeForum.Data.Services;
using AwesomeForum.Data.ViewModels;
using AwesomeForum.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AwesomeForum.Controllers
{
    public class MessageController : Controller
    {
        private readonly string _apiUrl;
        private UserService _userService = new UserService();
        private TopicController _topicController;

        public MessageController(IConfiguration configuration)
        {
            _apiUrl = configuration.GetValue<string>("ApiUrl");
            _topicController = new TopicController(configuration);
        }

        [HttpGet]
        public IActionResult Create(int id = 8)
        {
            _userService.SetHttpContextUser(Request, HttpContext);
            TempData["TopicId"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewTopicVM newTopic)
        {
            _userService.SetHttpContextUser(Request, HttpContext);
            AppUser user = _userService.GetLoggedInUser(Request);

            var topicId = TempData["TopicId"];

            TopicDetailsVM topicDetailsVM = new TopicDetailsVM
            {
                topic = new Topic(),
                topicMessages = new List<TopicMessage>()
            };
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(newTopic), Encoding.UTF8, "application/json");

                var urlExtention = "/message/new?userId=" + user.Id + "&topicId=" + topicId + "&message=" + newTopic.MessageText;
                using (var response = await httpClient.PostAsync(_apiUrl + urlExtention, null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "{\"id\":1,\"name\":\"All about React\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"forumId\":3,\"dateCreated\":\"2023-01-18\",\"lastPosted\":\"2023-01-18\",\"messageCount\":2,\"messages\":[{\"id\":1,\"text\":\"Hey I'm new to React actually, I have no new information to give you.\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"topicId\":1,\"dateCreated\":\"2023-01-18\",\"lastEdited\":\"2023-01-18\"},{\"id\":2,\"text\":\"This topic is useless!!\",\"userId\":2,\"user\":{\"id\":2,\"username\":\"ileana\",\"password\":\"qwerty\",\"email\":\"ileana@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"topicId\":1,\"dateCreated\":\"2023-01-18\",\"lastEdited\":\"2023-01-18\"}]}";
                    if (apiResponse != null)
                    {
                        topicDetailsVM = _topicController.DecodeTopicAndMessages(apiResponse);
                    }
                }
            }

            if (topicDetailsVM.topicMessages.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Details", "Topic", new {id = topicDetailsVM.topic.id});
        }
    }
}
