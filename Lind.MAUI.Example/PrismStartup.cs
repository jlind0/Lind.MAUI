using Lind.MAUI.Example.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.MAUI.Example
{
    public static class PrismStartup
    {
        public static void Configure(PrismAppBuilder builder)
        {
            builder.RegisterTypes(RegisterTypes)
                .CreateWindow("MainPage");
        }

        private static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage, MainWindowViewModel>();
            containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>(NotificationDialogViewModel.NotificationDialog);
        }
    }
}
