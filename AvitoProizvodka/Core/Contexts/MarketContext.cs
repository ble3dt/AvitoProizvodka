using AvitoProizvodka.Core.Utilities;
using System.Collections.Generic;
using System;
using AvitoProizvodka.Data;

namespace AvitoProizvodka.Core.Contexts
{
    public class MarketContext
    {
        public string SearchText = string.Empty;

        public Product SelectedProduct { get; set; }

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
        public void Navigate(Func<ViewModel> func)
        {
            if (func is null) return;

            _history.Push(func());

            ViewModelChanged?.Invoke();
        }

        public void AddMyProduct(Product product)
        {
            MyProductAdded?.Invoke(product);
        }
        public void AddProductToCart(Cart cart)
        {
            ProductAddedToCart?.Invoke(cart);
        }
        
        public event Action ViewModelChanged;
        public event Action<Product> MyProductAdded;
        public event Action<Cart> ProductAddedToCart;
    }
}
