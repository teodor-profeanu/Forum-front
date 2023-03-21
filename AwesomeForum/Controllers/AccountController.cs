using AwesomeForum.Data.Services;
using AwesomeForum.Data.ViewModels;
using AwesomeForum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace AwesomeForum.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _apiUrl;
        private UserService _userService = new UserService();

        public AccountController(IConfiguration configuration)
        {
            _apiUrl = configuration.GetValue<string>("ApiUrl");
        }

        public async Task<IActionResult> Index()
        {
            if (!_userService.UserLoggedIn(Request))
            {
                return RedirectToAction("Index", "Home");
            }

            _userService.SetHttpContextUser(Request, HttpContext);

            AppUser user = _userService.GetLoggedInUser(Request);
            
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM newLogin)
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            if (!ModelState.IsValid)
            {
                return View(newLogin);
            }

            AppUser user = null;
            using (var httpClient = new HttpClient())
            {
                //var tmp1 = new { username = newLogin.EmailOrUsername, password = newLogin.Password };
                //StringContent content = new StringContent(JsonConvert.SerializeObject(tmp1), Encoding.UTF8, "application/json");

                var urlExtention = "/user/login?username=" + newLogin.EmailOrUsername + "&password=" + newLogin.Password;
                //using (var response = await httpClient.PostAsync(_apiUrl + "/login", content))
                using (var response = await httpClient.PostAsync(_apiUrl + urlExtention, null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "{\"id\":2,\"username\":\"ileana\",\"password\":\"qwerty\",\"email\":\"ileana@gmail.com\",\"nrOfMessages\":0,\"nrOfTopics\":0}";
                    if (apiResponse != null)
                    {
                        user = JsonConvert.DeserializeObject<AppUser>(apiResponse);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    if (user == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    _userService.SetUserLoginCookie(Response, user);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            AppUser user = null;
            using (var httpClient = new HttpClient())
            {
                var urlExtention = "/user?id=" + id.ToString();
                using (var response = await httpClient.GetAsync(_apiUrl + urlExtention))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "{\"id\":2,\"username\":\"ileana\",\"password\":\"qwerty\",\"email\":\"ileana@gmail.com\",\"nrOfMessages\":7,\"nrOfTopics\":88}";
                    if (apiResponse != null)
                    {
                        user = JsonConvert.DeserializeObject<AppUser>(apiResponse);
                    }
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

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM newRegister)
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            if (!ModelState.IsValid)
            {
                return View(newRegister);
            }

            AppUser user = null;
            using (var httpClient = new HttpClient())
            {
                /*var tmp1 = new
                {
                    email = newRegister.Email,
                    username = newRegister.Username,
                    password = newRegister.Password,
                    repeatPassword = newRegister.RepeatPassword
                };
                StringContent content = new StringContent(JsonConvert.SerializeObject(tmp1), Encoding.UTF8, "application/json");*/

                var urlExtention = "/user/register?email=" + newRegister.Email + "&username=" + newRegister.Username + "&password=" + newRegister.Password + "&repeatPassword=" + newRegister.RepeatPassword;
                //using (var response = await httpClient.PostAsync(_apiUrl + "/register", content))
                using (var response = await httpClient.PostAsync(_apiUrl + urlExtention, null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "{\"id\":2,\"username\":\"ileana\",\"password\":\"qwerty\",\"email\":\"ileana@gmail.com\",\"nrOfMessages\":7,\"nrOfTopics\":0}";
                    if (apiResponse != null)
                    {
                        user = JsonConvert.DeserializeObject<AppUser>(apiResponse);
                    }
                    else
                    {
                        RedirectToAction("Register", "Account");
                    }

                    _userService.SetUserLoginCookie(Response, user);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            _userService.SetHttpContextUser(Request, HttpContext);

            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("UserNrOfMessages");
            Response.Cookies.Delete("UserNrOfTopics");

            HttpContext.User = null;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!_userService.UserLoggedIn(Request))
            {
                return RedirectToAction("Index", "Home");
            }

            _userService.SetHttpContextUser(Request, HttpContext);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(RegisterVM registerVM)
        {
            if (!_userService.UserLoggedIn(Request))
            {
                return RedirectToAction("Index", "Home");
            }

            _userService.SetHttpContextUser(Request, HttpContext);

            AppUser user = _userService.GetLoggedInUser(Request);
            using (var httpClient = new HttpClient())
            {
                /*var tmp1 = new
                {
                    id = user.Id,
                    oldPassword = registerVM.Password,
                    newPassword = registerVM.RepeatPassword
                };
                StringContent content = new StringContent(JsonConvert.SerializeObject(tmp1), Encoding.UTF8, "application/json");*/
                bool tmp = false;
                var urlExtention = "/user/change-password?id=" + user.Id + "&oldPassword=" + registerVM.Password + "&newPassword=" + registerVM.RepeatPassword;
                using (var response = await httpClient.PostAsync(_apiUrl + urlExtention, null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //apiResponse = "true";
                    if (apiResponse != null)
                    {
                        tmp = JsonConvert.DeserializeObject<bool>(apiResponse);
                    }

                    if (!tmp)
                    {
                        return RedirectToAction("Index", "Home");
                    }                    
                }
            }
            return RedirectToAction("Index", "Account");
        }
    }
}
