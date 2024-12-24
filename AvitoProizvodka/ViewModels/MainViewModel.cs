using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using AvitoProizvodka.Properties;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly MainContext _mainContext;

        public ViewModel CurrentViewModel => _mainContext.CurrentViewModel;

        public MainViewModel(INavService auth, INavService composite, INavService profile, UserContext userContext, MainContext mainContext, MarketplaceEntities entities)
        {
            _mainContext = mainContext;

            _mainContext.ViewModelChanged += TrackViewModel;

            var userId = Settings.Default.UserId;
            if (userId == 0)
            {
                auth.Push();
            }
            else
            {
                var user = entities.User.FirstOrDefault(it => it.Id == userId);

                userContext.User = user;
                profile.Push();
                composite.PopAndPush();
            }
        }

        private void TrackViewModel()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
