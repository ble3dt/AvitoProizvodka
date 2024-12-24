using AvitoProizvodka.Core.Contexts;
using AvitoProizvodka.Core.Nav;
using AvitoProizvodka.Data;
using AvitoProizvodka.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace AvitoProizvodka
{
    public partial class App : Application
    {
        private readonly IServiceProvider _provider;

        public App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<MarketplaceEntities>();
            services.AddSingleton<MainContext>();
            services.AddSingleton<MarketContext>();
            services.AddSingleton<UserContext>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>(p =>
            {
                return new MainViewModel(
                    LoginFactory(p),
                    CompositeFactory(p),
                    ProfileFactory(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MainContext>(),
                    p.GetRequiredService<MarketplaceEntities>()
                    );
            });

            services.AddTransient<AuthViewModel>(p =>
            {
                return new AuthViewModel(
                    RegFactory(p),
                    CompositeFactory(p),
                    ProfileFactory(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MarketplaceEntities>()
                    );
            });
            services.AddTransient<RegViewModel>(p =>
            {
                return new RegViewModel(
                    MainBackOnly(p),
                    CompositeFactory(p),
                    ProfileFactory(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MarketplaceEntities>()
                    );
            });
            services.AddTransient<LayoutViewModel>(p =>
            {
                return new LayoutViewModel(
                    MarketFactory(p),
                    CartFactory(p),
                    ContactsFactory(p),
                    ProfileFactory(p),
                    ActivitiesFactory(p),
                    p.GetRequiredService<MarketContext>());
            });
            services.AddTransient<ProfileViewModel>(p =>
            {
                return new ProfileViewModel(
                    EditProfileFactory(p), 
                    LoginFactory(p),
                    AnotherProfileFactory(p),
                    ProductInfoFactory(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MarketContext>(),
                    p.GetRequiredService<MarketplaceEntities>());
            });
            services.AddTransient<ActivitiesViewModel>(p =>
            {
                return new ActivitiesViewModel(
                    ProductAddingFactory(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MarketContext>(),
                    p.GetRequiredService<MarketplaceEntities>()
                    );
            });
            services.AddTransient<ProductAddingViewModel>(p =>
            {
                return new ProductAddingViewModel(
                    MarketBackOnly(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MarketContext>(),
                    p.GetRequiredService<MarketplaceEntities>()
                    );
            });
            services.AddTransient<MainMarketViewModel>(p =>
            {
                return new MainMarketViewModel(
                    ProductInfoFactory(p), 
                    ProductListFactory(p),
                    p.GetRequiredService<UserContext>(), 
                    p.GetRequiredService<MarketContext>(), 
                    p.GetRequiredService<MarketplaceEntities>());
            });
            services.AddTransient<ProductInfoViewModel>(p =>
            {
                return new ProductInfoViewModel(
                    MarketBackOnly(p), 
                    AnotherProfileFactory(p), 
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MarketContext>(),
                    p.GetRequiredService<MarketplaceEntities>()
                    );
            });
            services.AddTransient<CartViewModel>(p =>
            {
                return new CartViewModel(
                    ProductInfoFactory(p), 
                    p.GetRequiredService<UserContext>(), 
                    p.GetRequiredService<MarketContext>(), 
                    p.GetRequiredService<MarketplaceEntities>());
            });
            services.AddTransient<AnotherProfileViewModel>(p =>
            {
                return new AnotherProfileViewModel(
                    MarketBackOnly(p),
                    AnotherProfileFactory(p),
                    ProductInfoFactory(p),
                    p.GetRequiredService<UserContext>(),
                    p.GetRequiredService<MarketContext>(),
                    p.GetRequiredService<MarketplaceEntities>()
                    );
            });
            services.AddTransient<ContactsViewModel>(p =>
            {
                return new ContactsViewModel(AnotherProfileFactory(p), ContactListFactory(p), p.GetRequiredService<UserContext>(), p.GetRequiredService<MarketplaceEntities>());
            });
            services.AddTransient<EditProfileViewModel>(p =>
            {
                return new EditProfileViewModel(ProfileFactory(p), p.GetRequiredService<UserContext>(), p.GetRequiredService<MarketplaceEntities>());
            });
            services.AddTransient<ProductListViewModel>(p =>
            {
                return new ProductListViewModel(MarketBackOnly(p), ProductInfoFactory(p), p.GetRequiredService<MarketContext>(), p.GetRequiredService<MarketplaceEntities>());
            });
            services.AddTransient<ContactListViewModel>(p =>
            {
                return new ContactListViewModel(MarketBackOnly(p), AnotherProfileFactory(p), p.GetRequiredService<UserContext>(), p.GetRequiredService<MarketplaceEntities>());
            });
            
            _provider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var window = _provider.GetRequiredService<MainWindow>();
            var viewModel = _provider.GetRequiredService<MainViewModel>();

            MainWindow = window;
            MainWindow.DataContext = viewModel;
            MainWindow.Show();

            base.OnStartup(e);
        }

        private MainNavService LoginFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<AuthViewModel>);
        }
        private MainNavService RegFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<RegViewModel>);
        }
        private MainNavService CompositeFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<LayoutViewModel>);
        }
        private MainNavService MainBackOnly(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>());
        }

        private MarketNavService ProfileFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<ProfileViewModel>);
        }
        private MarketNavService ActivitiesFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<ActivitiesViewModel>);
        }
        private MarketNavService ProductAddingFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<ProductAddingViewModel>);
        }
        private MarketNavService MarketFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<MainMarketViewModel>);
        }
        private MarketNavService ProductInfoFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<ProductInfoViewModel>);
        }
        private MarketNavService CartFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<CartViewModel>);
        }
        private MarketNavService AnotherProfileFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<AnotherProfileViewModel>);
        }
        private MarketNavService ContactsFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<ContactsViewModel>);
        }
        private MarketNavService EditProfileFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<EditProfileViewModel>);
        }
        private MarketNavService ProductListFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<ProductListViewModel>);
        }
        private MarketNavService ContactListFactory(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>(), p.GetRequiredService<ContactListViewModel>);
        }
        private MarketNavService MarketBackOnly(IServiceProvider p)
        {
            return new MarketNavService(p.GetRequiredService<MarketContext>());
        }
    }
}
