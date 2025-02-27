using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Controllers
{
    public class CustomerRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}CustomerRequest";

        public CustomerRequestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        // Controller'da view'a model gönderildiğinden emin olun
        public IActionResult Add()
        {
            return View(new CustomerRequestViewModel());  // Yeni bir CategoryViewModel nesnesi gönderiyoruz
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CustomerRequestViewModel customerRequest)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Eğer model geçerli değilse, formu tekrar gösteriyoruz
            //    return View(customerRequest);
            //}
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model doğrulama hatası: Eksik veya hatalı bilgiler var.";
                return View(customerRequest);
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(customerRequest), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Müşteri isteği başarıyla eklendi.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Müşteri isteği eklenirken bir hata oluştu.";
                return View(customerRequest);
            }
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
