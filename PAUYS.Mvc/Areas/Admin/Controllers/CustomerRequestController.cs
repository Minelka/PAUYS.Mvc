using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class CustomerRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}CustomerRequest";

        public CustomerRequestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var customerRequestsDto = await GetAllCustomerRequests();  // API'den DTO'yu alıyoruz
            var customerRequestsViewModel = customerRequestsDto
                .Where(c => !c.IsDeleted)  // Silinmiş olmayanları filtreliyoruz
                .Select(c => new CustomerRequestViewModel  // DTO'dan ViewModel'e dönüşüm
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    CustomerMessage = c.CustomerMessage,
                    AdminMessage = c.AdminMessage,
                    RefundorNewRequest = c.RefundorNewRequest
                }).ToList();

            return View(customerRequestsViewModel);  // ViewModel ile View'a veri gönderiyoruz
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var customerRequest = await GetCustomerRequestById(id); // Kategori bilgilerini API'den alıyoruz

            if (customerRequest == null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li müşteri isteği bulunamadığı için güncelleme yapılamıyor.";
                return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
            }

            return View(customerRequest); // Kategori bilgisiyle güncelleme formunu render ediyoruz
        }

        // Kategori Güncelleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CustomerRequestViewModel customerRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(customerRequest); // Eğer model geçerli değilse formu tekrar gösteriyoruz
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(customerRequest), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiBaseUrl}/{customerRequest.Id}", content); // Güncellenen veriyi API'ye gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Müşteri isteği başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Müşteri isteği güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
        }

        // Get Material by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            CustomerRequestViewModel? customerRequest = await GetCustomerRequestById(id); // GetMaterialById'yi kullanarak API'den veri alıyoruz.

            if (customerRequest is null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li kayıt bulunamadığı için düzenleme yapılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            return View(customerRequest);
        }

        // Kategori Silme
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Müşteri isteği başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Müşteri isteği silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
        }


        // API'den veri alma
        private async Task<List<CustomerRequestViewModel>> GetAllCustomerRequests()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CustomerRequestViewModel>>(content);
            }

            return new List<CustomerRequestViewModel>();  // Eğer hata varsa boş liste döndürüyoruz
        }

        private async Task<CustomerRequestViewModel?> GetCustomerRequestById(int id)
        {
            var client = _httpClientFactory.CreateClient(); // HttpClient nesnesi oluşturuyoruz
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}"); // ID'yi URL'ye ekleyerek GET isteği gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API'den dönen JSON'u ViewModel'e dönüştürme
                return JsonConvert.DeserializeObject<CustomerRequestViewModel>(content);
            }

            return null; // Eğer hata varsa null döndürüyoruz
        }
    }
}
