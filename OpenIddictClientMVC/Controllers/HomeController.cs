using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddictClient.Attributes;
using OpenIddictClientMVC.Models;
using Polly;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OpenIddictClientMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [OpenIddictAuthorize, HttpPost]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            // Для сценариев, в которых не следует использовать обработчик аутентификации по умолчанию, настроенный в ASP.NET Core
            // параметры аутентификации, можно указать здесь конкретную схему.
            string? token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            
            using HttpClient client = _httpClientFactory.CreateClient();
            // запрос к апи на сервере
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44376/api/message2");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            // запрос к запрещенному апи
            using var request_res = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7260/GetWeatherForecast");
            request_res.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response_res = await client.SendAsync(request_res, cancellationToken);
            response_res.EnsureSuccessStatusCode();
            var res = await response_res.Content.ReadAsStringAsync(cancellationToken);

            return View(model: res);
        }
        [OpenIddictAuthorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Если ошибка возникла на сервере OpenIddict, отобразить сведения об ошибке.
            var response = HttpContext.GetOpenIddictServerResponse();
            if (response is not null)
            {
                return View(new ErrorViewModel
                {
                    Error = response.Error,
                    ErrorDescription = response.ErrorDescription
                });
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
