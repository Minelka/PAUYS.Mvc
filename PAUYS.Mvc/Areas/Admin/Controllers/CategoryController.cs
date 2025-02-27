using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")] // AdminPolicy politikasını uygula
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
            var categoriesDto = await GetAllCategories();  // API'den DTO'yu alıyoruz
            var categoriesViewModel = categoriesDto
                .Where(m => !m.IsDeleted)  // Silinmiş olmayanları filtreliyoruz
                .Select(m => new CategoryViewModel  // DTO'dan ViewModel'e dönüşüm
                {
                    Id = m.Id,
                    Name = m.Name,
                    Shape = m.Shape,
                    Description = m.Description,
                    UsingArea = m.UsingArea
                }).ToList();

            return View(categoriesViewModel);  // ViewModel ile View'a veri gönderiyoruz
        }

        [HttpGet]
        // Controller'da view'a model gönderildiğinden emin olun
        public IActionResult Add()
        {
            return View(new CategoryViewModel());  // Yeni bir CategoryViewModel nesnesi gönderiyoruz
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryViewModel category)
        {
            if (!ModelState.IsValid)
            {
                // Eğer model geçerli değilse, formu tekrar gösteriyoruz
                return View(category);
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kategori başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Kategori eklenirken bir hata oluştu.";
                return View(category);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var category = await GetCategoryById(id); // Kategori bilgilerini API'den alıyoruz

            if (category == null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li kategori bulunamadığı için güncelleme yapılamıyor.";
                return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
            }

            return View(category); // Kategori bilgisiyle güncelleme formunu render ediyoruz
        }

        // Kategori Güncelleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryViewModel category)
        {
            if (!ModelState.IsValid)
            {
                return View(category); // Eğer model geçerli değilse formu tekrar gösteriyoruz
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiBaseUrl}/{category.Id}", content); // Güncellenen veriyi API'ye gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kategori güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
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

        // Kategori Silme
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kategori silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
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
