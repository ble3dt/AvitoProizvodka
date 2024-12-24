using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using System;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class LayoutViewModel : ViewModel
    {
        private readonly INavService _market;
        private readonly INavService _cart;
        private readonly INavService _contacts;
        private readonly INavService _profile;
        private readonly INavService _mine;
        private readonly MarketContext _marketContext;

        public ViewModel CurrentViewModel => _marketContext.CurrentViewModel;

        private bool _isMain;
        private bool _isCart;
        private bool _isHistory;
        private bool _isContacts;
        private bool _isMine;
        private bool _isProfile = true;

        public bool IsMain
        {
            get => _isMain; set
            {
                if (value != _isMain)
                {
                    _isMain = value; OnPropertyChanged();
                }

                if (value)
                {
                    _market.PopAndPush();
                }
            }
        }
        public bool IsCart
        {
            get => _isCart;
            set
            {
                if (value != _isCart)
                {
                    _isCart = value; OnPropertyChanged();
                }

                if (value)
                {
                    _cart.PopAndPush();
                }
            }
        }
        public bool IsHistory
        {
            get => _isHistory; set
            {
                if (value != _isHistory)
                {
                    _isHistory = value; OnPropertyChanged();
                }
            }
        }
        public bool IsContacts
        {
            get => _isContacts; set
            {
                if (value != _isContacts)
                {
                    _isContacts = value; OnPropertyChanged();
                }

                if (value)
                {
                    _contacts.PopAndPush();
                }
            }
        }
        public bool IsMine
        {
            get => _isMine; set
            {
                if (value != _isMine)
                {
                    _isMine = value; OnPropertyChanged();
                }

                if (value)
                {
                    _mine.PopAndPush();
                }
            }
        }
        public bool IsProfile
        {
            get => _isProfile; set
            {
                if (value != _isProfile)
                {
                    _isProfile = value; OnPropertyChanged();
                }

                if (value)
                {
                    _profile.PopAndPush();
                }
            }
        }

        public ICommand GoMainCommand { get; }
        public ICommand GoCartCommand { get; }
        public ICommand GoHistoryCommand { get; }
        public ICommand GoContactsCommand { get; }
        public ICommand GoMineCommand { get; }
        public ICommand GoProfileCommand { get; }

        public LayoutViewModel(INavService market, INavService cart, INavService contacts, INavService profile, INavService mine, MarketContext marketContext)
        {
            _market = market;
            _cart = cart;
            _contacts = contacts;
            _profile = profile;
            _mine = mine;
            _marketContext = marketContext;

            _marketContext.ViewModelChanged += TrackViewModel;
        }

        private void TrackViewModel()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public override void Dispose()
        {
            _marketContext.ViewModelChanged -= TrackViewModel;
            GC.SuppressFinalize(this);
        }
    }
}
