using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvitoProizvodka.Core.Command
{
    public class RelayAsyncCommand : ICommand
    {
        private readonly Func<Task> _action;
        private readonly Func<object, Task> _paramedAction;

        public RelayAsyncCommand(Func<Task> action)
        {
            _action = action;
        }

        public RelayAsyncCommand(Func<object, Task> paramedAction)
        {
            _paramedAction = paramedAction;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
            _paramedAction?.Invoke(parameter);
        }
    }
}
