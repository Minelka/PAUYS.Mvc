using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Question";

        public QuestionController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var questionsDto = await GetAllQuestions();  // API'den DTO'yu alıyoruz
            var questionsViewModel = questionsDto
                .Where(m => !m.IsDeleted)  // Silinmiş olmayanları filtreliyoruz
                .Select(m => new QuestionViewModel  // DTO'dan ViewModel'e dönüşüm
                {
                    Id = m.Id,
                    Questions = m.Questions,
                    Answer = m.Answer
                }).ToList();

            return View(questionsViewModel);  // ViewModel ile View'a veri gönderiyoruz
        }

        // API'den veri alma
        private async Task<List<QuestionViewModel>> GetAllQuestions()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<QuestionViewModel>>(content);
            }

            return new List<QuestionViewModel>();  // Eğer hata varsa boş liste döndürüyoruz
        }

        private async Task<QuestionViewModel?> GetQuestionById(int id)
        {
            var client = _httpClientFactory.CreateClient(); // HttpClient nesnesi oluşturuyoruz
            var response = await client.GetAsync($"{_apiBaseUrl}/{id}"); // ID'yi URL'ye ekleyerek GET isteği gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API'den dönen JSON'u ViewModel'e dönüştürme
                return JsonConvert.DeserializeObject<QuestionViewModel>(content);
            }

            return null; // Eğer hata varsa null döndürüyoruz
        }
    }
}
