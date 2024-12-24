using AvitoProizvodka.Core.Nav;
using System;
using System.Windows.Input;

namespace AvitoProizvodka.Core.Command
{
    public class NavCommand : ICommand
    {
        private readonly INavService _navService;

        public NavCommand(INavService navService)
        {
            _navService = navService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _navService.Push();
        }
    }
}
