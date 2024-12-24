using AvitoProizvodka.Core.Utilities;
using System;
using System.Linq;

namespace AvitoProizvodka.Data
{
    public partial class User : ObservableObject
    {
        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        public byte[] Icon
        {
            get
            {
                if (UserImage.Count > 0)
                {
                    return UserImage.OrderBy(it => it.Timestamp).LastOrDefault().Image.Binary;
                }
                else
                {
                    return Array.Empty<byte>();
                }
            }
        }
    }
}
