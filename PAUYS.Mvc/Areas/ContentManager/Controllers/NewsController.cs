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

        [HttpGet]
        // Controller'da view'a model gönderildiğinden emin olun
        public IActionResult Add()
        {
            return View(new NewsAddViewModel());  // Yeni bir CategoryViewModel nesnesi gönderiyoruz
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(NewsAddViewModel news)
        {
            if (!ModelState.IsValid)
            {
                // Eğer model geçerli değilse, formu tekrar gösteriyoruz
                return View(news);
            }

            news.ConvertPicture();

            var clientForPost = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(news), Encoding.UTF8, "application/json");

            var responsePost2 = await clientForPost.PostAsync(_apiBaseUrl, content);

            NewsViewModel newsViewModel = new NewsViewModel();
            news.Status = true;
            newsViewModel.Status = news.Status;
            newsViewModel.Title = news.Title;
            newsViewModel.Text = news.Text;
            newsViewModel.Picture = news.Picture;
            newsViewModel.PictureFileName = news.PictureFileName;


            var newsJson = System.Text.Json.JsonSerializer.Serialize(newsViewModel);

            var HttpClient = new HttpClient();

            var stringUrl = Environment.GetEnvironmentVariable("webapi-service-url")!;

            HttpClient.BaseAddress = new Uri(stringUrl);

            var responsePost = await HttpClient.PostAsJsonAsync("News", newsViewModel);

            if (responsePost.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Haber başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Haber eklenirken bir hata oluştu.";

                // Hata durumunda materyalleri yeniden yükleyerek formu geri döndürüyoruz

                return View(news);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // Ürünü API'den çekiyoruz
            var newsResponse = await client.GetAsync($"{_apiBaseUrl}/{id}");
            if (!newsResponse.IsSuccessStatusCode)
            {
                TempData["RecordNotFounded"] = $"Id: {id} li haber bulunamadığı için güncelleme yapılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            var newsJson = await newsResponse.Content.ReadAsStringAsync();
            var news = JsonConvert.DeserializeObject<NewsAddViewModel>(newsJson);

            return View(news);
        }

        // Kategori Güncelleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(NewsViewModel news)
        {

            // Eğer yeni bir fotoğraf yüklenmişse, fotoğrafı güncelle
            if (news.PictureFormFile != null && news.PictureFormFile.Length > 0)
            {


                using (var memoryStream = new MemoryStream())
                {
                    news.PictureFormFile.CopyTo(memoryStream);
                    news.Picture = memoryStream.ToArray();
                    news.PictureFileName = news.PictureFormFile.FileName;
                }

            }
            else
            {
                // Yeni fotoğraf yüklenmemişse mevcut fotoğrafı koru
                var existingNewsResponse = await GetNewsById(news.Id);
                if (existingNewsResponse != null)
                {
                    news.Picture = existingNewsResponse.Picture;
                    news.PictureFileName = existingNewsResponse.PictureFileName;

                    // Mevcut fotoğrafı Base64 formatına çevirip kontrol ediyoruz
                    if (news.Picture != null)
                    {
                        var pictureBase64 = Convert.ToBase64String(news.Picture);
                        // Opsiyonel: Base64'ü loglayabilir veya başka bir işleme sokabilirsiniz
                        Console.WriteLine($"Mevcut resim Base64 formatı: {pictureBase64}");
                    }
                }
            }

            // Update için ProductViewModel nesnesi oluştur
            var updatedNews = new NewsViewModel
            {
                Id = news.Id,
                Title = news.Title,
                Text = news.Text,
                Picture = news.Picture,
                PictureFileName = news.PictureFileName,
                Status = news.Status
            };

            // API'ye PUT isteği gönder
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(updatedNews), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBaseUrl}/{news.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Haber başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Ürün güncellenirken bir hata oluştu.";

                return View(news);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Haber başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Haber silinirken bir hata oluştu.";
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