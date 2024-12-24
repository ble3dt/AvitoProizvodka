using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace AvitoProizvodka.ViewModels
{
    public class RegViewModel : ViewModel
    {
        private readonly MarketplaceEntities _entities;
        private readonly INavService _composite;
        private readonly INavService _profile;
        private readonly UserContext _userContext;

        public User User { get; set; } = new User();

        private bool _isNotBlocked = true;
        public bool IsNotBlocked
        {
            get => _isNotBlocked;
            set
            {
                _isNotBlocked = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegCommand { get; }
        public ICommand GoBackCommand { get; }

        public RegViewModel(
            INavService back,
            INavService composite,
            INavService profile,
            UserContext userContext,
            MarketplaceEntities entities
            )
        {
            _profile = profile;
            _composite = composite;

            _entities = entities;
            _userContext = userContext;

            GoBackCommand = new BackCommand(back);
            RegCommand = new RelayAsyncCommand(RegAsync);
        }

        private async Task RegAsync(object param)
        {
            IsNotBlocked = false;

            try
            {
                if (param is PasswordBox passwordBox)
                {
                    var password = passwordBox.Password;
                    User.Password = password;

                    var isAccountExists = await _entities.User.AnyAsync(it => it.Email == User.Email || it.PhoneNo == User.PhoneNo);

                    if (isAccountExists)
                    {
                        MessageBox.Show("Аккаунт с такой почтой и/или таким телефоном уже существует");
                        return;
                    }

                    var email = new EmailAddressAttribute();
                    var phone = new PhoneAttribute();
                    
                    if (!email.IsValid(User.Email))
                    {
                        MessageBox.Show("Неверная почта");
                        return;
                    }
                    if (!phone.IsValid(User.PhoneNo))
                    {
                        MessageBox.Show("Неверный телефон");
                        return;
                    }

                    _entities.User.Add(User);
                    await _entities.SaveChangesAsync();

                    _userContext.User = User;
                    _profile.Push();
                    _composite.PopAndPush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                IsNotBlocked = true;
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
