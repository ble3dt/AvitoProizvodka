using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class MainMarketViewModel : ViewModel
    {
        private readonly INavService _product;
        private readonly UserContext _userContext;
        private readonly MarketContext _marketContext;
        private readonly MarketplaceEntities _entities;

        public ICommand SelectProductCommand { get; }
        public ICommand SearchProductCommand { get; }

        public List<Product> FirstBanch { get; set; } = new List<Product>();
        public List<Product> SecondBanch { get; set; } = new List<Product>();
        public List<Product> ThirdBanch { get; set; } = new List<Product>();

        public string SearchText
        {
            get => _marketContext.SearchText;
            set
            {
                _marketContext.SearchText = value;
                OnPropertyChanged();
            }
        }

        public MainMarketViewModel(INavService product, INavService list, UserContext userContext, MarketContext marketContext, MarketplaceEntities entities)
        {
            _product = product;
            _userContext = userContext;
            _marketContext = marketContext;
            _entities = entities;

            SelectProductCommand = new RelayCommand(SelectCommand);
            SearchProductCommand = new NavCommand(list);

            Task.Run(LoadBanches);
        }

        private void SelectCommand(object param)
        {
            if (param is Product product)
            {
                _marketContext.SelectedProduct = product;
                _product.Push();
            }
        }

        private async Task LoadBanches()
        {
            var products = await _entities.Product
                .Where(it => it.UserProduct.FirstOrDefault().UserId != _userContext.User.Id && !it.UserProduct.FirstOrDefault().IsNotActive)
                .OrderBy(it => Guid.NewGuid())
                .Take(24)
                .ToListAsync();

            FirstBanch = products.Take(8).ToList();
            SecondBanch = products.Skip(8).Take(8).ToList();
            ThirdBanch = products.Skip(16).Take(8).ToList();

            OnPropertyChanged(nameof(FirstBanch));
            OnPropertyChanged(nameof(SecondBanch));
            OnPropertyChanged(nameof(ThirdBanch));
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
