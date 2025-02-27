using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;

namespace PAUYS.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Product";
        private readonly string _apiBaseUrl2 = Environment.GetEnvironmentVariable("webapi-service-url")!;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            // Materyalleri API'den çekiyoruz
            var response = await client.GetAsync($"{_apiBaseUrl2}Material");
            List<MaterialViewModel> materials = new();
            if (response.IsSuccessStatusCode)
            {
                var materialsJson = await response.Content.ReadAsStringAsync();
                materials = JsonConvert.DeserializeObject<List<MaterialViewModel>>(materialsJson) ?? new List<MaterialViewModel>();
            }
            // Kategorileri API'den çekiyoruz
            var response2 = await client.GetAsync($"{_apiBaseUrl2}Category");
            List<CategoryViewModel> categories = new();
            if (response2.IsSuccessStatusCode)
            {
                var categoriesJson = await response2.Content.ReadAsStringAsync();
                categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoriesJson) ?? new List<CategoryViewModel>();
            }

            // Ürünleri API'den çekiyoruz
            var productsDto = await GetAllProducts();
            var productsViewModel = productsDto
                .Where(m => !m.IsDeleted) // Silinmiş olmayanları filtreliyoruz
                .Select(m => new ProductViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    PictureFileName = m.PictureFileName,
                    Picture = m.Picture,
                    MaterialId = m.MaterialId,
                    MaterialViewModel = materials.FirstOrDefault(mat => mat.Id == m.MaterialId), // Materyali eşleştiriyoruz
                                                                                                 // matName = materials.FirstOrDefault(mat => mat.Id == m.MaterialId)?.Name // Materyal adını atıyoruz
                    CategoryId = m.CategoryId,
                    CategoryViewModel = categories.FirstOrDefault(mat => mat.Id == m.CategoryId)
                }).ToList();

            return View(productsViewModel); // ViewModel ile View'a veri gönderiyoruz
        }

        private async Task<List<ProductViewModel>> GetAllProducts()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProductViewModel>>(content);
            }

            return new List<ProductViewModel>();  // Eğer hata varsa boş liste döndürüyoruz
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
