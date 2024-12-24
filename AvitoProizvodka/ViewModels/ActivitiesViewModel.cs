using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class ActivitiesViewModel : ViewModel
    {
        private readonly UserContext _userContext;
        private readonly MarketContext _marketContext;
        private readonly MarketplaceEntities _entities;

        private bool _isLoaded = false;
        private event Action Loaded;

        private readonly List<Product> _pendingProducts = new List<Product>();
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public ICommand AddProductCommand { get; }
        public ICommand RemoveProductCommand { get; }

        public ActivitiesViewModel(INavService add, UserContext userContext, MarketContext marketContext, MarketplaceEntities entities)
        {
            _userContext = userContext;
            _marketContext = marketContext;
            _entities = entities;

            _marketContext.MyProductAdded += TrackProduct;

            AddProductCommand = new NavCommand(add);
            RemoveProductCommand = new RelayAsyncCommand(RemoveProduct);

            Loaded += AddPendingProducts;

            Task.Run(LoadProducts);
        }

        private async Task RemoveProduct(object param)
        {
            if (param is Product product)
            {
                try
                {
                    var userProduct = product.UserProduct.FirstOrDefault();

                    userProduct.IsNotActive = true;
                    product.TrackIsSold();

                    await _entities.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async Task LoadProducts()
        {
            try
            {
                var productsIds = await _entities.UserProduct
                    .Where(it => it.UserId == _userContext.User.Id)
                    .Select(it => it.ProductId)
                    .ToListAsync();

                var products = await _entities.Product
                    .Where(it => productsIds.Contains(it.Id))
                    .ToListAsync();

                Products = new ObservableCollection<Product>(products);
                OnPropertyChanged(nameof(Products));
            }
            finally
            {
                _isLoaded = true;
                Loaded?.Invoke();
            }
        }

        private void AddPendingProducts()
        {
            foreach (var item in _pendingProducts)
            {
                Products.Add(item);
            }
        }

        private void TrackProduct(Product product)
        {
            if (_isLoaded)
            {
                Products.Add(product);
            }
            else
            {
                _pendingProducts.Add(product);
            }
        }

        public override void Dispose()
        {
            _marketContext.MyProductAdded -= TrackProduct;
            Loaded -= AddPendingProducts;

            GC.SuppressFinalize(this);
        }
    }
}
