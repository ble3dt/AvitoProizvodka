using AvitoProizvodka.Core.Command;
using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Core.Utilities;
using AvitoProizvodka.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace AvitoProizvodka.ViewModels
{
    public class ProductAddingViewModel : ViewModel
    {
        private readonly UserContext _userContext;
        private readonly MarketContext _marketContext;
        private readonly MarketplaceEntities _entities;
        private ObservableCollection<byte[]> _bytes = new ObservableCollection<byte[]>();

        public Product Product { get; set; } = new Product();
        public ObservableCollection<byte[]> Bytes
        {
            get => _bytes;
            set
            {
                _bytes = value; OnPropertyChanged();
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }

        public ProductAddingViewModel(INavService back, UserContext userContext, MarketContext marketContext, MarketplaceEntities entities)
        {
            _userContext = userContext;
            _marketContext = marketContext;
            _entities = entities;

            GoBackCommand = new BackCommand(back);
            AddImageCommand = new RelayCommand(AddImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
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
                    Bytes.Add(bytes);
                }
            }
        }
        private void RemoveImage(object param)
        {
            if (param is byte[] bytes)
            {
                Bytes.Remove(bytes);
            }
        }

        private async Task SaveChanges()
        {
            if (string.IsNullOrEmpty(Product.Title) || string.IsNullOrEmpty(Product.Subtitle) || Bytes.Count == 0)
            {
                MessageBox.Show("Заполните поля");
                return;
            }

            try
            {
                _entities.Product.Add(Product);

                var userProducts = new UserProduct
                {
                    ProductId = Product.Id,
                    UserId = _userContext.User.Id,
                    Timestamp = DateTime.Now,
                    IsNotActive = false,
                };
                _entities.UserProduct.Add(userProducts);

                await _entities.SaveChangesAsync();

                var imageList = new List<Image>();
                foreach (var bytes in Bytes)
                {
                    var image = new Image
                    {
                        Binary = bytes,
                        Timestamp = DateTime.Now,
                    };

                    imageList.Add(image);
                }

                _entities.Image.AddRange(imageList);
                await _entities.SaveChangesAsync();

                var productsImages = new List<ProductImage>();
                foreach (var item in imageList)
                {
                    var productImage = new ProductImage
                    {
                        ImageId = item.Id,
                        ProductId = Product.Id,
                        Timestamp = DateTime.Now,
                    };
                    productsImages.Add(productImage);
                }

                _entities.ProductImage.AddRange(productsImages);
                await _entities.SaveChangesAsync();

                Product.ProductImage = productsImages;

                _marketContext.AddMyProduct(Product);

                GoBackCommand.Execute(null);
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
