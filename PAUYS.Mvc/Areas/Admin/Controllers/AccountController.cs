using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PAUYS.Mvc.Areas.Admin.Models;
using PAUYS.ViewModel.Concrete;
using System.Net.Http;
using System.Text;

namespace PAUYS.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory; // IHttpClientFactory'yi ekliyoruz
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Users";
        private readonly string _apiroleBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Roles";

        // IHttpClientFactory'yi konstruktor ile enjekte ediyoruz
        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Users()
        {
            var client = _httpClientFactory.CreateClient();
            List<UserUpdateViewModel> model = client.GetFromJsonAsync<List<UserUpdateViewModel>>(_apiBaseUrl).Result;

            return View(model);
        }

        public IActionResult UserAdd()
        {
            UserCreateViewModel model = new UserCreateViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserAdd(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage responseMessage = client.PostAsJsonAsync(_apiBaseUrl, model).Result;

            if (!responseMessage.IsSuccessStatusCode)
            {
                ModelState.AddModelError("ServiceError", responseMessage.RequestMessage.Content.ReadAsStringAsync().Result);
                return View(model);
            }

            return RedirectToAction("Users", "Account");
        }

        public IActionResult UserEdit(string id)
        {
            var client = _httpClientFactory.CreateClient();
            UserUpdateViewModel model = client.GetFromJsonAsync<UserUpdateViewModel>(_apiBaseUrl + $"/{id}").Result;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserEdit(UserUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage responseMessage = client.PutAsJsonAsync(_apiBaseUrl + $"/{model.Id}", model).Result;

            if (!responseMessage.IsSuccessStatusCode)
            {
                ModelState.AddModelError("ServiceError", responseMessage.RequestMessage.Content.ReadAsStringAsync().Result);
                return View(model);
            }

            return RedirectToAction("Users", "Account");
        }

        public IActionResult UserRemove(string id)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = client.DeleteAsync(_apiBaseUrl + $"/{id}").Result;

            if (response.IsSuccessStatusCode)
                TempData["DeleteSuccess"] = $"{id} nolu kayıt silindi";
            else
                TempData["DeleteFail"] = $"{id} nolu kayıt silinemedi";

            return RedirectToAction("Users", "Account");
        }

        public IActionResult UserRoles(string id)
        {
            var client = _httpClientFactory.CreateClient();
            UserWithRolesViewModel model = client.GetFromJsonAsync<UserWithRolesViewModel>(_apiBaseUrl + $"/{id}/Roles").Result;

            return View(model);
        }

        public IActionResult UserAddRole(string id)
        {
            var client = _httpClientFactory.CreateClient();

            List<RoleUpdateViewModel> roleList = client.GetFromJsonAsync<List<RoleUpdateViewModel>>(_apiroleBaseUrl).Result;

            List<SelectListItem> roles = new List<SelectListItem>();

            foreach (RoleUpdateViewModel item in roleList)
            {
                roles.Add(new SelectListItem() { Text = item.Name, Value = item.Id });
            }

            ViewBag.Roles = roles;

            UserRoleViewModel model = new UserRoleViewModel();
            model.UserId = id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserAddRole(UserRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage responseMessage = client.PostAsync(_apiBaseUrl + $"/{model.UserId}/Roles/{model.RoleId}", null).Result;

            if (!responseMessage.IsSuccessStatusCode)
            {
                ModelState.AddModelError("ServiceError", responseMessage.RequestMessage.Content.ReadAsStringAsync().Result);
                return View(model);
            }
            //UserRoles/00784676-6323-463f-90c9-1781f932efe5
            return RedirectToAction("UserRoles", "Account", new { id = model.UserId });
        }
        public IActionResult UserRemoveRole(string id, string roleId)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = client.DeleteAsync(_apiBaseUrl + $"/{id}/Roles/{roleId}").Result;

            if (response.IsSuccessStatusCode)
                TempData["DeleteSuccess"] = $"{roleId} nolu rol silindi";
            else
                TempData["DeleteFail"] = $"{roleId} nolu rol silinemedi";

            return RedirectToAction("UserRoles", "Account", new { id });
        }

        public async Task<IActionResult> Roles()
        {
            // IHttpClientFactory kullanarak HttpClient'ı oluşturun
            var client = _httpClientFactory.CreateClient();

            // HttpClient'ı kullanarak JSON verisini API'den alıyoruz
            List<RoleUpdateViewModel> model = await client.GetFromJsonAsync<List<RoleUpdateViewModel>>(_apiroleBaseUrl);

            // Verileri view'a gönderiyoruz
            return View(model);
        }

        public IActionResult RoleAdd()
        {
            RoleCreateViewModel model = new RoleCreateViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleAdd(RoleCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage responseMessage = client.PostAsJsonAsync(_apiroleBaseUrl, model).Result;

            if (!responseMessage.IsSuccessStatusCode)
            {
                ModelState.AddModelError("ServiceError", responseMessage.RequestMessage.Content.ReadAsStringAsync().Result);
                return View(model);
            }

            return RedirectToAction("Roles", "Account");
        }

        public IActionResult RoleEdit(string id)
        {
            var client = _httpClientFactory.CreateClient();

            RoleUpdateViewModel model = client.GetFromJsonAsync<RoleUpdateViewModel>(_apiroleBaseUrl + $"/{id}").Result;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleEdit(RoleUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var client = _httpClientFactory.CreateClient();

            HttpResponseMessage responseMessage = client.PutAsJsonAsync(_apiroleBaseUrl + $"/{model.Id}", model).Result;

            if (!responseMessage.IsSuccessStatusCode)
            {
                ModelState.AddModelError("ServiceError", responseMessage.RequestMessage.Content.ReadAsStringAsync().Result);
                return View(model);
            }

            return RedirectToAction("Roles", "Account");
        }

        public IActionResult RoleRemove(string id)
        {
            var client = _httpClientFactory.CreateClient();

            HttpResponseMessage response = client.DeleteAsync(_apiroleBaseUrl + $"/{id}").Result;

            if (response.IsSuccessStatusCode)
                TempData["DeleteSuccess"] = $"{id} nolu kayıt silindi";
            else
                TempData["DeleteFail"] = $"{id} nolu kayıt silinemedi";

            return RedirectToAction("Roles", "Account");
        }

        public IActionResult RoleUsers(string id)
        {
            var client = _httpClientFactory.CreateClient();

            RoleUpdateViewModel role = client.GetFromJsonAsync<RoleUpdateViewModel>(_apiroleBaseUrl + $"/{id}").Result;

            RoleWithUsersViewModel model = new RoleWithUsersViewModel();
            model.Id = role.Id;
            model.Name = role.Name;
            model.Users = client.GetFromJsonAsync<List<UserResponseViewModel>>(_apiroleBaseUrl + $"/{id}/Users").Result;

            return View(model);
        }

    }
}

