using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
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
                    Name = m.Name,
                }).ToList();

            return View(materialsViewModel);  // ViewModel ile View'a veri gönderiyoruz
        }

        [HttpGet]
        // Controller'da view'a model gönderildiğinden emin olun
        public IActionResult Add()
        {
            return View(new MaterialViewModel());  // Yeni bir CategoryViewModel nesnesi gönderiyoruz
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MaterialViewModel material)
        {
            if (!ModelState.IsValid)
            {
                // Eğer model geçerli değilse, formu tekrar gösteriyoruz
                return View(material);
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(material), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Materyal başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Materyal eklenirken bir hata oluştu.";
                return View(material);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var material = await GetMaterialById(id); // Kategori bilgilerini API'den alıyoruz

            if (material == null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li kategori bulunamadığı için güncelleme yapılamıyor.";
                return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
            }

            return View(material); // Kategori bilgisiyle güncelleme formunu render ediyoruz
        }

        // Kategori Güncelleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(MaterialViewModel material)
        {
            if (!ModelState.IsValid)
            {
                return View(material); // Eğer model geçerli değilse formu tekrar gösteriyoruz
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(material), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiBaseUrl}/{material.Id}", content); // Güncellenen veriyi API'ye gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Materyal başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Materyal güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
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

        // Kategori Silme
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Materyal başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Materyal silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
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
