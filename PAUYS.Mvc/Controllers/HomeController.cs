using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.Mvc.Models;
using PAUYS.ViewModel.Concrete;
using System.Diagnostics;

namespace PAUYS.Mvc.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
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

            // Materyalleri API'den �ekiyoruz
            var response = await client.GetAsync($"{_apiBaseUrl2}Material");
            List<MaterialViewModel> materials = new();
            if (response.IsSuccessStatusCode)
            {
                var materialsJson = await response.Content.ReadAsStringAsync();
                materials = JsonConvert.DeserializeObject<List<MaterialViewModel>>(materialsJson) ?? new List<MaterialViewModel>();
            }
            // Kategorileri API'den �ekiyoruz
            var response2 = await client.GetAsync($"{_apiBaseUrl2}Category");
            List<CategoryViewModel> categories = new();
            if (response2.IsSuccessStatusCode)
            {
                var categoriesJson = await response2.Content.ReadAsStringAsync();
                categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoriesJson) ?? new List<CategoryViewModel>();
            }

            // �r�nleri API'den �ekiyoruz
            var productsDto = await GetAllProducts();
            var productsViewModel = productsDto
                .Where(m => !m.IsDeleted) // Silinmi� olmayanlar� filtreliyoruz
                .Select(m => new ProductViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    PictureFileName = m.PictureFileName,
                    Picture = m.Picture,
                    MaterialId = m.MaterialId,
                    MaterialViewModel = materials.FirstOrDefault(mat => mat.Id == m.MaterialId), // Materyali e�le�tiriyoruz
                                                                                                 // matName = materials.FirstOrDefault(mat => mat.Id == m.MaterialId)?.Name // Materyal ad�n� at�yoruz
                    CategoryId = m.CategoryId,
                    CategoryViewModel = categories.FirstOrDefault(mat => mat.Id == m.CategoryId)
                }).ToList();

            return View(productsViewModel); // ViewModel ile View'a veri g�nderiyoruz
        }

        // Get �r�n by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {


            var client = _httpClientFactory.CreateClient();

            // Materyalleri API'den �ekiyoruz
            var response = await client.GetAsync($"{_apiBaseUrl2}Material");
            List<MaterialViewModel> materials = new();
            if (response.IsSuccessStatusCode)
            {
                var materialsJson = await response.Content.ReadAsStringAsync();
                materials = JsonConvert.DeserializeObject<List<MaterialViewModel>>(materialsJson) ?? new List<MaterialViewModel>();
            }
            var client2 = _httpClientFactory.CreateClient();

            // Kategorileri API'den �ekiyoruz
            var response2 = await client2.GetAsync($"{_apiBaseUrl2}Category");
            List<CategoryViewModel> categories = new();
            if (response2.IsSuccessStatusCode)
            {
                var categoriesJson = await response2.Content.ReadAsStringAsync();
                categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoriesJson) ?? new List<CategoryViewModel>();
            }

            // �r�nleri API'den �ekiyoruz ve ilgili �r�n� buluyoruz
            var productsDto = await GetAllProducts();
            var productDto = productsDto.FirstOrDefault(p => p.Id == id && !p.IsDeleted);

            if (productDto == null)
            {
                return NotFound(); // �r�n bulunamazsa 404 d�neriz
            }

            // �r�n ViewModel'ini olu�turuyoruz
            var productViewModel = new ProductViewModel
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Picture = productDto.Picture,
                PictureFileName = productDto.PictureFileName,
                MaterialId = productDto.MaterialId,
                MaterialViewModel = materials.FirstOrDefault(mat => mat.Id == productDto.MaterialId),
                // MadedMaterialName = materials.FirstOrDefault(mat => mat.Id == productDto.MaterialId)?.Name
                CategoryId = productDto.CategoryId,
                CategoryViewModel = categories.FirstOrDefault(mat => mat.Id == productDto.CategoryId)
            };

            return View(productViewModel); // Detay View'�na ViewModel g�nderiyoruz

        }






        // API'den veri alma
        private async Task<List<ProductViewModel>> GetAllProducts()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProductViewModel>>(content);
            }

            return new List<ProductViewModel>();  // E�er hata varsa bo� liste d�nd�r�yoruz
        }


        private async Task<ProductViewModel?> GetProductById(int id)
        {
            var client = _httpClientFactory.CreateClient(); // HttpClient nesnesi olu�turuyoruz
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}"); // ID'yi URL'ye ekleyerek GET iste�i g�nderiyoruz

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API'den d�nen JSON'u ViewModel'e d�n��t�rme
                return JsonConvert.DeserializeObject<ProductViewModel>(content);
            }

            return null; // E�er hata varsa null d�nd�r�yoruz
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
