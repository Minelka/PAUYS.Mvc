using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAUYS.Mvc.Areas.Admin.Models;
using System.Security.Claims;

namespace PAUYS.Mvc.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize(Policy = "ContentPolicy")]
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory; // IHttpClientFactory'yi ekliyoruz
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Users";

        // IHttpClientFactory'yi konstruktor ile enjekte ediyoruz
        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult UserEdit(string id)
        {
            string userId = GetUserId();
            var client = _httpClientFactory.CreateClient();
            UserUpdateViewModel model = client.GetFromJsonAsync<UserUpdateViewModel>(_apiBaseUrl + $"/{userId}").Result;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserEdit(UserUpdateViewModel model)
        {
            model.Id = GetUserId();

            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage responseMessage = client.PutAsJsonAsync(_apiBaseUrl + $"/{model.Id}", model).Result;

            if (!responseMessage.IsSuccessStatusCode)
            {
                ModelState.AddModelError("ServiceError", responseMessage.RequestMessage.Content.ReadAsStringAsync().Result);
                return View(model);
            }

            return RedirectToAction("Index", "MainPage");
        }

        public string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier); // Kullanıcı ID'si genellikle NameIdentifier claim'de yer alır.
        }
    }
}
