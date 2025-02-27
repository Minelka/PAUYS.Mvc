using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAUYS.ViewModel.Concrete;


namespace PAUYS.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = $"{Environment.GetEnvironmentVariable("webapi-service-url")!}CustomerRequest";


        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);

            List<CustomerRequestViewModel> customerRequests = new();
            if (response.IsSuccessStatusCode)
            {
                var customerRequestsJson = await response.Content.ReadAsStringAsync();
                customerRequests = JsonConvert.DeserializeObject<List<CustomerRequestViewModel>>(customerRequestsJson)
                    ?? new List<CustomerRequestViewModel>();
            }

            var today = DateTime.Now;

            // İstatistikler
            var lastWeekRefundRequests = customerRequests
                .Count(cr => !cr.RefundorNewRequest && cr.Created >= today.AddDays(-7));

            var lastWeekNewRequests = customerRequests
                .Count(cr => cr.RefundorNewRequest && cr.Created >= today.AddDays(-7));

            var lastMonthRefundRequests = customerRequests
                .Count(cr => !cr.RefundorNewRequest && cr.Created >= today.AddMonths(-1));

            var lastMonthNewRequests = customerRequests
                .Count(cr => cr.RefundorNewRequest && cr.Created >= today.AddMonths(-1));

            var thisYearNewRequests = customerRequests
                .Count(cr => cr.RefundorNewRequest && cr.Created.Year == today.Year);

            var thisYearRefundRequests = customerRequests
                .Count(cr => !cr.RefundorNewRequest && cr.Created.Year == today.Year);

            var unansweredRequests = customerRequests
                .Count(cr => string.IsNullOrEmpty(cr.AdminMessage));

            // Grafik verileri
            ViewBag.RefundRequestsData = new[] { lastWeekRefundRequests, lastMonthRefundRequests, thisYearRefundRequests };
            ViewBag.NewRequestsData = new[] { lastWeekNewRequests, lastMonthNewRequests, thisYearNewRequests };
            ViewBag.UnansweredRequests = unansweredRequests;

            return View();
        }
    }


}
