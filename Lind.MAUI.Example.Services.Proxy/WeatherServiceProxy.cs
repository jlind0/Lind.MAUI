using Lind.MAUI.Example.Security.Core;
using Lind.MAUI.Example.Services.Proxy;
using Lind.MAUI.Example.Services.Proxy.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Lind.MAUI.Example.Services.Proxy
{
    public abstract class SecureServiceProxy
    {
        protected ISecurityProvider SecurityProvider { get; }
        protected ILogger Logger { get; }
        protected IHttpClientFactory HttpClientFactory { get; }
        protected abstract string HttpClientKey { get; }

        public SecureServiceProxy(ISecurityProvider securityProvider, IHttpClientFactory httpClientFactory,
            ILogger logger)
        {
            SecurityProvider = securityProvider;
            Logger = logger;
            HttpClientFactory = httpClientFactory;
        }

        protected virtual async Task<HttpClient> GetHttpClientAsync(string[] scopes, CancellationToken token = default)
        {
            try
            {
                if (!SecurityProvider.IsAuthenticated)
                {
                    await SecurityProvider.Login(scopes, true, token);
                }
                var accessToken = await SecurityProvider.GetAccessToken(scopes, token);
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new InvalidOperationException("Failed to obtain access token.");
                }
                var client = HttpClientFactory.CreateClient(HttpClientKey);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return client;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting HTTP client with access token");
                throw;
            }
        }
    }
    public class WeatherServiceProxy : SecureServiceProxy, IWeatherServiceProxy
    {
        public const string WeatherServiceHttpClientKey = "WeatherServiceClient";
        protected override string HttpClientKey => WeatherServiceHttpClientKey;
        protected string[] Scopes { get; }
        public WeatherServiceProxy(ISecurityProvider securityProvider, 
            IHttpClientFactory httpClientFactory, ILogger<WeatherServiceProxy> logger, IConfiguration config)
            : base(securityProvider, httpClientFactory, logger)
        {
            Scopes = config["WeatherService:Scopes"]?.Split(',') ?? throw new ArgumentNullException("WeatherServiceScopes is not configured in appsettings.json or environment variables.");
        }
        public async Task<IEnumerable<WeatherForecast>?> GetWeatherForecastAsync(CancellationToken token = default)
        {
            try
            {
                var client = await GetHttpClientAsync(Scopes, token);
                var response = await client.GetAsync("weatherforecast", token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>(token);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting weather forecast");
                throw;
            }
        }
    }
}

