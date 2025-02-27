using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Net.Http;
using System.Text;

namespace PAUYS.Mvc.Controllers
{
    public class NewsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}News";

        public NewsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            // Ürünleri API'den çekiyoruz
            var newsDto = await GetAllNews();
            var newsViewModel = newsDto
                .Where(m => !m.IsDeleted) // Silinmiş olmayanları filtreliyoruz
                .Select(m => new NewsViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Text = m.Text,
                    PictureFileName = m.PictureFileName,
                    Picture = m.Picture
                }).ToList();

            return View(newsViewModel); // ViewModel ile View'a veri gönderiyoruz
        }

        // Get Material by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // Ürünleri API'den çekiyoruz ve ilgili ürünü buluyoruz
            var newssDto = await GetAllNews();
            var newsDto = newssDto.FirstOrDefault(p => p.Id == id && !p.IsDeleted);

            if (newsDto == null)
            {
                return NotFound(); // Ürün bulunamazsa 404 döneriz
            }

            // Ürün ViewModel'ini oluşturuyoruz
            var newsViewModel = new NewsViewModel
            {
                Id = newsDto.Id,
                Title = newsDto.Title,
                Text = newsDto.Text,
                Picture = newsDto.Picture,
                PictureFileName = newsDto.PictureFileName,
            };

            return View(newsViewModel); // Detay View'ına ViewModel gönderiyoruz
        }

        private async Task<List<NewsViewModel>> GetAllNews()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<NewsViewModel>>(content);
            }

            return new List<NewsViewModel>();
        }

        private async Task<NewsViewModel?> GetNewsById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<NewsViewModel>(content);
            }

            return null;
        }

    }
}