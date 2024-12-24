using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using AvitoProizvodka.Properties;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class AuthViewModel : ViewModel
    {
        private readonly MarketplaceEntities _entities;
        private readonly INavService _composite;
        private readonly INavService _profile;
        private readonly UserContext _userContext;

        public User User { get; set; } = new User();

        public ICommand GoToRegistrationCommand { get; }
        public ICommand LoginCommand { get; }

        private bool isBlocked = true;
        public bool IsNotBlocked
        {
            get => isBlocked;
            set
            {
                isBlocked = value;
                OnPropertyChanged();
            }
        }

        public AuthViewModel(
            INavService reg, 
            INavService composite, 
            INavService profile, 
            UserContext userContext, 
            MarketplaceEntities entities)
        {
            _profile = profile;
            _composite = composite;

            _entities = entities;
            _userContext = userContext;

            Settings.Default.Reset();
            Settings.Default.Save();

            GoToRegistrationCommand = new NavCommand(reg);
            LoginCommand = new RelayAsyncCommand(LoginAsync);
        }

        private async Task LoginAsync(object param)
        {
            IsNotBlocked = false;

            try
            {
                if (param is PasswordBox passwordBox)
                {
                    var password = passwordBox.Password;

                    var email = User.Email;

                    var user = await _entities.User.FirstOrDefaultAsync(it => it.Email == email && it.Password == password);

                    if (user is null)
                    {
                        MessageBox.Show("Неправильные данные");
                    }
                    else
                    {
                        Settings.Default.UserId = user.Id;
                        Settings.Default.Save();

                        _userContext.User = user;
                        _profile.Push();
                        _composite.PopAndPush();
                    }
                }
            }
            catch(Exception ex)
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
