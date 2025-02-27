using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
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


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            var packingGuide = await GetPackingGuideById(id); // Kategori bilgilerini API'den alıyoruz

            if (packingGuide == null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li bulunamadığı için güncelleme yapılamıyor.";
                return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
            }

            return View(packingGuide); // Kategori bilgisiyle güncelleme formunu render ediyoruz
        }

        // Kategori Güncelleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PackingGuideViewModel packingGuide)
        {
            if (!ModelState.IsValid)
            {
                return View(packingGuide); // Eğer model geçerli değilse formu tekrar gösteriyoruz
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(packingGuide), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiBaseUrl}/{packingGuide.Id}", content); // Güncellenen veriyi API'ye gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
        }

        //[HttpGet]
        //// Controller'da view'a model gönderildiğinden emin olun
        //public IActionResult Add()
        //{
        //    return View(new PackingGuideViewModel());  // Yeni bir CategoryViewModel nesnesi gönderiyoruz
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Add(PackingGuideViewModel material)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Eğer model geçerli değilse, formu tekrar gösteriyoruz
        //        return View(material);
        //    }

        //    var client = _httpClientFactory.CreateClient();
        //    var content = new StringContent(JsonConvert.SerializeObject(material), Encoding.UTF8, "application/json");

        //    var response = await client.PostAsync(_apiBaseUrl, content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        TempData["SuccessMessage"] = "Ambalaj Rehberi başarıyla eklendi.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Ambalaj Rehberi eklenirken bir hata oluştu.";
        //        return View(material);
        //    }
        //}

        // Get Material by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
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

