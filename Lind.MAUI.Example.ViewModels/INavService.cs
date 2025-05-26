using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.MAUI.Example.ViewModels
{
    public interface INavService
    {
        
        //
        // Summary:
        //     Navigates to the most recent entry in the back navigation history by popping
        //     the calling Page off the navigation stack.
        //
        // Parameters:
        //   parameters:
        //     The navigation parameters
        //
        // Returns:
        //     If true a go back operation was successful. If false the go back operation failed.
        Task<INavigationResult> GoBackAsync(INavigationParameters parameters);

        //
        // Summary:
        //     Navigates to the most recent entry in the back navigation history for the viewName.
        //
        //
        // Parameters:
        //   viewName:
        //     The name of the View to navigate back to
        //
        //   parameters:
        //     The navigation parameters
        //
        // Returns:
        //     If true a go back operation was successful. If false the go back operation failed.
        Task<INavigationResult> GoBackToAsync(string viewName, INavigationParameters parameters);

        //
        // Summary:
        //     When navigating inside a NavigationPage: Pops all but the root Page off the navigation
        //     stack
        //
        // Parameters:
        //   parameters:
        //     The navigation parameters
        //
        // Returns:
        //     Prism.Navigation.INavigationResult indicating whether the request was successful
        //     or if there was an encountered System.Exception.
        //
        // Remarks:
        //     Only works when called from a View within a NavigationPage
        Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters);

        //
        // Summary:
        //     Initiates navigation to the target specified by the uri.
        //
        // Parameters:
        //   uri:
        //     The Uri to navigate to
        //
        //   parameters:
        //     The navigation parameters
        //
        // Returns:
        //     Prism.Navigation.INavigationResult indicating whether the request was successful
        //     or if there was an encountered System.Exception.
        //
        // Remarks:
        //     Navigation parameters can be provided in the Uri and by using the parameters.
        Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters);

        //
        // Summary:
        //     Selects a Tab of the TabbedPage parent and Navigates to a specified Uri
        //
        // Parameters:
        //   name:
        //     The name of the tab to select
        //
        //   uri:
        //     The Uri to navigate to
        //
        //   parameters:
        //     The navigation parameters
        //
        // Returns:
        //     Prism.Navigation.INavigationResult indicating whether the request was successful
        //     or if there was an encountered System.Exception.
        Task<INavigationResult> SelectTabAsync(string name, Uri uri, INavigationParameters parameters);
        Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters);
        Task<INavigationResult> NavigateAsync(string name);
    }
}
