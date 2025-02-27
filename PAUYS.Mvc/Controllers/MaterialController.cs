using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;


namespace PAUYS.Mvc.Controllers
{
    public class MaterialController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Material";

        public MaterialController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var materialsDto = await GetAllMaterials();  // API'den DTO'yu alıyoruz
            var materialsViewModel = materialsDto
                .Where(m => !m.IsDeleted)  // Silinmiş olmayanları filtreliyoruz
                .Select(m => new MaterialViewModel  // DTO'dan ViewModel'e dönüşüm
                {
                    Id = m.Id,
                    Name = m.Name
                }).ToList();

            return View(materialsViewModel);  // ViewModel ile View'a veri gönderiyoruz
        }

        // Get Material by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            MaterialViewModel? material = await GetMaterialById(id); // GetMaterialById'yi kullanarak API'den veri alıyoruz.

            if (material is null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li kayıt bulunamadığı için düzenleme yapılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            return View(material);
        }
        // API'den veri alma
        private async Task<List<MaterialViewModel>> GetAllMaterials()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MaterialViewModel>>(content);
            }

            return new List<MaterialViewModel>();  // Eğer hata varsa boş liste döndürüyoruz
        }

        private async Task<MaterialViewModel?> GetMaterialById(int id)
        {
            var client = _httpClientFactory.CreateClient(); // HttpClient nesnesi oluşturuyoruz
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}"); // ID'yi URL'ye ekleyerek GET isteği gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API'den dönen JSON'u ViewModel'e dönüştürme
                return JsonConvert.DeserializeObject<MaterialViewModel>(content);
            }

            return null; // Eğer hata varsa null döndürüyoruz
        }
    }
}
