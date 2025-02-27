using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PAUYS.Mvc.Areas.Admin.Models;
using PAUYS.ViewModel.Concrete;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Policy;

namespace PAUYS.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory; // IHttpClientFactory'yi ekliyoruz
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}Auth"; // API Base URL
        //private static readonly ILog _logger = LogManager.GetLogger(typeof(AuthController));


        // IHttpClientFactory'yi konstruktor ile enjekte ediyoruz
        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Login GET
        [HttpGet]
        public IActionResult Login()
        {
            //// Burada bir giriş loglaması yapabilirsiniz
            //_logger.Info("AuthController sınıfı başlatıldı.");
            //// Burada bir giriş loglaması yapabilirsiniz
            //_logger.Warn("AuthController sınıfı başlatıldı.");
            //// Burada bir giriş loglaması yapabilirsiniz
            //_logger.Debug("AuthController sınıfı başlatıldı.");
            //_logger.Error("AuthController sınıfı başlatıldı.");
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string token = HttpContext.Session.GetString("Token");
                // JWT token'ı işliyoruz
                JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                var claims = jwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.Role);
                var schemeName = "";
                var redirectUrl = "";

                if (claims.Any())
                {
                    Claim roleClaim = claims.FirstOrDefault();

                    if (roleClaim != null)
                    {
                        if (roleClaim.Value == "Admin")
                        {
                            return Redirect("/Admin/Home/Index");
                        }

                        if (roleClaim.Value == "ContentManager")
                        {
                            return Redirect("/ContentManager/MainPage/Index");
                        }

                        if (roleClaim.Value == "CustomerService")
                        {
                            return Redirect("/CustomerService/MainPage/Index");
                        }
                    }
                }
            }


            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {


            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
            {
                // Hata mesajlarını loglama veya ekrana yazdırma
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Hata mesajlarını loglayabilirsiniz veya kullanıcıya gösterebilirsiniz
                    Console.WriteLine(error.ErrorMessage); // Hata mesajlarını konsola yazdır
                }
                return View(model);
            }

            try
            {
                // IHttpClientFactory ile HttpClient örneği alıyoruz
                var httpClient = _httpClientFactory.CreateClient();

                // API'ye istek gönderiyoruz (endPoint'e "/Login" ekliyoruz)
                HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync(_apiBaseUrl + "/Login", model);

                // Başarılı olup olmadığını kontrol ediyoruz
                responseMessage.EnsureSuccessStatusCode();

                // API'den dönen token'ı alıyoruz
                LoginResponseViewModel result = await responseMessage.Content.ReadFromJsonAsync<LoginResponseViewModel>();

                if (string.IsNullOrEmpty(result?.Token)) // Null kontrolü
                {
                    ModelState.AddModelError("TokenEmpty", "Token boş veya null");
                    return View(model);
                }

                // JWT token'ı işliyoruz
                JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

                var claims = jwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.Role);
                var schemeName = "";
                var redirectUrl = "";

                if (claims.Any())
                {
                    Claim roleClaim = claims.FirstOrDefault();

                    if (roleClaim != null) 
                    {
                        if(roleClaim.Value == "Admin")
                        {
                            schemeName = "AdminScheme";
                            redirectUrl = "/Admin/Home/Index";
                        }

                        if (roleClaim.Value == "ContentManager")
                        {
                            schemeName = "ContentScheme";
                            redirectUrl = "/ContentManager/MainPage/Index";
                        }
                        if (roleClaim.Value == "CustomerService")
                        {
                            schemeName = "CustomerScheme";
                            redirectUrl = "/CustomerService/MainPage/Index";
                        }
                    }
                }

                // Kullanıcı bilgilerini ClaimsIdentity ile oluşturuyoruz
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(jwtSecurityToken.Claims, schemeName);

                // ClaimsPrincipal oluşturuyoruz
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Kullanıcıyı giriş yaptırıyoruz
                HttpContext.SignInAsync(schemeName, claimsPrincipal).Wait();

                // Token'ı session'a kaydediyoruz
                HttpContext.Session.SetString("Token", result.Token);

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya bilgi veriyoruz
                ModelState.AddModelError("ApiError", $"Bir hata oluştu: {ex.Message}");
                return View(model);
            }
        }

        // Logout
        [HttpGet]
        public IActionResult Logout()
        {
            // Oturumu sonlandırıyoruz
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();

            return Redirect(Environment.GetEnvironmentVariable("application-url")!);
        }
    }

}
