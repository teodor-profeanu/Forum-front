using AwesomeForum.Data.Services;
using AwesomeForum.Data.ViewModels;
using AwesomeForum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace AwesomeForum.Controllers
{
    public class TopicController : Controller
    {
        private readonly string _apiUrl;
        private UserService _userService = new UserService();

        public TopicController(IConfiguration configuration)
        {
            _apiUrl = configuration.GetValue<string>("ApiUrl");
        }
        public async Task<IActionResult> Details(int id = 7)
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            Topic topic = new Topic();
            List<TopicMessage> topicMessages = new List<TopicMessage>();
            TopicDetailsVM topicDeets = new TopicDetailsVM
            {
                topic = new Topic(),
                topicMessages = new List<TopicMessage>()
            };
            var urlExtention = "/topic?id=" + id.ToString();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiUrl + urlExtention))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "{\"id\":1,\"name\":\"All about React\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"forumId\":3,\"dateCreated\":\"2023-01-18\",\"lastPosted\":\"2023-01-18\",\"messageCount\":2,\"messages\":[{\"id\":1,\"text\":\"Hey I'm new to React actually, I have no new information to give you.\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"topicId\":1,\"dateCreated\":\"2023-01-18\",\"lastEdited\":\"2023-01-18\"},{\"id\":2,\"text\":\"This topic is useless!!\",\"userId\":2,\"user\":{\"id\":2,\"username\":\"ileana\",\"password\":\"qwerty\",\"email\":\"ileana@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"topicId\":1,\"dateCreated\":\"2023-01-18\",\"lastEdited\":\"2023-01-18\"}]}";
                    if (apiResponse != null)
                    {
                        topicDeets = DecodeTopicAndMessages(apiResponse);
                    }
                }
            }

            if(topicDeets.topicMessages.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(topicDeets);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            List<Forum> forums = new List<Forum>();

            List<Category> categories = new List<Category>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_apiUrl + "/categories"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "[{\"id\":1,\"name\":\"cat1\",\"orderNr\":1,\"forums\":[{\"id\":1,\"name\":\"for1\",\"orderNr\":1,\"categoryId\":1},{\"id\":2,\"name\":\"for2\",\"orderNr\":2,\"categoryId\":1}]},{\"id\":2,\"name\":\"cat2\",\"orderNr\":2,\"forums\":[{\"id\":3,\"name\":\"for3\",\"orderNr\":3,\"categoryId\":2},{\"id\":4,\"name\":\"for4\",\"orderNr\":4,\"categoryId\":2}]}]";
                    if (apiResponse != null)
                    {
                        categories = JsonConvert.DeserializeObject<List<Category>>(apiResponse);
                    }
                    else
                    {
                        categories = new List<Category>
                        {
                            new Category
                            {
                                id = 1,
                                name = "Administrative",
                                orderNr = 0,
                                forums = new List<Forum> {
                                new Forum {
                                    id = 1,
                                    name = "Rules & announcements",
                                    topicCount = 3,
                                    orderNr = 0,
                                    categoryId = 1
                                },
                                new Forum {
                                    id = 2,
                                    name = "Welcome",
                                    topicCount = 17,
                                    orderNr = 1,
                                    categoryId = 1
                                }
                            }
                            },
                            new Category
                            {
                                id = 2,
                                name = "IT Talks",
                                orderNr = 1,
                                forums = new List<Forum> {
                                new Forum {
                                    id = 3,
                                    name = "Software",
                                    topicCount = 28,
                                    orderNr = 0,
                                    categoryId = 1
                                },
                                new Forum {
                                    id = 4,
                                    name = "Hardware",
                                    topicCount = 5,
                                    orderNr = 1,
                                    categoryId = 1
                                },
                                new Forum {
                                    id = 5,
                                    name = "DevOps",
                                    topicCount = 67,
                                    orderNr = 1,
                                    categoryId = 1
                                }
                            }
                            }

                        };
                    }
                }
            }

            foreach (var category in categories)
            {
                foreach (var forum in category.forums)
                {
                    forums.Add(forum);
                }
            }

            if (forums.Count == 0)
            {
                forums = new List<Forum>
                {
                    new Forum {
                        id = 1,
                        name = "Rules & announcements",
                        topicCount = 3,
                        orderNr = 0,
                        categoryId = 1
                    },
                    new Forum {
                        id = 2,
                        name = "Welcome",
                        topicCount = 17,
                        orderNr = 1,
                        categoryId = 1
                    }
                };
            }

            ViewBag.Forums = new SelectList(forums, "id", "name", id.ToString());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewTopicVM newTopic)
        {
            _userService.SetHttpContextUser(Request, HttpContext);
            AppUser user = _userService.GetLoggedInUser(Request);

            TopicDetailsVM topicDetailsVM = null;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(newTopic), Encoding.UTF8, "application/json");

                var urlExtention = "/topic/new?userId=" + user.Id + "&forumId=" + newTopic.ForumId + "&name=" + newTopic.Name + "&message=" + newTopic.MessageText;
                using (var response = await httpClient.PostAsync(_apiUrl + urlExtention, null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "{\"id\":1,\"name\":\"All about React\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"forumId\":3,\"dateCreated\":\"2023-01-18\",\"lastPosted\":\"2023-01-18\",\"messageCount\":1,\"messages\":[{\"id\":1,\"text\":\"Hey I'm new to React actually, I have no new information to give you.\",\"userId\":1,\"user\":{\"id\":1,\"username\":\"asmo_192\",\"password\":\"1234\",\"email\":\"teo@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0},\"topicId\":1,\"dateCreated\":\"2023-01-18\",\"lastEdited\":\"2023-01-18\"}]}";
                    if (apiResponse != null)
                    {
                        topicDetailsVM = DecodeTopicAndMessages(apiResponse);
                    }
                }
            }
            if (topicDetailsVM != null && topicDetailsVM.topicMessages.Count > 0)
            {
                return RedirectToAction("Details", "Topic", new {id = topicDetailsVM.topic.id});
            }

            return RedirectToAction("Index", "Home");
        }

        public TopicDetailsVM DecodeTopicAndMessages(string topicAndMessages)
        {
            dynamic topicDetails = JsonConvert.DeserializeObject<dynamic>(topicAndMessages);

            var topic = new Topic
            {
                id = topicDetails.id,
                name = topicDetails.name,
                creatorId = topicDetails.userId,
                creator = new AppUser
                {
                    Id = topicDetails.userId,
                    UserName = topicDetails.user.username,
                    nrOfMessages = topicDetails.user.nrOfMessages,
                    nrOfTopics = topicDetails.user.nrOfTopics
                },
                forumId = topicDetails.forumId,
                dateCreated = topicDetails.dateCreated,
                lastPosted = topicDetails.lastPosted,
                messageCount = topicDetails.messageCount
            };

            var topicMessages = new List<TopicMessage>();
            foreach (var message in topicDetails.messages)
            {
                TopicMessage msg = new TopicMessage
                {
                    id = message.id,
                    messageText = message.text,
                    userId = message.userId,
                    user = new AppUser
                    {
                        Id = message.user.id,
                        UserName = message.user.username,
                        Email = message.user.email,
                        nrOfMessages = message.user.nrOfMessages,
                        nrOfTopics = message.user.nrOfTopics
                    },
                    topicId = message.topicId,
                    createdDate = message.dateCreated,
                    lastEditedDate = message.lastEdited
                };

                topicMessages.Add(msg);
            }

            return new TopicDetailsVM { topic = topic, topicMessages = topicMessages };
        }

    }
}
