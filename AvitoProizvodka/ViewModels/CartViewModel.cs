using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class CartViewModel : ViewModel
    {
        private readonly INavService _product;
        private readonly UserContext _userContext;
        private readonly MarketContext _marketContext;
        private readonly MarketplaceEntities _entities;
        private ObservableCollection<Cart> products = new ObservableCollection<Cart>();

        private readonly object _locker = 123;

        public ObservableCollection<Cart> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectProductCommand { get; }
        public ICommand RemoveProductCommand { get; }
        public ICommand BuyCommand { get; }

        public CartViewModel(INavService product, UserContext userContext, MarketContext marketContext, MarketplaceEntities entities)
        {
            _product = product;
            _userContext = userContext;
            _marketContext = marketContext;
            _entities = entities;

            SelectProductCommand = new RelayCommand(SelectProduct);
            RemoveProductCommand = new RelayCommand(RemoveProduct);
            BuyCommand = new RelayAsyncCommand(BuyAsync);

            Task.Run(LoadCart);
        }

        private async Task LoadCart()
        {
            var products = await _entities.Cart
                .Where(it => it.UserId == _userContext.User.Id && !it.Deleted)
                .ToListAsync();

            Products = new ObservableCollection<Cart>(products);
        }

        private void SelectProduct(object param)
        {
            if (param is Cart product)
            {
                _marketContext.SelectedProduct = product.Product;
                _product.Push();
            }
        }

        private async Task BuyAsync()
        {
            if (Products.Count == 0)
            {
                MessageBox.Show("Покупать нечего");
            }

            try
            {
                var products = Products.SelectMany(it => it.Product.UserProduct).ToList();

                products.ForEach(it => it.IsSelled = true);

                _entities.Cart.RemoveRange(Products.AsEnumerable());

                await _entities.SaveChangesAsync();

                Products.Clear();

                MessageBox.Show("Покупка совершена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveProduct(object param)
        {
            if (param is Cart cart)
            {
                try
                {
                    lock (_locker)
                    {
                        var remote = _entities.Cart.FirstOrDefault(it => it.UserId == cart.UserId && it.ProductId == cart.ProductId);

                        _entities.Cart.Remove(remote);
                        _entities.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Products.Remove(cart);
                }
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
