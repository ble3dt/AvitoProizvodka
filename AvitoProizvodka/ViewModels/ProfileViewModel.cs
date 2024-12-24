using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.IO;
using AvitoProizvodka.Core.Nav;
using System.Windows.Input;
using AvitoProizvodka.Core.Command;

namespace AvitoProizvodka.ViewModels
{
    public class ProfileViewModel : ViewModel
    {
        private readonly INavService _navUser;
        private readonly INavService _product;
        private readonly UserContext _userContext;
        private readonly MarketContext _marketContext;
        private readonly MarketplaceEntities _entities;

        private User _user;
        private byte[] _icon = Array.Empty<byte>();

        public byte[] Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        }
        public User User
        {
            get => _user;
            set
            {
                _user = value; OnPropertyChanged();
            }
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

        public ICommand EditProfileCommand { get; }
        public ICommand ExitProfileCommand { get; }

        public ICommand SelectUserCommand { get; }
        public ICommand SelectProductCommand { get; }

        public ProfileViewModel(INavService edit, INavService login, INavService user, INavService product, UserContext userContext, MarketContext marketContext, MarketplaceEntities entities)
        {
            _navUser = user;
            _product = product;
            _userContext = userContext;
            _marketContext = marketContext;
            _entities = entities;

            LoadContent();

            EditProfileCommand = new NavCommand(edit);
            ExitProfileCommand = new NavPopCommand(login);

            SelectUserCommand = new RelayCommand(SelectUser);
            SelectProductCommand = new RelayCommand(SelectProduct);
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

        private void LoadContent()
        {
            try
            {
                User = _entities.User.FirstOrDefault(it => it.Id == _userContext.User.Id);
                if (User.UserImage.Count > 0)
                {
                    Icon = User.UserImage.OrderBy(it => it.Timestamp).Last().Image.Binary;
                }
                Contacts = User.Contact.ToList();
                Products = User.UserProduct.Select(it => it.Product).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
