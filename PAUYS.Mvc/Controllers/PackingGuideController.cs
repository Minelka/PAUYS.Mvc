using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Controllers
{
    public class PackingGuideController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}PackingGuide";

        public PackingGuideController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var packingGuidesDto = await GetAllPackingGuides();  // API'den DTO'yu alıyoruz
            var packingGuidesViewModel = packingGuidesDto
                .Where(m => !m.IsDeleted)  // Silinmiş olmayanları filtreliyoruz
                .Select(m => new PackingGuideViewModel  // DTO'dan ViewModel'e dönüşüm
                {
                    Id = m.Id,
                    Confirmation = m.Confirmation,
                    Sample=m.Sample,
                    MoldProduction=m.MoldProduction,
                    Finalization=m.Finalization
                    
                }).ToList();

            return View(packingGuidesViewModel);  // ViewModel ile View'a veri gönderiyoruz
        }



        // Get Material by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            id = 1;

            PackingGuideViewModel? packingGuide = await GetPackingGuideById(id); // GetMaterialById'yi kullanarak API'den veri alıyoruz.

            if (packingGuide is null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li kayıt bulunamadığı için düzenleme yapılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            return View(packingGuide);
        }


        // API'den veri alma
        private async Task<List<PackingGuideViewModel>> GetAllPackingGuides()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PackingGuideViewModel>>(content);
            }

            return new List<PackingGuideViewModel>();  // Eğer hata varsa boş liste döndürüyoruz
        }

        private async Task<PackingGuideViewModel?> GetPackingGuideById(int id)
        {
            var client = _httpClientFactory.CreateClient(); // HttpClient nesnesi oluşturuyoruz
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}"); // ID'yi URL'ye ekleyerek GET isteği gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API'den dönen JSON'u ViewModel'e dönüştürme
                return JsonConvert.DeserializeObject<PackingGuideViewModel>(content);
            }

            return null; // Eğer hata varsa null döndürüyoruz
        }
    }
}

