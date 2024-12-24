using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class ProductListViewModel : ViewModel
    {
        private readonly INavService _product;
        private readonly MarketContext _marketContext;

        public string SearchText { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();

        public ICommand GoBackCommand { get; }
        public ICommand SelectProductCommand { get; }

        public ProductListViewModel(INavService back, INavService product, MarketContext marketContext, MarketplaceEntities entities)
        {
            _product = product;
            _marketContext = marketContext;

            SearchText = $"Результаты по \"{marketContext.SearchText}\"";
            GoBackCommand = new BackCommand(back);
            SelectProductCommand = new RelayCommand(SelectProduct);

            Products = entities.Product
                .Where(it => it.Title.ToLower().Contains(marketContext.SearchText.ToLower())
                    || it.Subtitle.ToLower().Contains(marketContext.SearchText.ToLower()))
                .ToList();
        }

        private void SelectProduct(object param)
        {
            if (param is Product product)
            {
                _marketContext.SelectedProduct = product;
                _product.Push();
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
