using Lind.MAUI.Example.Security;
using Lind.MAUI.Example.Security.Core;
using Lind.MAUI.Example.Services.Proxy;
using Lind.MAUI.Example.Services.Proxy.Core;
using Lind.MAUI.Example.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Maui.LifecycleEvents;
using System.Reflection;
using CommunityToolkit.Maui;

namespace Lind.MAUI.Example
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            var a = Assembly.GetExecutingAssembly();
            using (var stream = a.GetManifestResourceStream("Lind.MAUI.Example.appsettings.json"))
            {
                builder.Configuration.AddJsonStream(stream ?? throw new InvalidDataException()); // This line now works with the added namespace
            }
            
            

#if DEBUG

            builder.Logging.AddDebug();
#endif
            builder
                .ConfigureLifecycleEvents(events =>
                {
#if ANDROID
                    events.AddAndroid(platform =>
                    {
                        platform.OnActivityResult((activity, rc, result, data) =>
                        {
                            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(rc, result, data);
                        });
                    });
#endif
                })
                .UseMauiApp<App>().UseMauiCommunityToolkit().UsePrism(pBuilder =>
                {
                    pBuilder.ConfigureServices(services =>
                    {
                        services.AddSingleton(provider =>
                        {
                            var client = PublicClientApplicationBuilder.Create(builder.Configuration["AzureAD:ClientId"])
                            .WithB2CAuthority(builder.Configuration["AzureAD:Authority"])
#if WINDOWS
                            .WithRedirectUri(builder.Configuration["AzureAD:RedirectURI"]) // needed only for the system browser
#elif IOS
                .WithRedirectUri(builder.Configuration["AzureAD:iOSRedirectURI"])
                .WithIosKeychainSecurityGroup(builder.Configuration["AzureAD:iOSKeyChainGroup"])
#elif ANDROID
                .WithParentActivityOrWindow(() => Platform.CurrentActivity)
                .WithRedirectUri(builder.Configuration["AzureAD:AndroidRedirectURI"])
#elif MACCATALYST
                .WithRedirectUri(builder.Configuration["AzureAD:iOSRedirectURI"])
#endif
                            .Build();
#if WINDOWS || MACCATALYST
                            string fileName = Path.Join(FileSystem.CacheDirectory, "msal.token.cache2");
                            client.UserTokenCache.SetBeforeAccessAsync(async args =>
                            {
                                if (!(await FileSystem.Current.AppPackageFileExistsAsync(fileName)))
                                    return;
                                byte[] fileBytes;
                                using (var stream = new FileStream(fileName, FileMode.Open))
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        await stream.CopyToAsync(memoryStream);
                                        fileBytes = memoryStream.ToArray();
                                    }
                                }
                                args.TokenCache.DeserializeMsalV3(fileBytes);
                            });
                            client.UserTokenCache.SetAfterAccessAsync(async args =>
                            {
                                if (args.HasStateChanged)
                                {
                                    var data = args.TokenCache.SerializeMsalV3();
                                    using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                                    {
                                        await fs.WriteAsync(data, 0, data.Length);
                                    }
                                }
                            });
#endif
                            return client;
                        });
                        services.AddSingleton<IConfiguration>(builder.Configuration);
                        services.AddSingleton<ISecurityProvider, SecurityProvider>();
                        services.AddHttpClient(WeatherServiceProxy.WeatherServiceHttpClientKey, client =>
                        {
                            client.BaseAddress = new Uri(builder.Configuration["WeatherService:BaseUrl"] ?? throw new ArgumentNullException("WeatherServiceBaseUrl is not configured in appsettings.json or environment variables."));
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                        });
                        services.AddSingleton<IWeatherServiceProxy, WeatherServiceProxy>();
                        services.AddScoped<NotificationDialogViewModel>();
                        services.AddTransient<NotificationDialog>();
                        services.AddScoped<MainWindowViewModel>();
                        services.AddTransient<MainPage>();

                    });
                    pBuilder.RegisterTypes(registry =>
                    {
                        registry.RegisterForNavigation<MainPage, MainWindowViewModel>();
                        registry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>(NotificationDialogViewModel.NotificationDialog);
                    });
                    pBuilder.CreateWindow("/MainPage");
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            return builder.Build();
        }
    }
}
