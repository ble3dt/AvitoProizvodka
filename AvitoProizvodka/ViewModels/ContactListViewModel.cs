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
    public class ContactListViewModel : ViewModel
    {
        private readonly INavService _user;
        private readonly UserContext _userContext;
        private readonly MarketplaceEntities _entities;
        private string _searchText;
        public string SearchText
        {
            get => _searchText; set
            {
                _searchText = value; OnPropertyChanged();
            }
        }

        public List<User> Users => 
            _entities.User
                .Where(it => it.Title.ToLower().Contains(SearchText.ToLower())
                    || it.Subtitle.ToLower().Contains(SearchText.ToLower())
                    || it.Email.ToLower().Contains(SearchText.ToLower())
                    || it.PhoneNo.ToLower().Contains(SearchText.ToLower()))
                .ToList();

        public ICommand GoBackCommand { get; }
        public ICommand ConfirmSearchCommand { get; }
        public ICommand SelectUserCommand { get; }

        public ContactListViewModel(INavService back, INavService user, UserContext userContext, MarketplaceEntities entities)
        {
            _user = user;
            _userContext = userContext;
            _entities = entities;

            GoBackCommand = new BackCommand(back);
            SelectUserCommand = new RelayCommand(SelectUser);
            ConfirmSearchCommand = new RelayCommand(ConfirmSearch);
        }

        private void SelectUser(object param)
        {
            if (param is User user)
            {
                _userContext.SelectedUser = user;
                _user.Push();
            }
        }

        private void ConfirmSearch()
        {
            OnPropertyChanged(nameof(Users));
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
