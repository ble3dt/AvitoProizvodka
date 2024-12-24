using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Utilities;
using System;

namespace AvitoProizvodka.Core.Nav
{
    public class MainNavService : INavService
    {
        private readonly MainContext _mainContext;
        private readonly MarketContext _marketContext;
        private readonly UserContext _userContext;
        private readonly Func<ViewModel> _viewModel;

        public MainNavService(MainContext context)
        {
            _mainContext = context;
        }

        public MainNavService(MainContext mainContext, Func<ViewModel> viewModel)
        {
            _mainContext = mainContext;
            _viewModel = viewModel;
        }

        public MainNavService(MainContext mainContext, MarketContext marketContext, UserContext userContext, Func<ViewModel> viewModel)
        {
            _mainContext = mainContext;
            _marketContext = marketContext;
            _userContext = userContext;
            _viewModel = viewModel;
        }

        public void Pop()
        {
            _mainContext?.GoBack();
        }

        public void PopAndPush()
        {
            if (_viewModel is null) return;

            _mainContext.ClearAndNavigate(_viewModel);
        }

        public void Push()
        {
            if (_viewModel is null) return;

            _mainContext.Navigate(_viewModel);
        }
    }
}
