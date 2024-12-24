using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class EditProfileViewModel : ViewModel
    {
        private byte[] icon;
        private readonly UserContext _userContext;
        private readonly MarketplaceEntities _entities;

        public ICommand GoBackCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand AddImageCommand { get; }

        public byte[] Icon
        {
            get => icon; set
            {
                icon = value; OnPropertyChanged();
            }
        }
        public User User { get; set; } = new User();

        public EditProfileViewModel(INavService back, UserContext userContext, MarketplaceEntities entities)
        {
            _userContext = userContext;
            _entities = entities;

            User.Title = _userContext.User.Title;
            User.Subtitle = _userContext.User.Subtitle;
            Icon = _userContext.User.Icon;

            GoBackCommand = new NavPopCommand(back);
            AddImageCommand = new RelayCommand(AddImage);
            SaveChangesCommand = new RelayAsyncCommand(SaveChanges);
        }


        private void AddImage()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Медиа|*.jpg;*.png;;*.jpeg",
                Multiselect = true,
            };

            if (fileDialog.ShowDialog().GetValueOrDefault())
            {
                foreach (var item in fileDialog.FileNames)
                {
                    var bytes = File.ReadAllBytes(item);
                    Icon = bytes;
                }
            }
        }

        private async Task SaveChanges()
        {
            try
            {
                var user = _userContext.User;

                user.Title = User.Title;
                user.Subtitle = User.Subtitle;

                var image = new Image
                {
                    Binary = Icon,
                    Timestamp = DateTime.Now,
                };

                _entities.Image.Add(image);
                await _entities.SaveChangesAsync();

                var userImage = new UserImage
                {
                    ImageId = image.Id,
                    Timestamp = DateTime.Now,
                    UserId = _userContext.User.Id
                };

                _entities.UserImage.Add(userImage);
                await _entities.SaveChangesAsync();

                GoBackCommand.Execute(null);
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
