using Lind.MAUI.Example.Security.Core;
using Lind.MAUI.Example.Services.Proxy.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lind.MAUI.Example.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private bool isAuthenticated;
        public bool IsAuthenticated
        {
            get => isAuthenticated;
            set
            {
                if(SetProperty(ref isAuthenticated, value))
                    RaisePropertyChanged(nameof(IsNotAuthenticated));
            }
        }
        public bool IsNotAuthenticated => !IsAuthenticated;
        private string? userName;
        public string? UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }
        private bool isLoaded;
        public bool IsLoaded
        {
            get => isLoaded;
            set => SetProperty(ref isLoaded, value);
        }
        protected ISecurityProvider SecurityProvider { get; }
        public ObservableCollection<WeatherForecast> WeatherForecasts { get; } = new ObservableCollection<WeatherForecast>();
        protected IWeatherServiceProxy WeatherService { get; }
        protected ILogger Logger { get; }
        public ICommand Login { get; }
        public ICommand Logout { get; }
        public ICommand Load { get; }
        protected string[] Scopes { get; }
        protected IDialogService DialogService { get; }
        public MainWindowViewModel(ISecurityProvider securityProvider, IWeatherServiceProxy weatherService,
            ILogger<MainWindowViewModel> logger, IConfiguration config, IDialogService dialogService)
        {
            SecurityProvider = securityProvider;
            WeatherService = weatherService;
            Logger = logger;
            Scopes = config["WeatherService:Scopes"]?.Split(',') ?? throw new ArgumentNullException("WeatherServiceScopes is not configured in appsettings.json or environment variables.");
            Login = new AsyncDelegateCommand(DoLogin, () => !IsAuthenticated);
            Logout = new AsyncDelegateCommand(DoLogout, () => IsAuthenticated);
            Load = new AsyncDelegateCommand(DoLoad, () => IsAuthenticated);
            DialogService = dialogService;
        }
        protected virtual async Task DoLogin(CancellationToken token)
        {
            try
            {
                await SecurityProvider.Login(Scopes, false, token);
                IsAuthenticated = SecurityProvider.IsAuthenticated;
                UserName = SecurityProvider.UserName;
                await DoLoad(token);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred during login.");
                IsAuthenticated = false;
                UserName = null;
                DialogService.ShowDialog(NotificationDialogViewModel.NotificationDialog, new DialogParameters
                {
                    { "message", "An error occurred during login. Please try again." }
                });
            }
        }
        protected virtual async Task DoLogout(CancellationToken token)
        {
            try
            {
                await SecurityProvider.Logout(token);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred during logout.");
                DialogService.ShowDialog(NotificationDialogViewModel.NotificationDialog, new DialogParameters
                {
                    { "message", "An error occurred during logout. Please try again." }
                });
            }
            finally
            {
                IsAuthenticated = false;
                UserName = null;
            }
        }
        protected virtual async Task DoLoad(CancellationToken token)
        {
            if (!IsAuthenticated)
                return;
            try
            {
                IsLoading = true;
                WeatherForecasts.Clear();
                var forecasts = await WeatherService.GetWeatherForecastAsync(token);
                if (forecasts != null)
                {

                    foreach (var forecast in forecasts)
                    {
                        WeatherForecasts.Add(forecast);
                    }
                }
                IsLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while loading weather forecasts.");
                IsLoaded = false;
                DialogService.ShowDialog(NotificationDialogViewModel.NotificationDialog, new DialogParameters
                {
                    { "message", "An error occurred while loading weather forecasts. Please try again." }
                });
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
    public class NotificationDialogViewModel : BindableBase, IDialogAware
    {
        public const string NotificationDialog = nameof(NotificationDialog);
        private DelegateCommand<string>? _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _title = "Notification";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DialogCloseListener RequestClose { get; }

        protected virtual void CloseDialog(string parameter)
        {
            RaiseRequestClose(new DialogResult());
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }


}
