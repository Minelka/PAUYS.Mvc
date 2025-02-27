using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;
using System.Text;

namespace PAUYS.Mvc.Areas.CustomerService.Controllers
{
    [Area("CustomerService")]
    [Authorize(Policy = "CustomerPolicy")]
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

        [HttpGet]
        // Controller'da view'a model gönderildiğinden emin olun
        public IActionResult Add()
        {
            return View(new QuestionViewModel());  // Yeni bir CategoryViewModel nesnesi gönderiyoruz
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(QuestionViewModel question)
        {
            if (!ModelState.IsValid)
            {
                // Eğer model geçerli değilse, formu tekrar gösteriyoruz
                return View(question);
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(question), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Eklenirken bir hata oluştu.";
                return View(question);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var question = await GetQuestionById(id); // Kategori bilgilerini API'den alıyoruz

            if (question == null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li bulunamadığı için güncelleme yapılamıyor.";
                return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
            }

            return View(question); // Kategori bilgisiyle güncelleme formunu render ediyoruz
        }

        // Kategori Güncelleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(QuestionViewModel question)
        {
            if (!ModelState.IsValid)
            {
                return View(question); // Eğer model geçerli değilse formu tekrar gösteriyoruz
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(question), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiBaseUrl}/{question.Id}", content); // Güncellenen veriyi API'ye gönderiyoruz

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
        }

        // Get Material by Id
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            QuestionViewModel? question = await GetQuestionById(id); // GetMaterialById'yi kullanarak API'den veri alıyoruz.

            if (question is null)
            {
                TempData["RecordNotFounded"] = $"Id : {id} li kayıt bulunamadığı için düzenleme yapılamıyor.";
                return RedirectToAction(nameof(Index));
            }

            return View(question);
        }

        // Kategori Silme
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index)); // Kategoriler listesine yönlendiriyoruz
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
