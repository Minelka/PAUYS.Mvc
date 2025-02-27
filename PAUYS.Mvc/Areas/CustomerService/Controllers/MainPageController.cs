using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;

namespace PAUYS.Mvc.Areas.CustomerService.Controllers
{
    [Area("CustomerService")]
    [Authorize(Policy = "CustomerPolicy")]
    public class MainPageController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}CustomerRequest";


        public MainPageController(IHttpClientFactory httpClientFactory)
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

