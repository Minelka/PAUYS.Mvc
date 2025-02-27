using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize(Policy = "ContentPolicy")]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Product";
        private readonly string _apiBaseUrl2 = Environment.GetEnvironmentVariable("webapi-service-url")!;

        public ProductController(IHttpClientFactory httpClientFactory)
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



        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // Materyalleri API'den çekiyoruz
            var response = await client.GetAsync($"{_apiBaseUrl2}Material");
            if (response.IsSuccessStatusCode)
            {
                var materialsJson = await response.Content.ReadAsStringAsync();
                var materials = JsonConvert.DeserializeObject<List<MaterialViewModel>>(materialsJson);
                ViewBag.Materials = materials;
            }
            else
            {
                ViewBag.Materials = new List<MaterialViewModel>();
            }

            // Kategorileri API'den çekiyoruz
            var response2 = await client.GetAsync($"{_apiBaseUrl2}Category");
            if (response2.IsSuccessStatusCode)
            {
                var categoriesJson = await response2.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoriesJson);
                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<CategoryViewModel>();
            }

            // Ürünü API'den çekiyoruz
            var productResponse = await client.GetAsync($"{_apiBaseUrl}/{id}");
            if (!productResponse.IsSuccessStatusCode)
            {
                TempData["RecordNotFounded"] = $"Id: {id} li ürün bulunamadığı için güncelleme yapılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            var productJson = await productResponse.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductAddViewModel>(productJson);

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductAddViewModel product)
        {
            if (!ModelState.IsValid)
            {
                // Materyalleri ve kategorileri yeniden yükle
                await LoadMaterialsToViewBag();
                await LoadCategoriesToViewBag();
                return View(product);
            }

            // Eğer yeni bir fotoğraf yüklenmişse, fotoğrafı güncelle
            if (product.PictureFormFile != null && product.PictureFormFile.Length > 0)
            {
                product.ConvertPicture();
            }
            else
            {
                // Yeni fotoğraf yüklenmemişse mevcut fotoğrafı koru
                var existingProductResponse = await GetProductById(product.Id);
                if (existingProductResponse != null)
                {
                    product.Picture = existingProductResponse.Picture;
                    product.PictureFileName = existingProductResponse.PictureFileName;

                    // Mevcut fotoğrafı Base64 formatına çevirip kontrol ediyoruz
                    if (product.Picture != null)
                    {
                        var pictureBase64 = Convert.ToBase64String(product.Picture);
                        // Opsiyonel: Base64'ü loglayabilir veya başka bir işleme sokabilirsiniz
                        Console.WriteLine($"Mevcut resim Base64 formatı: {pictureBase64}");
                    }
                }
            }

            // Update için ProductViewModel nesnesi oluştur
            var updatedProduct = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Picture = product.Picture,
                PictureFileName = product.PictureFileName,
                MaterialId = product.MaterialId,
                CategoryId = product.CategoryId,
                Status = product.Status
            };

            // API'ye PUT isteği gönder
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(updatedProduct), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/{product.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Ürün güncellenirken bir hata oluştu.";
                await LoadMaterialsToViewBag();
                await LoadCategoriesToViewBag();
                return View(product);
            }
        }




        private async Task LoadMaterialsToViewBag()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl2}Material");
            if (response.IsSuccessStatusCode)
            {
                var materialsJson = await response.Content.ReadAsStringAsync();
                var materials = JsonConvert.DeserializeObject<List<MaterialViewModel>>(materialsJson);
                ViewBag.Materials = materials;
            }
            else
            {
                ViewBag.Materials = new List<MaterialViewModel>();
            }
        }


        private async Task LoadCategoriesToViewBag()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl2}Category");
            if (response.IsSuccessStatusCode)
            {
                var categoriesJson = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoriesJson);
                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<CategoryViewModel>();
            }
        }



        [HttpGet]
        public async Task<IActionResult> Detail(int id)
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
            var client2 = _httpClientFactory.CreateClient();

            // Kategorileri API'den çekiyoruz
            var response2 = await client2.GetAsync($"{_apiBaseUrl2}Category");
            List<CategoryViewModel> categories = new();
            if (response2.IsSuccessStatusCode)
            {
                var categoriesJson = await response2.Content.ReadAsStringAsync();
                categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoriesJson) ?? new List<CategoryViewModel>();
            }

            // Ürünleri API'den çekiyoruz ve ilgili ürünü buluyoruz
            var productsDto = await GetAllProducts();
            var productDto = productsDto.FirstOrDefault(p => p.Id == id && !p.IsDeleted);

            if (productDto == null)
            {
                return NotFound(); // Ürün bulunamazsa 404 döneriz
            }

            // Ürün ViewModel'ini oluşturuyoruz
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

            return View(productViewModel); // Detay View'ına ViewModel gönderiyoruz

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

            return new List<ProductViewModel>();  // Eğer hata varsa boş liste döndürüyoruz
        }

        private async Task<ProductViewModel?> GetProductById(int id)
        {
            var client = _httpClientFactory.CreateClient(); // HttpClient nesnesi oluşturuyoruz
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}"); // ID'yi URL'ye ekleyerek GET isteği gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API'den dönen JSON'u ViewModel'e dönüştürme
                return JsonConvert.DeserializeObject<ProductViewModel>(content);
            }

            return null; // Eğer hata varsa null döndürüyoruz
        }

    }
}
