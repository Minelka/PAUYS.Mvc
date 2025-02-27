using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize(Policy = "ContentPolicy")]
    public class MainPageController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Product";
        private readonly string _apiBaseUrl2 = Environment.GetEnvironmentVariable("webapi-service-url")!;

        public MainPageController(IHttpClientFactory httpClientFactory)
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





        //private ProductViewModel GetSampleProduct(int id, string name, string description, decimal price, string pictureFileName, string materialName, List<CategoryViewModel> categories)
        //{
        //    return new ProductViewModel
        //    {
        //        Name = name,
        //        Description = description,
        //        Price = price,
        //        PictureFileName = pictureFileName,
        //        MaterialViewModel = new MaterialViewModel
        //        {
        //            Id = id,
        //            Name = materialName
        //        },
        //        CategoryViewModels = categories,
        //        Id = id
        //    };
        //}

        //public IActionResult Index()
        //{
        //    var product = GetSampleProduct(
        //        id: 1,
        //        name: "Ürün",
        //        description: "Ürün açıklaması burada yer alır.",
        //        price: 100.0m,
        //        pictureFileName: "resim.jpg",
        //        materialName: "Materyal Adı",
        //        categories: new List<CategoryViewModel>
        //        {
        //            new CategoryViewModel { Id = 1, Name = "Kategori 1" },
        //            new CategoryViewModel { Id = 2, Name = "Kategori 2" }
        //        }
        //    );

        //    return View(product);
        //}

        //public IActionResult Details(int id)
        //{
        //    var product = GetSampleProduct(
        //        id: id,
        //        name: "Detay Ürün",
        //        description: "Bu detaylı açıklama alanıdır.",
        //        price: 150.0m,
        //        pictureFileName: "detay_resim.jpg",
        //        materialName: "Detay Materyal Adı",
        //        categories: new List<CategoryViewModel>
        //        {
        //            new CategoryViewModel { Id = 3, Name = "Kategori 3" },
        //            new CategoryViewModel { Id = 4, Name = "Kategori 4" }
        //        }
        //    );

        //    return View(product);
        //}

        //public IActionResult ShowPopup(int id)
        //{
        //    var product = GetSampleProduct(
        //        id: id,
        //        name: "Popup Ürün",
        //        description: "Popup ürün detay açıklaması.",
        //        price: 120.0m,
        //        pictureFileName: "popup_resim.jpg",
        //        materialName: "Popup Materyal Adı",
        //        categories: new List<CategoryViewModel>
        //        {
        //            new CategoryViewModel { Id = 5, Name = "Kategori 5" },
        //            new CategoryViewModel { Id = 6, Name = "Kategori 6" }
        //        }
        //    );

        //    return PartialView("_ProductPopup", product);
        //}
    }
}

