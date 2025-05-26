using Lind.MAUI.Example.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.MAUI.Example
{
    public class NavService : INavService
    {
        protected INavigationService NavigationService { get; }
        public NavService(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
        public Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
        {
            return NavigationService.GoBackAsync(parameters);
        }

        public Task<INavigationResult> GoBackToAsync(string viewName, INavigationParameters parameters)
        {
            return NavigationService.GoBackToAsync(viewName, parameters);
        }

        public Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters)
        {
            return NavigationService.GoBackToRootAsync(parameters);
        }

        public Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
        {
            return NavigationService.NavigateAsync(uri, parameters);
            return NavigationService.NavigateAsync("", parameters);
        }

        public Task<INavigationResult> SelectTabAsync(string name, Uri uri, INavigationParameters parameters)
        {
            return NavigationService.SelectTabAsync(name, uri, parameters);
        }

        public Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters)
        {
            return NavigationService.NavigateAsync(name, parameters);
        }

        public Task<INavigationResult> NavigateAsync(string name)
        {
            return NavigationService.NavigateAsync(name);
        }
    }
}
