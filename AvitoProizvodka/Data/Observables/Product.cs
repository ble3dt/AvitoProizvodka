using AvitoProizvodka.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvitoProizvodka.Data
{
    public partial class Product : ObservableObject
    {
        public byte[] ShowcaseImage
        {
            get
            {
                if (ProductImage.Count > 0)
                {
                    return ProductImage.OrderBy(it => it.Timestamp).Last().Image.Binary;
                }
                else
                {
                    return Array.Empty<byte>();
                }
            }
        }

        public bool IsSold => UserProduct.FirstOrDefault()?.IsNotActive ?? false;

        public string IsSoldText
        {
            get
            {
                return IsSold ? "Продано" : string.Empty;
            }
        }

        public void TrackIsSold()
        {
            OnPropertyChanged(nameof(IsSold));
            OnPropertyChanged(nameof(IsSoldText));
        }
    }
}
