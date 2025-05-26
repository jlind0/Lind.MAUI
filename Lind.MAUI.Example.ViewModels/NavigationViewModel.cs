using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lind.MAUI.Example.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        protected INavService NavService { get; }
        public ICommand MainWindowNav { get; }
        public ICommand CounterNav { get; }
        public NavigationViewModel(INavService navService)
        {
            NavService = navService;
            MainWindowNav = new AsyncDelegateCommand(() => navService.NavigateAsync("MainPage"));
            CounterNav = new AsyncDelegateCommand(() => navService.NavigateAsync("CounterPage"));
        }
    }
}
