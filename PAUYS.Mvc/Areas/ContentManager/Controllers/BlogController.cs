using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Net.Http;
using System.Text;

namespace PAUYS.Mvc.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize(Policy = "ContentPolicy")]
    public class BlogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Blog";
        public BlogController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            // Ürünleri API'den çekiyoruz
            var blogDto = await GetAllBlogs();
            var blogsViewModel = blogDto
                .Where(m => !m.IsDeleted) // Silinmiş olmayanları filtreliyoruz
                .Select(m => new BlogViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Text = m.Text,
                    PictureFileName = m.PictureFileName,
                    Picture = m.Picture
                }).ToList();

            return View(blogsViewModel); // ViewModel ile View'a veri gönderiyoruz
        }

        // Get Material by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // Ürünleri API'den çekiyoruz ve ilgili ürünü buluyoruz
            var blogsDto = await GetAllBlogs();
            var blogDto = blogsDto.FirstOrDefault(p => p.Id == id && !p.IsDeleted);

            if (blogDto == null)
            {
                return NotFound(); // Ürün bulunamazsa 404 döneriz
            }

            // Ürün ViewModel'ini oluşturuyoruz
            var blogViewModel = new BlogViewModel
            {
                Id = blogDto.Id,
                Title = blogDto.Title,
                Text = blogDto.Text,
                Picture = blogDto.Picture,
                PictureFileName = blogDto.PictureFileName,
            };

            return View(blogViewModel); // Detay View'ına ViewModel gönderiyoruz
        }

        [HttpGet]
        // Controller'da view'a model gönderildiğinden emin olun
        public IActionResult Add()
        {
            return View(new BlogAddViewModel());  // Yeni bir CategoryViewModel nesnesi gönderiyoruz
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BlogAddViewModel blog)
        {
            if (!ModelState.IsValid)
            {
                // Eğer model geçerli değilse, formu tekrar gösteriyoruz
                return View(blog);
            }

            blog.ConvertPicture();

            var clientForPost = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8, "application/json");

            var responsePost2 = await clientForPost.PostAsync(_apiBaseUrl, content);

            BlogViewModel blogViewModel = new BlogViewModel();
            blog.Status = true;
            blogViewModel.Status = blog.Status;
            blogViewModel.Title = blog.Title;
            blogViewModel.Text = blog.Text;
            blogViewModel.Picture = blog.Picture;
            blogViewModel.PictureFileName = blog.PictureFileName;


            var blogJson = System.Text.Json.JsonSerializer.Serialize(blogViewModel);

            var HttpClient = new HttpClient();

            var stringUrl = Environment.GetEnvironmentVariable("webapi-service-url")!;

            HttpClient.BaseAddress = new Uri(stringUrl);

            var responsePost = await HttpClient.PostAsJsonAsync("Blog", blogViewModel);

            if (responsePost.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Blog başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Blog eklenirken bir hata oluştu.";

                // Hata durumunda materyalleri yeniden yükleyerek formu geri döndürüyoruz

                return View(blog);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // Ürünü API'den çekiyoruz
            var blogResponse = await client.GetAsync($"{_apiBaseUrl}/{id}");
            if (!blogResponse.IsSuccessStatusCode)
            {
                TempData["RecordNotFounded"] = $"Id: {id} li blog bulunamadığı için güncelleme yapılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            var blogJson = await blogResponse.Content.ReadAsStringAsync();
            var blog = JsonConvert.DeserializeObject<BlogAddViewModel>(blogJson);

            return View(blog);
        }

        // Kategori Güncelleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BlogViewModel blog)
        {
            
            // Eğer yeni bir fotoğraf yüklenmişse, fotoğrafı güncelle
            if (blog.PictureFormFile != null && blog.PictureFormFile.Length > 0)
            {
                
                
                    using (var memoryStream = new MemoryStream())
                    {
                        blog.PictureFormFile.CopyTo(memoryStream);
                        blog.Picture = memoryStream.ToArray();
                        blog.PictureFileName = blog.PictureFormFile.FileName;
                    }
                                
            }
            else
            {
                // Yeni fotoğraf yüklenmemişse mevcut fotoğrafı koru
                var existingBlogResponse = await GetBlogById(blog.Id);
                if (existingBlogResponse != null)
                {
                    blog.Picture = existingBlogResponse.Picture;
                    blog.PictureFileName = existingBlogResponse.PictureFileName;

                    // Mevcut fotoğrafı Base64 formatına çevirip kontrol ediyoruz
                    if (blog.Picture != null)
                    {
                        var pictureBase64 = Convert.ToBase64String(blog.Picture);
                        // Opsiyonel: Base64'ü loglayabilir veya başka bir işleme sokabilirsiniz
                        Console.WriteLine($"Mevcut resim Base64 formatı: {pictureBase64}");
                    }
                }
            }

            // Update için ProductViewModel nesnesi oluştur
            var updatedBlog = new BlogViewModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Text = blog.Text,
                Picture = blog.Picture,
                PictureFileName = blog.PictureFileName,
                Status = blog.Status
            };

            // API'ye PUT isteği gönder
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(updatedBlog), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/{blog.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Blog başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Ürün güncellenirken bir hata oluştu.";

                return View(blog);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Blog başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Blog silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        //[HttpPost, ActionName("Delete")]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View();
        //}

        private async Task<List<BlogViewModel>> GetAllBlogs()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<BlogViewModel>>(content);
            }

            return new List<BlogViewModel>();
        }

        private async Task<BlogViewModel?> GetBlogById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BlogViewModel>(content);
            }

            return null;
        }

    }
}