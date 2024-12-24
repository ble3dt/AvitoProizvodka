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
using System.Windows;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class AnotherProfileViewModel : ViewModel
    {
        private readonly INavService _navUser;
        private readonly INavService _product;
        private readonly UserContext _userContext;
        private readonly MarketContext _marketContext;
        private readonly MarketplaceEntities _entities;

        public User SelectedUser => _userContext.SelectedUser;

        public bool IsNotLoggedUser { get; }

        private bool isContact;
        public bool IsContact
        {
            get => isContact; set
            {
                isContact = value; OnPropertyChanged();
            }
        }

        private byte[] _icon = Array.Empty<byte>();
        public byte[] Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        }

        private List<Contact> _contacts = new List<Contact>();
        public List<Contact> Contacts
        {
            get => _contacts;
            set { _contacts = value; OnPropertyChanged(); }
        }

        private List<Product> _products;
        public List<Product> Products
        {
            get => _products; set
            {
                _products = value; OnPropertyChanged();
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand AddContactCommand { get; }

        public ICommand SelectUserCommand { get; }
        public ICommand SelectProductCommand { get; }

        public AnotherProfileViewModel(INavService back, INavService user, INavService product, UserContext userContext, MarketContext marketContext, MarketplaceEntities entities)
        {
            _navUser = user;
            _product = product;
            _userContext = userContext;
            _marketContext = marketContext;
            _entities = entities;

            GoBackCommand = new BackCommand(back);
            AddContactCommand = new RelayAsyncCommand(AddContact);

            SelectUserCommand = new RelayCommand(SelectUser);
            SelectProductCommand = new RelayCommand(SelectProduct);

            if (_userContext.SelectedUser.UserImage.Count > 0)
            {
                var list = _userContext.SelectedUser.UserImage.ToList();
                Icon = list.OrderBy(it => it.Timestamp).Last().Image.Binary;
            }

            Contacts = SelectedUser.Contact.ToList();
            Products = SelectedUser.UserProduct.Select(it => it.Product).ToList();

            IsNotLoggedUser = _userContext.SelectedUser.Id != _userContext.User.Id;

            if (IsNotLoggedUser)
            {
                IsContact = _entities.Contact
                    .FirstOrDefault(it => it.UserId == _userContext.User.Id && it.ContactId == SelectedUser.Id) != null;
            }
        }

        private void SelectUser(object param)
        {
            if (param is Contact contact)
            {
                _userContext.SelectedUser = contact.User1;
                _navUser.Push();
            }
        }

        private void SelectProduct(object param)
        {
            if (param is Product product)
            {
                _marketContext.SelectedProduct = product;
                _product.Push();
            }
        }

        private async Task AddContact()
        {
            try
            {
                if (SelectedUser.Id == _userContext.User.Id)
                {
                    return;
                }

                var contact = await _entities.Contact.FirstOrDefaultAsync(it => it.UserId == _userContext.User.Id && it.ContactId == SelectedUser.Id);

                if (contact is null)
                {
                    var newContact = new Contact
                    {
                        UserId = _userContext.User.Id,
                        ContactId = SelectedUser.Id,
                        Timestamp = DateTime.Now
                    };

                    _entities.Contact.Add(newContact);
                    await _entities.SaveChangesAsync();

                    _userContext.AddByUserId(newContact.ContactId);
                }
                else
                {
                    int userId = contact.ContactId;

                    _entities.Contact.Remove(contact);
                    await _entities.SaveChangesAsync();

                    _userContext.RemoveByUserId(userId);
                }

                IsContact = !IsContact;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
