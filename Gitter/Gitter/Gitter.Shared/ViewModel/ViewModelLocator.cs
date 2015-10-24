/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Gitter"
                           x:Key="Locator" />
  </Application.Resources>

  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using Gitter.ViewModel.Abstract;
using Gitter.ViewModel.Concrete;
using Gitter.Views;
using GitterSharp.Services;
using Microsoft.Practices.ServiceLocation;

namespace Gitter.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            // Services
            if (!SimpleIoc.Default.IsRegistered<INavigationService>())
            {
                var navigationService = CreateNavigationService();
                SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            }

            SimpleIoc.Default.Register<IGitterApiService, GitterApiService>();
            SimpleIoc.Default.Register<AuthenticationService>();

            SimpleIoc.Default.Register<ISessionService, SessionService>();
            SimpleIoc.Default.Register<IApplicationStorageService, ApplicationStorageService>();
            SimpleIoc.Default.Register<IPasswordStorageService, PasswordStorageService>();
#if WINDOWS_PHONE_APP
            SimpleIoc.Default.Register<ILocalNotificationService, WindowsPhoneNotificationService>();
#endif
#if WINDOWS_APP || WINDOWS_UWP
            SimpleIoc.Default.Register<ILocalNotificationService, WindowsNotificationService>();
#endif
            SimpleIoc.Default.Register<IRatingService, RatingService>();
            SimpleIoc.Default.Register<IBackgroundTaskService, BackgroundTaskService>();
            SimpleIoc.Default.Register<IProgressIndicatorService, ProgressIndicatorService>();
            SimpleIoc.Default.Register<IEventService, EventService>();

            // ViewModels
            SimpleIoc.Default.Register<IMainViewModel, MainViewModel>();
            SimpleIoc.Default.Register<ILoginViewModel, LoginViewModel>();
            SimpleIoc.Default.Register<IRoomViewModel, RoomViewModel>();
            SimpleIoc.Default.Register<IFullImageViewModel, FullImageViewModel>();
            SimpleIoc.Default.Register<IAboutViewModel, AboutViewModel>();
        }

        #endregion


        #region Navigation Service (Page declaration)

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();

            navigationService.Configure("SplashScreen", typeof(SplashScreenPage));
            navigationService.Configure("Main", typeof(MainPage));
#if WINDOWS_PHONE_APP
            navigationService.Configure("Room", typeof(RoomPage));
            navigationService.Configure("About", typeof(AboutPage));
            navigationService.Configure("FullImage", typeof(FullImagePage));
#endif

            return navigationService;
        }

        #endregion


        #region ViewModels

        public static IMainViewModel Main => ServiceLocator.Current.GetInstance<IMainViewModel>();
        public static ILoginViewModel Login => ServiceLocator.Current.GetInstance<ILoginViewModel>();
        public static IFullImageViewModel FullImage => ServiceLocator.Current.GetInstance<IFullImageViewModel>();
        public static IAboutViewModel About => ServiceLocator.Current.GetInstance<IAboutViewModel>();

        #endregion
    }
}