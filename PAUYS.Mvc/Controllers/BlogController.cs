using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Net.Http;
using System.Text;

namespace PAUYS.Mvc.Controllers
{
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