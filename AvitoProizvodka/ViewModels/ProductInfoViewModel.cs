using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class ProductInfoViewModel : ViewModel
    {
        private readonly INavService _profile;
        private readonly UserContext _userContext;
        private readonly MarketContext _marketContext;
        private readonly MarketplaceEntities _entities;

        public Product Product => _marketContext.SelectedProduct;
        public User Seller
        {
            get
            {
                var product = Product.UserProduct.First();

                if (product is null)
                {
                    return new User();
                }
                else
                {
                    return product.User;
                }
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand GoToProfileCommand { get; }

        public ProductInfoViewModel(INavService back, INavService profile, UserContext userContext, MarketContext marketContext, MarketplaceEntities entities)
        {
            _profile = profile;
            _userContext = userContext;
            _marketContext = marketContext;
            _entities = entities;

            GoBackCommand = new BackCommand(back);
            GoToProfileCommand = new RelayCommand(GoToProfile);
            AddProductCommand = new RelayAsyncCommand(AddProduct);
        }

        private void GoToProfile()
        {
            _userContext.SelectedUser = Seller;
            _profile.Push();
        }

        public async Task AddProduct()
        {

            var remote = await _entities.Cart.FirstOrDefaultAsync(it => it.ProductId == Product.Id && it.UserId == _userContext.User.Id);

            if (remote is null)
            {
                var cart = new Cart
                {
                    UserId = _userContext.User.Id,
                    ProductId = Product.Id,
                    Timestamp = DateTime.Now,
                };
                _entities.Cart.Add(cart);

                await _entities.SaveChangesAsync();

                _marketContext.AddProductToCart(cart);
            }
            else
            {
                MessageBox.Show("Уже в корзине");
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
