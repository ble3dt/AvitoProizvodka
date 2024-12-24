using AvitoProizvodka.Core.Utilities;
using System;
using System.Collections.Generic;

namespace AvitoProizvodka.Core.Contexts
{
    public class MainContext
    {
        public readonly Stack<ViewModel> _history = new Stack<ViewModel>();
        
        public ViewModel CurrentViewModel
        {
            get
            {
                if (_history.Count == 0)
                {
                    return null;
                }
                else
                {
                    return _history.Peek();
                }
            }
        }

        public void GoBack()
        {
            _history.Pop();

            ViewModelChanged?.Invoke();
        }

        public void Navigate(Func<ViewModel> func)
        {
            if (func is null) return;

            _history.Push(func());

            ViewModelChanged?.Invoke();
        }

        public void ClearAndNavigate(Func<ViewModel> func)
        {
            foreach (var item in _history)
            {
                item?.Dispose();
            }
            _history.Clear();

            _history.Push(func());

            ViewModelChanged?.Invoke();
        }

        public event Action ViewModelChanged;
    }
}
