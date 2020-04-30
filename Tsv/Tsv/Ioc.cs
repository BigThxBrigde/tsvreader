/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Tsv"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Tsv.Service;
using Tsv.ViewModel;

namespace Tsv
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class Ioc
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        private Ioc()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register(() =>Config.Instance);
            SimpleIoc.Default.Register(() => GroupEngine.Instance);

        }

        private static Ioc @default;
        public static Ioc Default => @default ?? (@default = new Ioc());

        public T GetInstance<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}