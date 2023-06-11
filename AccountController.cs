using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using ShoppingCart_Core.Models;
using System.Net.Http.Json;

namespace ShoppingCart_Core.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(Login login)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                var postTask = await client.PostAsJsonAsync<Login>("api/Account/login", login);
                if (postTask.IsSuccessStatusCode)
                {
                    var customerJsonString = postTask.Content.ReadAsStringAsync();
                    var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponseModel>(customerJsonString.Result);

                    HttpContext.Session.SetString("Token", deserialized.Token);
                    HttpContext.Session.SetString("Username", deserialized.Username);
                    TempData["username"] = deserialized.Username;
                    HttpContext.Session.SetString("UserRole", deserialized.Role);
                    if (deserialized.Role == "Admin")
                    {
                        return RedirectToAction("GetProducts", "Admin");
                    }
                    else if (deserialized.Role == "User")
                    {
                        return RedirectToAction("ShowProducts", "User");
                    }
                    else
                    {
                        return RedirectToAction("Unauthorized", "Error");
                    }
                }
                ModelState.AddModelError(string.Empty, "Please enter the valid email and password.");
            }
            return View(login);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Register register)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://localhost:7257/");

                    var postTask = await httpClient.PostAsJsonAsync("api/Account/signup", register);

                    if (postTask.IsSuccessStatusCode)
                    {
                        var customerJsonString = await postTask.Content.ReadAsStringAsync();

                        return RedirectToAction("Login"); 
                    }
                    else
                    { 
                        var errorResponse = await postTask.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, $"Signup failed. Error: {errorResponse}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred during signup. Error: {ex.Message}");
            }

            return View(register); 
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }



    }
}
