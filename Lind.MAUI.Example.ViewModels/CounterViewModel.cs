using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lind.MAUI.Example.ViewModels
{
    public class CounterViewModel : BindableBase
    {
        private int count = 0;
        public int Count
        {
            get => count;
            set => SetProperty(ref count, value);
        }
        public ICommand Increment { get; }
        public ICommand Decrement { get; }
        public NavigationViewModel NavVM { get; }
        public CounterViewModel(NavigationViewModel navService)
        {
            Increment = new DelegateCommand(() => Count++);
            Decrement = new DelegateCommand(() => Count--);
            NavVM = navService;
        }
    }
}
