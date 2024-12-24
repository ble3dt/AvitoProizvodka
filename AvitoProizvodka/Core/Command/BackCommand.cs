using AvitoProizvodka.Core.Nav;
using System;
using System.Windows.Input;

namespace AvitoProizvodka.Core.Command
{
    public class BackCommand : ICommand
    {
        private readonly INavService _navService;

        public BackCommand(INavService navService)
        {
            _navService = navService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _navService.Pop();
        }
    }
}
