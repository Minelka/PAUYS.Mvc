using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;


namespace PAUYS.Mvc.Controllers
{
    public class CategoryController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Category";

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var categoriesViewModel = await GetAllCategories();  // API'den DTO'yu alıyoruz
            categoriesViewModel
                .Where(m => !m.IsDeleted)  // Silinmiş olmayanları filtreliyoruz
                .Select(m => new CategoryViewModel  // DTO'dan ViewModel'e dönüşüm
                {
                    Id = m.Id,
                    Name = m.Name,
                    Shape= m.Shape,
                    Description= m.Description,
                    UsingArea= m.UsingArea
                }).ToList();

            return View(categoriesViewModel);  // ViewModel ile View'a veri gönderiyoruz
        }

        // Get Material by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            CategoryViewModel? category = await GetCategoryById(id); // GetMaterialById'yi kullanarak API'den veri alıyoruz.

            if (category is null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li kayıt bulunamadığı için düzenleme yapılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
        // API'den veri alma
        private async Task<List<CategoryViewModel>> GetAllCategories()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CategoryViewModel>>(content);
            }

            return new List<CategoryViewModel>();  // Eğer hata varsa boş liste döndürüyoruz
        }

        private async Task<CategoryViewModel?> GetCategoryById(int id)
        {
            var client = _httpClientFactory.CreateClient(); // HttpClient nesnesi oluşturuyoruz
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}"); // ID'yi URL'ye ekleyerek GET isteği gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API'den dönen JSON'u ViewModel'e dönüştürme
                return JsonConvert.DeserializeObject<CategoryViewModel>(content);
            }

            return null; // Eğer hata varsa null döndürüyoruz
        }
    }
}
