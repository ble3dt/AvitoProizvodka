using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class ContactsViewModel : ViewModel
    {
        private readonly INavService _user;
        private readonly UserContext _userContext;
        private readonly MarketplaceEntities _entities;

        public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();

        public ICommand SelectUserCommand { get; }
        public ICommand SearchUserCommand { get; }

        public ContactsViewModel(INavService user, INavService search, UserContext userContext, MarketplaceEntities entities)
        {
            _user = user;
            _userContext = userContext;
            _entities = entities;

            var contacts = _entities.Contact.Where(it => it.UserId == _userContext.User.Id).ToList();
            Contacts = new ObservableCollection<Contact>(contacts);

            _userContext.AddedByUserId += AddUser;
            _userContext.RemovedByUserId += RemoveUser;

            SelectUserCommand = new RelayCommand(SelectUser);
            SearchUserCommand = new NavCommand(search);
        }

        private void SelectUser(object param)
        {
            if (param is Contact contact)
            {
                _userContext.SelectedUser = contact.User1;
                _user.Push();
            }
        }

        private void AddUser(int userId)
        {
            var contact = Contacts.FirstOrDefault(it => it.ContactId == userId);
            Contacts.Add(contact);
        }

        private void RemoveUser(int userId)
        {
            var contact = Contacts.FirstOrDefault(it => it.ContactId == userId);
            Contacts.Remove(contact);
        }

        public override void Dispose()
        {
            _userContext.AddedByUserId -= AddUser;
            _userContext.RemovedByUserId -= RemoveUser;

            GC.SuppressFinalize(this);
        }
    }
}
