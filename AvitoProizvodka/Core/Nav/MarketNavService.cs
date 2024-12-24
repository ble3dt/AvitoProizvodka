using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Utilities;
using System;

namespace AvitoProizvodka.Core.Nav
{
    public class MarketNavService : INavService
    {
        private readonly MarketContext _context;
        private readonly Func<ViewModel> _viewModel;

        public MarketNavService(MarketContext context)
        {
            _context = context;
        }

        public MarketNavService(MarketContext context, Func<ViewModel> viewModel)
        {
            _context = context;
            _viewModel = viewModel;
        }

        public void Pop()
        {
            _context?.GoBack();
        }

        public void PopAndPush()
        {
            if (_viewModel is null) return;

            _context.ClearAndNavigate(_viewModel);
        }

        public void Push()
        {
            if (_viewModel is null) return;

            _context.Navigate(_viewModel);
        }
    }
}
