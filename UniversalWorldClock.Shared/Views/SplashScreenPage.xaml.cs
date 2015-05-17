using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Practices.Prism.Mvvm;

namespace UniversalWorldClock.Views
{
    public sealed partial class SplashScreenPage:IView
    {
        private readonly SplashScreen _splashScreen;

        public SplashScreenPage(SplashScreen splashScreen)
        {
            _splashScreen = splashScreen;
            this.InitializeComponent();

            this.SizeChanged += ExtendedSplashScreen_SizeChanged;
            this.splashImage.ImageOpened += splashImage_ImageOpened;
        }

        void splashImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            Resize();
            Window.Current.Activate();
        }

        void ExtendedSplashScreen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Resize();
        }

        private void Resize()
        {
            if (this._splashScreen == null) return;

            this.splashImage.Height = this._splashScreen.ImageLocation.Height;
            this.splashImage.Width = this._splashScreen.ImageLocation.Width;

            this.splashImage.SetValue(Canvas.TopProperty, this._splashScreen.ImageLocation.Top);
            this.splashImage.SetValue(Canvas.LeftProperty, this._splashScreen.ImageLocation.Left);

            this.progressRing.SetValue(Canvas.TopProperty, this._splashScreen.ImageLocation.Top  + 50);
            this.progressRing.SetValue(Canvas.LeftProperty, this._splashScreen.ImageLocation.Left + this._splashScreen.ImageLocation.Width / 2 - this.progressRing.Width / 2);
        }
    }
}
